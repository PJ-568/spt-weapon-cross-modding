# 兼容性与限制

## 支持版本

- SPT `4.1.x` 服务端（ServerMod）。

## 与其它模组的关系

- **幂等共存**：所有注入基于 `HashSet.Add`，与同样向槽位过滤器追加白名单的模组共存时不会产生重复条目，也不会互相覆盖。若另一模组已加入同一 id，本插件只记一条 `Debug` 日志。
- **`WTT-ContentBackport`**：部分物品由该模组注入，例如 HUXWRX HX-QD 7.62x51 消音器（黑 / 黄褐色）、Tyrant Designs MOD Chevron AR-15 镂空手枪式握把（黑 / 黄 / 红）、MDR BLK LBL ALX 脚架（16 与 20 型号）、Zveno PK 缓冲管转接器与 TKPD 导轨防尘盖。
  - 这些物品作为**白名单新增项**时，插件只追加 id，不要求其存在，因此无论该模组是否安装都能正常加载。
  - 这些物品作为**冲突目标**时（HUXWRX 消音器），若其尚未注入，插件仅记 `Debug` 并跳过；由于 PPSh-41 防尘盖一侧的冲突记录仍会建立，互斥关系依旧成立，并避免了对第三方模组加载顺序的依赖。
  - Marlin MXLR 杠杆步枪由该模组注入，且本插件将其用作**槽位宿主**（区别于仅作白名单新增项的其它 WTT 物品）；若其未在本插件运行前注入，对应条目会记 `not found in Items` 警告并跳过。
- **原版物品**：RPD 520mm 枪管、M700 全部 4 种枪管、ORSIS T-5000M 660mm 枪管、SKS 与 OP-SKS 的 520mm 枪管、TAPCO Intrafuse SKS 枪身套件、Fab Defence UAS SKS 枪身套件及其枪托槽配件（TAPCO Intrafuse 缓冲管、Fab Defence UAS 折叠枪托）、SKS / OP-SKS 照门固定环、SKS / OP-SKS / SVT-40 / AVT-40 枪本体与 SVT-40 625mm 枪管、SVDS 照门固定环、SVDS 枪本体与 SVDS 枪管、PPSh-41 71 发弹鼓，以及 AKM PBS-1 7.62x39 消音器均为原版物品，不依赖 `WTT-ContentBackport`。
- **PPSh-41 上机匣槽**：`mod_reciever` 槽同时承载 PPSh-41 防尘盖、TAPCO Intrafuse SKS 与 Fab Defence UAS SKS 枪身套件，共用同一槽位，因此彼此天然二选一。
- **新增互斥**：两个 SKS 枪身套件与除 KS-23 金属枪托之外的全部 PPSh-41 枪托（含原装木制枪托与 Zveno 缓冲管转接器）互不兼容；两个 SKS 枪身套件与 PPSh-41 的 71 发弹鼓互不兼容；KS-23 金属枪托与两个 SKS 枪身套件枪托槽可装的配件互不兼容；TKPD 导轨防尘盖与 SKS / OP-SKS / SVT-40 / AVT-40 / SVDS 枪本体互不兼容；SVDS 照门固定环护木槽可装的莫辛 200mm、M700 全部 4 种与 ORSIS T-5000M 枪管与 SVDS 枪本体互不兼容；SVDS 枪管与自身互不兼容（防止重复安装）；SVDS 照门固定环与 PPSh-41 防尘盖、TAPCO Intrafuse SKS 枪身套件互不兼容；SVDS 照门固定环护木槽可装的 3 种原版 SVDS 护木与 PPSh-41 枪本体互不兼容；SVDS 照门固定环护木槽可装的 SVT-40 / AVT-40 625mm 枪管与 SVDS 枪本体互不兼容；TKPD 导轨防尘盖与自身互不兼容（防止重复安装）；MDR BLK LBL ALX 20 脚架与自身互不兼容（防止重复安装）；5 种 SKS 导气管防尘盖与 UltiMAK M1-B AK 导气管套件彼此之间（含与自身）互不兼容，使一把武器上最多只能装一个。
- **新增兼容**：Fab Defence UAS SKS 枪身套件的下导轨（`mod_tactical_002`）新增兼容 AK-100 系列聚合物护木可安装的全部 33 种前握把（含 Zenit RK 系列、BCM MOD.3、KAC、ASh-12、Tactical Dynamics 镂空、TangoDown Stubby BGV-MK46K 三色、Fortis Shift 等）。
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
- **`WTT-ContentBackport`**: some items are injected by that mod, such as the HUXWRX HX-QD 7.62x51 suppressors (black / tan), the Tyrant Designs MOD Chevron AR-15 skeletonized pistol grips (black / yellow / red), the MDR BLK LBL ALX bipods (16 and 20 models), the Zveno PK buffer tube adapter and the TKPD railed dust cover.
  - When these items are **whitelist additions**, the plugin only appends their ids and does not require them to exist, so it loads fine whether or not that mod is installed.
  - When these items are **conflict targets** (the HUXWRX suppressors), if they are not yet injected the plugin only logs `Debug` and skips; because the PPSh-41 dust cover side still records the conflict, the exclusion still holds, avoiding any dependency on third-party load order.
  - The Marlin MXLR lever-action rifle is injected by that mod, and this plugin uses it as a **slot owner** (unlike other WTT items that are only whitelist additions); if it is not injected before this plugin runs, the affected entry logs a `not found in Items` warning and is skipped.
- **Vanilla items**: the RPD 520 mm barrel, all four M700 barrels, the ORSIS T-5000M 660 mm barrel, the SKS and OP-SKS 520 mm barrels, the TAPCO Intrafuse SKS chassis kit, the Fab Defence UAS SKS chassis kit and their stock-slot parts (TAPCO Intrafuse buffer tube, Fab Defence UAS folding stock), the SKS / OP-SKS rear sight blocks, the SKS / OP-SKS / SVT-40 / AVT-40 weapons and the SVT-40 625 mm barrel, the SVDS rear sight block, the SVDS weapon and the SVDS barrel, the PPSh-41 71-round drum magazine, and the AKM PBS-1 7.62x39 suppressor are all vanilla items and do not depend on `WTT-ContentBackport`.
- **PPSh-41 upper receiver slot**: the `mod_reciever` slot carries the PPSh-41 dust cover, the TAPCO Intrafuse SKS and the Fab Defence UAS SKS chassis kits; since they share the same slot, they are inherently an either/or choice.
- **New exclusions**: the two SKS chassis kits are incompatible with every PPSh-41 stock except the KS-23 metal stock (including the PPSh wooden stock and the Zveno buffer tube adapter) and with the PPSh-41 71-round drum magazine; the KS-23 metal stock is incompatible with the parts accepted by the two SKS chassis kits' stock slots; the TKPD railed dust cover is incompatible with the SKS, OP-SKS, SVT-40, AVT-40 and SVDS weapons; the Mosin 200 mm, all four M700 and the ORSIS T-5000M barrels accepted by the SVDS rear sight block's handguard slot are incompatible with the SVDS weapon; the SVDS barrel is incompatible with itself (to prevent duplicate mounting); the SVDS rear sight block is incompatible with the PPSh-41 dust cover and with the TAPCO Intrafuse SKS chassis kit; the 3 vanilla SVDS handguards accepted by the SVDS rear sight block's handguard slot are incompatible with the PPSh-41 weapon itself; the SVT-40 / AVT-40 625 mm barrel accepted by that handguard slot is incompatible with the SVDS weapon; the TKPD railed dust cover is incompatible with itself (to prevent duplicate mounting); the MDR BLK LBL ALX 20 bipod is incompatible with itself (to prevent duplicate mounting); the 5 SKS gas-tube covers and the UltiMAK M1-B AK gas tube kit are mutually incompatible with each other (including with themselves), so at most one can be installed on a weapon.
- **New compatibility**: the Fab Defence UAS SKS chassis kit's lower rail (`mod_tactical_002`) now accepts all 33 foregrips compatible with the AK-100 series polymer handguards (Zenit RK series, BCM MOD.3, KAC, ASh-12, Tactical Dynamics skeletonized, TangoDown Stubby BGV-MK46K in three colors, Fortis Shift, and 25 others).
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
