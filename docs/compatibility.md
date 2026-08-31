# 兼容性与限制

## 支持版本

- SPT `4.1.x` 服务端（ServerMod）。

## 与其它模组的关系

- **幂等共存**：所有注入基于 `HashSet.Add`，与同样向槽位过滤器追加白名单的模组共存时不会产生重复条目，也不会互相覆盖。若另一模组已加入同一 id，本插件只记一条 `Debug` 日志。
- **`WTT-ContentBackport`**：部分物品由该模组注入，例如 HUXWRX HX-QD 7.62x51 消音器（黑 / 黄褐色）与 Tyrant Designs MOD Chevron AR-15 镂空手枪式握把（黑 / 黄 / 红）。
  - 这些物品作为**白名单新增项**时，插件只追加 id，不要求其存在，因此无论该模组是否安装都能正常加载。
  - 这些物品作为**冲突目标**时（HUXWRX 消音器），若其尚未注入，插件仅记 `Debug` 并跳过；由于 PPSh-41 防尘盖一侧的冲突记录仍会建立，互斥关系依旧成立，并避免了对第三方模组加载顺序的依赖。
- 插件只改写物品数据库，不注册 Harmony patch，也不与客户端插件争抢运行时方法。

## 已知限制

- **依赖物品 id 与槽位名**：注入目标取自具体物品 id 与槽位名；若上游数据在版本更新中变更，相关条目会跳过或失效。
- **必须由服务端加载**：这是服务端模组，仅在服务端数据库层面放宽白名单；未部署到服务端时游戏内不会生效。
- **版本敏感**：`SPTarkov.Server.Core` 等依赖与 SPT 服务端版本对齐，跨越 4.1.x 之外的版本可能需要重新适配。

## 命名说明

| 项目 | 值 |
| --- | --- |
| 展示名 | `PJ568's Weapon Cross Modding` |
| 仓库 slug | `spt-weapon-cross-modding` |
| 程序集 / 部署目录 | `WeaponCrossModding` |
| ModGuid | `com.pj568.weaponcrossmodding` |
| 作者 | `PJ568` |
| 许可证 | MIT |

===============================================================

# Compatibility and Limitations

## Supported Versions

- SPT `4.1.x` server (ServerMod).

## Interaction with Other Mods

- **Idempotent coexistence**: every injection relies on `HashSet.Add`, so running alongside mods that also append to slot filters produces no duplicates and no overwrites. If another mod already added the same id, this plugin only writes a `Debug` entry.
- **`WTT-ContentBackport`**: some items are injected by that mod, such as the HUXWRX HX-QD 7.62x51 suppressors (black / tan) and the Tyrant Designs MOD Chevron AR-15 skeletonized pistol grips (black / yellow / red).
  - When these items are **whitelist additions**, the plugin only appends their ids and does not require them to exist, so it loads fine whether or not that mod is installed.
  - When these items are **conflict targets** (the HUXWRX suppressors), if they are not yet injected the plugin only logs `Debug` and skips; because the PPSh-41 dust cover side still records the conflict, the exclusion still holds, avoiding any dependency on third-party load order.
- The plugin only rewrites the item database. It registers no Harmony patch and never contends with client plugins for runtime methods.

## Known Limitations

- **Depends on item ids and slot names**: the injection targets come from concrete item ids and slot names; if upstream data changes in a version update, the affected entries are skipped or stop applying.
- **Must be loaded by the server**: this is a server mod that widens whitelists only at the server database level; it has no in-game effect unless deployed to the server.
- **Version sensitive**: dependencies such as `SPTarkov.Server.Core` are version-aligned with the SPT server, and going beyond the 4.1.x line may require re-adaptation.

## Naming Notes

| Item | Value |
| --- | --- |
| Display name | `PJ568's Weapon Cross Modding` |
| Repository slug | `spt-weapon-cross-modding` |
| Assembly / deploy directory | `WeaponCrossModding` |
| ModGuid | `com.pj568.weaponcrossmodding` |
| Author | `PJ568` |
| License | MIT |
