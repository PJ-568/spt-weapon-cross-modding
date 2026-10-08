# 原理

> 本文说明 PJ568's Weapon Cross Modding 如何在 SPT 4.1.x 服务端修改物品数据，使其实现跨武器兼容。

## 数据模型

SPT 服务端把所有物品模板存放在 `TemplateTable.Items`（`Dictionary<MongoId, TemplateItem>`）中。与兼容性相关的字段有三处：

### 槽位过滤器

每个 `TemplateItem` 的 `Properties.Slots` 是一个 `Slot` 列表。每个 `Slot` 有：

- `Name`：槽位名，如 `mod_scope`、`mod_sight_front`、`mod_sight_rear`、`mod_barrel`、`mod_handguard`、`mod_stock`、`mod_tactical`、`mod_tactical_000`、`mod_tactical_001`、`mod_pistol_grip`、`mod_mount`、`mod_muzzle`、`mod_reciever`。
- `Properties.Filters`：`SlotFilter` 列表。
- `SlotFilter.Filter`：`HashSet<MongoId>`，即该槽位允许安装的物品 id 白名单。

**要让一件物品能装进某个槽位，就是把这个物品的 id 加进该槽位过滤器的白名单。**

### 弹药容器

除槽位外，武器与弹匣还各有一个「弹药容器」，其结构与槽位一致（同样是 `Slot` 列表，同样有 `Properties.Filters[].Filter`）：

- 武器：`Properties.Chambers`（膛室，`Slot.Name` 为 `patron_in_weapon`），决定能上膛的弹药模板。
- 弹匣：`Properties.Cartridges`（装填位，`Slot.Name` 为 `cartridges`），决定能装填的弹药模板。

**要让武器或弹匣接受某种弹药，就是把这个弹药的模板 id 加进对应容器的过滤器白名单。**

### 冲突列表

`TemplateItem.Properties.ConflictingItems` 是 `HashSet<MongoId>`。若 A 的 `ConflictingItems` 含 B 的 id（或反之），则 A 与 B 不能同时安装在一件武器上。

## 注入流程

插件入口实现 `IOnLoad`，并以 `[Injectable(InjectionType.Singleton, OnLoadOrder.Preload + 4)]` 注册，在服务端预加载阶段、原版数据库构建完成后运行一次。

`OnLoadAsync(CancellationToken)` 的流程：

1. 取出 `TemplateTable.Items`。
2. 对每一组「宿主物品 + 槽位名 + 待追加 id」，调用 `AddItemIdsToSlot`：
   - 若宿主 id 不在 `Items` 中，记警告并跳过；若宿主没有槽位，记警告并跳过。
   - 按槽位名（`OrdinalIgnoreCase`）找到宿主的所有同名槽。
   - 对每个槽的每个 `SlotFilter`，把各 id 加入 `Filter`。
3. 对每一组「宿主物品 + 弹药容器 + 待追加弹药 id」，调用 `AddAmmoIdsToContainer`，逻辑与上一步相同，只是把目标槽换成武器的 `Chambers` 或弹匣的 `Cartridges`。
4. 对每一组互斥关系，调用 `AddConflictingItems`，把冲突 id 加入宿主物品的 `ConflictingItems`。
4. 整个过程包在 `try/catch` 中：任何异常都被记录为 `error`，不会中断服务端启动。

## 幂等

注入全部基于 `HashSet` 的 `Add`：

- `Add` 返回 `true` 表示新增成功，记一条 info 日志。
- 返回 `false` 表示已存在，仅记一条 debug 日志，不重复写入。

因此插件重复加载、或与其它同样追加白名单的模组共存时，都不会产生重复条目或冲突。

### 缺失物品的处理

- **白名单宿主缺失**（`AddItemIdsToSlot` 找不到宿主）：记警告并跳过。追加的 id 本身无需存在。
- **冲突目标缺失**（`AddConflictingItems` 找不到宿主）：记 debug 并静默跳过。对由第三方模组注入的物品（如 HUXWRX HX-QD 消音器），这是有意为之：只要另一侧（PPSh-41 防尘盖）仍能写入冲突记录，互斥关系就已建立，从而避免对第三方模组加载顺序的依赖。

## 日志形态

成功的注入会输出（`Info`）：

```
[Info][...] WeaponCrossModding: added '<物品 id>' to slot '<槽位名>' (<宿主物品名>)
[Info][...] WeaponCrossModding: added conflicting item '<物品 id>' to '<宿主标签>' (<宿主物品名>)
```

已存在或缺失则只写 `Debug`，不打扰常规日志。

===============================================================

# Principles

> This document explains how PJ568's Weapon Cross Modding modifies item data on the SPT 4.1.x server to achieve cross-weapon compatibility.

## Data Model

The SPT server stores every item template in `TemplateTable.Items` (`Dictionary<MongoId, TemplateItem>`). Three fields are relevant to compatibility:

### Slot filters

Each `TemplateItem`'s `Properties.Slots` is a list of `Slot`. Every `Slot` has:

- `Name`: the slot name, such as `mod_scope`, `mod_sight_front`, `mod_sight_rear`, `mod_barrel`, `mod_handguard`, `mod_stock`, `mod_tactical`, `mod_tactical_000`, `mod_tactical_001`, `mod_pistol_grip`, `mod_mount`, `mod_muzzle` and `mod_reciever`.
- `Properties.Filters`: a list of `SlotFilter`.
- `SlotFilter.Filter`: a `HashSet<MongoId>`, the whitelist of item ids the slot accepts.

**Making an item fit a slot means adding that item's id to the slot filter's whitelist.**

### Ammo containers

Besides slots, weapons and magazines each have an "ammo container" with the same structure as a slot (also a list of `Slot`, also with `Properties.Filters[].Filter`):

- Weapon: `Properties.Chambers` (the chamber, `Slot.Name` is `patron_in_weapon`), which determines the ammo templates that can be chambered.
- Magazine: `Properties.Cartridges` (the loading position, `Slot.Name` is `cartridges`), which determines the ammo templates that can be loaded.

**Making a weapon or magazine accept an ammo type means adding that ammo's template id to the corresponding container filter's whitelist.**

### Conflict list

`TemplateItem.Properties.ConflictingItems` is a `HashSet<MongoId>`. If A's `ConflictingItems` contains B's id (or vice versa), A and B cannot be installed on the same weapon.

## Injection Flow

The plugin entry implements `IOnLoad` and is registered with `[Injectable(InjectionType.Singleton, OnLoadOrder.Preload + 4)]`, so it runs once during the server preload phase, after the vanilla database has been built.

`OnLoadAsync(CancellationToken)` works as follows:

1. Fetch `TemplateTable.Items`.
2. For each group of "owner item + slot name + ids to add", call `AddItemIdsToSlot`:
   - If the owner id is missing from `Items`, log a warning and skip; if the owner has no slots, log a warning and skip.
   - Find all same-named slots (case-insensitive, `OrdinalIgnoreCase`).
   - For every `SlotFilter` of every matching slot, add the ids to `Filter`.
3. For each group of "owner item + ammo container + ammo ids to add", call `AddAmmoIdsToContainer`, which works exactly like the previous step but targets the weapon's `Chambers` or the magazine's `Cartridges` instead of a slot.
4. For each mutual exclusion, call `AddConflictingItems` to add the conflicting id to the owner item's `ConflictingItems`.
4. The whole process is wrapped in `try/catch`: any exception is logged as `error` and never interrupts server startup.

## Idempotency

All injections rely on `HashSet.Add`:

- `Add` returns `true` when the entry is new, and an info log is written.
- It returns `false` when the entry already exists; only a debug log is written and nothing is duplicated.

Reloading the plugin, or running it alongside other mods that also append to whitelists, therefore never produces duplicates or conflicts.

### Handling missing items

- **Missing whitelist owner** (`AddItemIdsToSlot` cannot find the owner): log a warning and skip. The added ids themselves need not exist.
- **Missing conflict target** (`AddConflictingItems` cannot find the owner): log at debug level and skip silently. This is intentional for items injected by third-party mods (such as the HUXWRX HX-QD suppressors): as long as the other side (the PPSh-41 dust cover) still records the conflict, the exclusion is established, avoiding any dependency on third-party load order.

## Log Shape

Successful injections print (`Info`):

```
[Info][...] WeaponCrossModding: added '<item id>' to slot '<slot name>' (<owner item name>)
[Info][...] WeaponCrossModding: added conflicting item '<item id>' to '<owner label>' (<owner item name>)
```

If the entry already exists or is missing, only `Debug` is written, keeping the regular log quiet.
