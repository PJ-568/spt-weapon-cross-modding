# 功能与互斥

> 本文汇总 PJ568's Weapon Cross Modding 实现的全部跨武器兼容条目与互斥关系。所有条目均通过向目标槽位过滤器白名单追加物品 id 实现（互斥则写入冲突列表），详见[原理](principle.md)。

## 瞄具与基座

- UZI StormWerkz 顶盖导轨的 `mod_scope` 槽：新增 ELCAN SpecterDR 1x/4x（含 FDE 变体）与 SIG Sauer BRAVO4 4x30 瞄准镜。
- CR 200DS、CR 50DS 的 `mod_sight_front` 前准星槽：新增 UZI StormWerkz 顶盖导轨与 MP-18 瞄具基座。
- Aim Sports“三轨”莫辛步枪导轨的 `mod_scope` 槽：新增 M14 SAGE International DCSB 瞄具基座。
- SKS / OP-SKS 照门固定环的 `mod_sight_rear` 照门槽：新增 TKPD 导轨防尘盖。
- SVT-40 625mm 枪管的 `mod_sight_rear` 照门槽：新增 TKPD 导轨防尘盖与 5 种 SKS 导气管防尘盖（OP-SKS 标准、SKS 木制标准、TAPCO、Fab Defence UAS、ATI Monte Carlo；即原生 SKS 导气箍可装的那 5 种）。
- SVDS 照门固定环的 `mod_sight_rear` 照门槽：新增 TKPD 导轨防尘盖、3 种 SKS 导气管防尘盖（OP-SKS 标准、SKS 木制标准、ATI Monte Carlo）与 UltiMAK M1-B AK 导气管套件。
- Alpha Dog Alpha 9 9x19 声音抑制器的 `mod_scope` 槽：新增 M14 SAGE International DCSB 瞄具基座。

## 枪管与枪托

- PPSh-41 冲锋枪的 `mod_barrel` 枪管槽：新增莫辛纳甘 220mm 锯短螺纹 / 514mm 卡宾 / 730mm 标准枪管、SKS / OP-SKS 的 520mm 枪管、SVT-40 / AVT-40 共用的 625mm 枪管与 SVDS 照门固定环。（莫辛 200mm、M700 全部 4 种与 ORSIS T-5000M 枪管已移至 SVDS 照门固定环的护木槽。）
- SVDS 照门固定环的 `mod_handguard` 护木槽：新增莫辛 200mm 锯短枪管、M700 全部 4 种枪管（26 英寸、20 英寸螺纹、26 英寸不锈钢、20 英寸不锈钢螺纹）、ORSIS T-5000M 660mm 枪管、SVDS 枪管、SVT-40 / AVT-40 共用的 625mm 枪管与 SVDS 照门固定环自身。
- PPSh-41 冲锋枪的 `mod_stock` 枪托槽：新增 Benelli M3 可伸缩枪托、PKM 木制枪托、Zenit PT-2 "Klassika" PK 机枪枪托、PKP 聚合物枪托、Ultima MP-155 塑料手枪式握把、KS-23 金属枪托、M14 SAGE International M14ALCS (MOD-0) 枪托与 Zveno PK 缓冲管转接器（由 `WTT-ContentBackport` 注入）。
- 730mm 标准莫辛枪管的 `mod_sight_front` 前准星槽：新增 MDR BLK LBL ALX 脚架（16 与 20 型号，由 `WTT-ContentBackport` 注入；与莫辛准星共用同一槽位，二者互斥）。
- SVT-40 标准枪口装置的 `mod_sight_front` 前准星槽：新增 MDR BLK LBL ALX 脚架（16 与 20 型号，由 `WTT-ContentBackport` 注入；与原准星共用同一槽位，二者互斥）。
- RPD 520mm 枪管的 `mod_muzzle` 枪口装置槽：新增 AKM PBS-1 7.62x39 消音器。
- AA-12 457mm 枪管的 `mod_mount` 导轨槽：新增 M60 脚架。
- MP-18 与 Marlin MXLR 的 `mod_stock` 枪托槽：新增 KS-23 金属枪托。
- MTs-255-12 12 号左轮霰弹枪的 `mod_handguard` 护木槽：新增 MP-18 的全部护木（木制、塑料）；其 `mod_stock` 枪托槽：新增雷明顿 Model 870 的 Magpul SGA、Remington SPS 与 Shockwave Raptor 枪托（不含 Mesa Tactical LEO 转接器与 Fab Defence AGR 握把）。

## 弹药与口径

- 20x1mm 玩具枪（`weapon_ussr_pd_20x1mm`）的膛室（`Chambers`）：在保留原 20x1mm 玩具弹能力的同时，新增对全部 7 种 7.62x25 托卡列夫弹药（AKBS、FMJ43、LRN、LRNPC、P Gl、Pst gzh、T Gzh）的兼容。做法是向武器膛室（`Chambers`）白名单追加弹药 id，不改动武器单值 `ammoCaliber`；其原装弹匣不再直接支持 7.62x25。
- Marlin MXLR .308 ME 杠杆步枪的 5 发管状弹仓（`mag_m1895_marlin_mxlr_784x49_5`）：新增对全部 7 种 7.62x25 托卡列夫弹药的兼容。
- 20x1mm 玩具枪（`weapon_ussr_pd_20x1mm`）的 `mod_magazine` 槽：新增 Marlin MXLR 的 5 发管状弹仓（`mag_m1895_marlin_mxlr_784x49_5`）。

## 握把与配件

- Aim Sports“三轨”的第一个战术配件槽（`mod_tactical_000`）：新增 13 款前握把（Zenit RK 系列、镂空型、垂直型、BCM MOD.3、TangoDown Stubby BGV-MK46K；不含 KeyMod / M-LOK 型）、SV-98 隔热带与 Fortis Shift 战术前握把。
- Aim Sports“三轨”的第二个战术配件槽（`mod_tactical_001`）：新增 SV-98 隔热带。
- UZI StormWerkz 护木底轨的 `mod_tactical` 槽：新增 Zenit RK 系列前握把。
- Fab Defence UAS SKS 枪身套件的下导轨（`mod_tactical_002`）：新增 MDR BLK LBL ALX 20 脚架（由 `WTT-ContentBackport` 注入）与 AK-100 系列聚合物护木兼容的全部前握把（含 Zenit RK 系列、BCM MOD.3、KAC、ASh-12、Tactical Dynamics 镂空、TangoDown Stubby BGV-MK46K 三色、Fortis Shift 等 33 种）。
- UltiMAK M1-B AK 导气管套件的战术配件槽（`mod_tactical_000`）：新增 MDR BLK LBL ALX 20 脚架（由 `WTT-ContentBackport` 注入）。
- CR 50DS 的 `mod_tactical` 战术设备槽：新增 Zenit RK 系列前握把、KAC MWS 脚架转接器与 BT10 V8 Atlas 折叠脚架。
- M14 SAGE International M14ALCS (MOD-0) 枪托的 `mod_pistol_grip` 握把位：新增 AR-15 Tactical Dynamics 镂空手枪式握把、Tyrant Designs MOD Chevron AR-15 镂空手枪式握把（黑 / 黄 / 红）与 AS VAL Rotor 43 手枪式握把附缓冲管转接器。
- PPSh-41 冲锋枪的 `mod_reciever` 上机匣槽：新增 TAPCO Intrafuse SKS 枪身套件与 Fab Defence UAS SKS 枪身套件（二者与 PPSh-41 防尘盖共用同一槽位，天然互斥）。

## 互斥

- PPSh-41 防尘盖与 HUXWRX HX-QD 7.62x51 消音器（含黄褐色变体）互不兼容。
- PPSh-41 机匣槽的两个 SKS 枪身套件（TAPCO Intrafuse、Fab Defence UAS）与除 KS-23 金属枪托之外的全部 PPSh-41 枪托（原装 PPSh 木制枪托、Benelli M3 可伸缩枪托、PKM 木制枪托、Zenit PT-2、PKP 聚合物枪托、Ultima MP-155 握把、M14ALCS (MOD-0) 枪托、Zveno PK 缓冲管转接器）互不兼容。
- 两个 SKS 枪身套件与 PPSh-41 的 71 发弹鼓互不兼容。
- KS-23 金属枪托与两个 SKS 枪身套件枪托槽可装的配件（TAPCO Intrafuse 缓冲管、Fab Defence UAS 折叠枪托）互不兼容。
- TKPD 导轨防尘盖与 SKS、OP-SKS、SVT-40、AVT-40、SVDS 枪本体互不兼容。
- SVDS 照门固定环护木槽可装的莫辛 200mm、M700 全部 4 种与 ORSIS T-5000M 枪管与 SVDS 枪本体互不兼容。
- SVDS 枪管与自身互不兼容（防止重复安装）。
- TKPD 导轨防尘盖与自身互不兼容（防止重复安装）。
- SVDS 照门固定环护木槽可装的 SVT-40 / AVT-40 625mm 枪管与 SVDS 枪本体互不兼容。
- MDR BLK LBL ALX 20 脚架与自身互不兼容（防止重复安装）。
- 5 种 SKS 导气管防尘盖与 UltiMAK M1-B AK 导气管套件彼此之间（含与自身）互不兼容，使一把武器上最多只能装一个。
- SVDS 照门固定环与 PPSh-41 防尘盖互不兼容。
- SVDS 照门固定环与 TAPCO Intrafuse SKS 枪身套件互不兼容。
- SVDS 照门固定环护木槽可装的 3 种原版 SVDS 护木（CAA XRS DRG、Izhmash 现代化套件、SVDS 标准护木）与 PPSh-41 枪本体互不兼容。

===============================================================

# Features and Conflicts

> This document collects every cross-weapon compatibility entry and conflict implemented by PJ568's Weapon Cross Modding. All entries add item ids to the target slot filter whitelist (conflicts are written to the conflict list); see [Principles](principle.md) for details.

## Optics and mounts

- `mod_scope` slot of the UZI StormWerkz top cover rail: adds the ELCAN SpecterDR 1x/4x (including the FDE variant) and the SIG Sauer BRAVO4 4x30.
- `mod_sight_front` slot of the CR 200DS and CR 50DS: adds the UZI StormWerkz top cover rail and the MP-18 scope base.
- `mod_scope` slot of the Aim Sports tri-rail Mosin mount: adds the M14 SAGE International DCSB scope mount.
- `mod_sight_rear` sight slot of the SKS / OP-SKS rear sight blocks: adds the TKPD railed dust cover.
- `mod_sight_rear` sight slot of the SVT-40 625 mm barrel: adds the TKPD railed dust cover and the 5 SKS gas-tube covers (OP-SKS standard, SKS wooden standard, TAPCO, Fab Defence UAS, ATI Monte Carlo; the five accepted by the native SKS gas block).
- `mod_sight_rear` sight slot of the SVDS rear sight block: adds the TKPD railed dust cover, the 3 SKS gas-tube covers (OP-SKS standard, SKS wooden standard, ATI Monte Carlo) and the UltiMAK M1-B AK gas tube kit.
- `mod_scope` slot of the Alpha Dog Alpha 9 9x19 sound suppressor: adds the M14 SAGE International DCSB scope mount.

## Barrels and stocks

- `mod_barrel` slot of the PPSh-41: adds the 220 mm threaded sawn-off, 514 mm carbine and 730 mm standard Mosin barrels, the SKS / OP-SKS 520 mm barrels, the 625 mm barrel shared by the SVT-40 / AVT-40 and the SVDS rear sight block. (The Mosin 200 mm barrel, all four M700 barrels and the ORSIS T-5000M barrel have been moved to the SVDS rear sight block's handguard slot.)
- `mod_handguard` slot of the SVDS rear sight block: adds the Mosin 200 mm sawn-off barrel, all four M700 barrels (26-inch, 20-inch threaded, 26-inch stainless, 20-inch stainless threaded), the ORSIS T-5000M 660 mm barrel, the SVDS barrel, the 625 mm barrel shared by the SVT-40 / AVT-40 and the SVDS rear sight block itself.
- `mod_stock` slot of the PPSh-41: adds the Benelli M3 telescopic stock, the PKM wooden stock, the Zenit PT-2 "Klassika" PK stock, the PKP polymer stock, the Ultima MP-155 plastic pistol grip, the KS-23 metal stock, the M14 SAGE International M14ALCS (MOD-0) buttstock and the Zveno PK buffer tube adapter (injected by `WTT-ContentBackport`).
- `mod_sight_front` slot of the 730 mm standard Mosin barrel: adds the MDR BLK LBL ALX bipods (16 and 20 models, injected by `WTT-ContentBackport`; shared with the Mosin front sight, so the two are mutually exclusive).
- `mod_sight_front` slot of the SVT-40 standard muzzle: adds the MDR BLK LBL ALX bipods (16 and 20 models, injected by `WTT-ContentBackport`; shared with the stock front sight, so the two are mutually exclusive).
- `mod_muzzle` muzzle device slot of the RPD 520 mm barrel: adds the AKM PBS-1 7.62x39 suppressor.
- `mod_mount` rail slot of the AA-12 457 mm barrel: adds the M60 bipod.
- `mod_stock` slot of the MP-18 and Marlin MXLR: adds the KS-23 metal stock.
- `mod_handguard` slot of the MTs-255-12 12ga revolver shotgun: adds all MP-18 handguards (wooden, plastic); its `mod_stock` slot: adds the Remington Model 870's Magpul SGA, Remington SPS and Shockwave Raptor stocks (excluding the Mesa Tactical LEO stock adapter and the Fab Defence AGR pistol grip).

## Ammunition and caliber

- Chamber (`Chambers`) of the 20x1mm toy gun (`weapon_ussr_pd_20x1mm`): while keeping the original 20x1mm toy-round capability, adds compatibility with all 7 types of 7.62x25 Tokarev ammunition (AKBS, FMJ43, LRN, LRNPC, P Gl, Pst gzh, T Gzh). It appends the ammo ids to the chamber whitelist without touching the weapon's single-value `ammoCaliber`; its stock magazine no longer accepts 7.62x25 directly.
- 5-round tubular magazine of the Marlin MXLR .308 ME lever-action rifle (`mag_m1895_marlin_mxlr_784x49_5`): adds compatibility with all 7 types of 7.62x25 Tokarev ammunition.
- `mod_magazine` slot of the 20x1mm toy gun (`weapon_ussr_pd_20x1mm`): adds the Marlin MXLR 5-round tubular magazine.

## Grips and accessories

- First tactical slot (`mod_tactical_000`) of the Aim Sports tri-rail: adds 13 foregrips (Zenit RK series, skeletonized, vertical, BCM MOD.3, TangoDown Stubby BGV-MK46K; excluding KeyMod / M-LOK types), the SV-98 heat ribbon and the Fortis Shift tactical foregrip.
- Second tactical slot (`mod_tactical_001`) of the Aim Sports tri-rail: adds the SV-98 heat ribbon.
- `mod_tactical` slot of the UZI StormWerkz lower handguard rail: adds the Zenit RK series foregrips.
- Lower rail (`mod_tactical_002`) of the Fab Defence UAS SKS chassis kit: adds the MDR BLK LBL ALX 20 bipod (injected by `WTT-ContentBackport`) and all foregrips compatible with the AK-100 series polymer handguards (Zenit RK series, BCM MOD.3, KAC, ASh-12, Tactical Dynamics skeletonized, TangoDown Stubby BGV-MK46K in three colors, Fortis Shift, and 25 others, 33 total).
- Tactical slot (`mod_tactical_000`) of the UltiMAK M1-B AK gas tube kit: adds the MDR BLK LBL ALX 20 bipod (injected by `WTT-ContentBackport`).
- `mod_tactical` slot of the CR 50DS: adds the Zenit RK series foregrips, the KAC MWS bipod adapter and the BT10 V8 Atlas folding bipod.
- `mod_pistol_grip` slot of the M14 SAGE International M14ALCS (MOD-0) buttstock: adds the AR-15 Tactical Dynamics skeletonized pistol grip, the Tyrant Designs MOD Chevron AR-15 skeletonized pistol grips (black / yellow / red) and the AS VAL Rotor 43 pistol grip with buffer tube adapter.
- `mod_reciever` upper receiver slot of the PPSh-41: adds the TAPCO Intrafuse SKS chassis kit and the Fab Defence UAS SKS chassis kit (both share the same slot that carries the PPSh-41 dust cover, so they are inherently mutually exclusive with it).

## Conflicts

- The PPSh-41 dust cover is incompatible with the HUXWRX HX-QD 7.62x51 suppressors (including the tan variant).
- The two SKS chassis kits in the PPSh-41 receiver slot (TAPCO Intrafuse, Fab Defence UAS) are incompatible with every PPSh-41 stock except the KS-23 metal stock (PPSh wooden stock, Benelli M3 telescopic stock, PKM wooden stock, Zenit PT-2, PKP polymer stock, Ultima MP-155 pistol grip, M14ALCS (MOD-0) buttstock, Zveno PK buffer tube adapter).
- The two SKS chassis kits are incompatible with the PPSh-41 71-round drum magazine.
- The KS-23 metal stock is incompatible with the parts accepted by the two SKS chassis kits' stock slots (TAPCO Intrafuse buffer tube, Fab Defence UAS folding stock).
- The TKPD railed dust cover is incompatible with the SKS, OP-SKS, SVT-40, AVT-40 and SVDS weapons themselves.
- The Mosin 200 mm, all four M700 and the ORSIS T-5000M barrels accepted by the SVDS rear sight block's handguard slot are incompatible with the SVDS weapon itself.
- The SVDS barrel is incompatible with itself (to prevent duplicate mounting).
- The TKPD railed dust cover is incompatible with itself (to prevent duplicate mounting).
- The SVT-40 / AVT-40 625 mm barrel accepted by the SVDS rear sight block's handguard slot is incompatible with the SVDS weapon itself.
- The MDR BLK LBL ALX 20 bipod is incompatible with itself (to prevent duplicate mounting).
- The 5 SKS gas-tube covers and the UltiMAK M1-B AK gas tube kit are mutually incompatible with each other (including with themselves), so at most one can be installed on a weapon.
- The SVDS rear sight block is incompatible with the PPSh-41 dust cover.
- The SVDS rear sight block is incompatible with the TAPCO Intrafuse SKS chassis kit.
- The 3 vanilla SVDS handguards accepted by the SVDS rear sight block's handguard slot (CAA XRS DRG, Izhmash modernized kit, SVDS standard) are incompatible with the PPSh-41 weapon itself.
