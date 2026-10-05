# 功能与互斥

> 本文汇总 PJ568's Weapon Cross Modding 实现的全部跨武器兼容条目与互斥关系。所有条目均通过向目标槽位过滤器白名单追加物品 id 实现（互斥则写入冲突列表），详见[原理](principle.md)。

## 瞄具与基座

- UZI StormWerkz 顶盖导轨的 `mod_scope` 槽：新增 ELCAN SpecterDR 1x/4x（含 FDE 变体）与 SIG Sauer BRAVO4 4x30 瞄准镜。
- CR 200DS、CR 50DS 的 `mod_sight_front` 前准星槽：新增 UZI StormWerkz 顶盖导轨与 MP-18 瞄具基座。
- Aim Sports“三轨”莫辛步枪导轨的 `mod_scope` 槽：新增 M14 SAGE International DCSB 瞄具基座。
- SKS / OP-SKS 照门固定环的 `mod_sight_rear` 照门槽：新增 TKPD 导轨防尘盖。
- SVT-40 625mm 枪管的 `mod_sight_rear` 照门槽：新增 TKPD 导轨防尘盖。

## 枪管与枪托

- PPSh-41 冲锋枪的 `mod_barrel` 枪管槽：新增莫辛纳甘全部 4 种尺寸枪管（200mm 锯短、220mm 锯短螺纹、514mm 卡宾、730mm 标准）、M700 全部 4 种枪管（26 英寸、20 英寸螺纹、26 英寸不锈钢、20 英寸不锈钢螺纹）、ORSIS T-5000M 660mm 枪管、SKS / OP-SKS 的 520mm 枪管与 SVT-40 / AVT-40 共用的 625mm 枪管。
- PPSh-41 冲锋枪的 `mod_stock` 枪托槽：新增 Benelli M3 可伸缩枪托、PKM 木制枪托、Zenit PT-2 "Klassika" PK 机枪枪托、PKP 聚合物枪托、Ultima MP-155 塑料手枪式握把、KS-23 金属枪托、M14 SAGE International M14ALCS (MOD-0) 枪托与 Zveno PK 缓冲管转接器（由 `WTT-ContentBackport` 注入）。
- 730mm 标准莫辛枪管的 `mod_sight_front` 前准星槽：新增 MDR BLK LBL ALX 脚架（16 与 20 型号，由 `WTT-ContentBackport` 注入；与莫辛准星共用同一槽位，二者互斥）。
- SVT-40 标准枪口装置的 `mod_sight_front` 前准星槽：新增 MDR BLK LBL ALX 脚架（16 与 20 型号，由 `WTT-ContentBackport` 注入；与原准星共用同一槽位，二者互斥）。
- RPD 520mm 枪管的 `mod_muzzle` 枪口装置槽：新增 AKM PBS-1 7.62x39 消音器。
- AA-12 457mm 枪管的 `mod_mount` 导轨槽：新增 M60 脚架。
- MP-18 与 Marlin MXLR 的 `mod_stock` 枪托槽：新增 KS-23 金属枪托。

## 握把与配件

- Aim Sports“三轨”的第一个战术配件槽（`mod_tactical_000`）：新增 13 款前握把（Zenit RK 系列、镂空型、垂直型、BCM MOD.3、TangoDown Stubby BGV-MK46K；不含 KeyMod / M-LOK 型）、SV-98 隔热带与 Fortis Shift 战术前握把。
- Aim Sports“三轨”的第二个战术配件槽（`mod_tactical_001`）：新增 SV-98 隔热带。
- UZI StormWerkz 护木底轨的 `mod_tactical` 槽：新增 Zenit RK 系列前握把。
- CR 50DS 的 `mod_tactical` 战术设备槽：新增 Zenit RK 系列前握把、KAC MWS 脚架转接器与 BT10 V8 Atlas 折叠脚架。
- M14 SAGE International M14ALCS (MOD-0) 枪托的 `mod_pistol_grip` 握把位：新增 AR-15 Tactical Dynamics 镂空手枪式握把、Tyrant Designs MOD Chevron AR-15 镂空手枪式握把（黑 / 黄 / 红）与 AS VAL Rotor 43 手枪式握把附缓冲管转接器。
- PPSh-41 冲锋枪的 `mod_reciever` 上机匣槽：新增 TAPCO Intrafuse SKS 枪身套件与 Fab Defence UAS SKS 枪身套件（二者与 PPSh-41 防尘盖共用同一槽位，天然互斥）。

## 互斥

- PPSh-41 防尘盖与 HUXWRX HX-QD 7.62x51 消音器（含黄褐色变体）互不兼容。
- PPSh-41 机匣槽的两个 SKS 枪身套件（TAPCO Intrafuse、Fab Defence UAS）与除 KS-23 金属枪托之外的全部 PPSh-41 枪托（原装 PPSh 木制枪托、Benelli M3 可伸缩枪托、PKM 木制枪托、Zenit PT-2、PKP 聚合物枪托、Ultima MP-155 握把、M14ALCS (MOD-0) 枪托、Zveno PK 缓冲管转接器）互不兼容。
- 两个 SKS 枪身套件与 PPSh-41 的 71 发弹鼓互不兼容。
- KS-23 金属枪托与两个 SKS 枪身套件枪托槽可装的配件（TAPCO Intrafuse 缓冲管、Fab Defence UAS 折叠枪托）互不兼容。
- TKPD 导轨防尘盖与 SKS、OP-SKS、SVT-40、AVT-40 枪本体互不兼容。

===============================================================

# Features and Conflicts

> This document collects every cross-weapon compatibility entry and conflict implemented by PJ568's Weapon Cross Modding. All entries add item ids to the target slot filter whitelist (conflicts are written to the conflict list); see [Principles](principle.md) for details.

## Optics and mounts

- `mod_scope` slot of the UZI StormWerkz top cover rail: adds the ELCAN SpecterDR 1x/4x (including the FDE variant) and the SIG Sauer BRAVO4 4x30.
- `mod_sight_front` slot of the CR 200DS and CR 50DS: adds the UZI StormWerkz top cover rail and the MP-18 scope base.
- `mod_scope` slot of the Aim Sports tri-rail Mosin mount: adds the M14 SAGE International DCSB scope mount.
- `mod_sight_rear` sight slot of the SKS / OP-SKS rear sight blocks: adds the TKPD railed dust cover.
- `mod_sight_rear` sight slot of the SVT-40 625 mm barrel: adds the TKPD railed dust cover.

## Barrels and stocks

- `mod_barrel` slot of the PPSh-41: adds all four Mosin barrel lengths (200 mm sawn-off, 220 mm threaded sawn-off, 514 mm carbine, 730 mm standard), all four M700 barrel lengths (26-inch, 20-inch threaded, 26-inch stainless, 20-inch stainless threaded), the ORSIS T-5000M 660 mm barrel, the SKS / OP-SKS 520 mm barrels and the 625 mm barrel shared by the SVT-40 / AVT-40.
- `mod_stock` slot of the PPSh-41: adds the Benelli M3 telescopic stock, the PKM wooden stock, the Zenit PT-2 "Klassika" PK stock, the PKP polymer stock, the Ultima MP-155 plastic pistol grip, the KS-23 metal stock, the M14 SAGE International M14ALCS (MOD-0) buttstock and the Zveno PK buffer tube adapter (injected by `WTT-ContentBackport`).
- `mod_sight_front` slot of the 730 mm standard Mosin barrel: adds the MDR BLK LBL ALX bipods (16 and 20 models, injected by `WTT-ContentBackport`; shared with the Mosin front sight, so the two are mutually exclusive).
- `mod_sight_front` slot of the SVT-40 standard muzzle: adds the MDR BLK LBL ALX bipods (16 and 20 models, injected by `WTT-ContentBackport`; shared with the stock front sight, so the two are mutually exclusive).
- `mod_muzzle` muzzle device slot of the RPD 520 mm barrel: adds the AKM PBS-1 7.62x39 suppressor.
- `mod_mount` rail slot of the AA-12 457 mm barrel: adds the M60 bipod.
- `mod_stock` slot of the MP-18 and Marlin MXLR: adds the KS-23 metal stock.

## Grips and accessories

- First tactical slot (`mod_tactical_000`) of the Aim Sports tri-rail: adds 13 foregrips (Zenit RK series, skeletonized, vertical, BCM MOD.3, TangoDown Stubby BGV-MK46K; excluding KeyMod / M-LOK types), the SV-98 heat ribbon and the Fortis Shift tactical foregrip.
- Second tactical slot (`mod_tactical_001`) of the Aim Sports tri-rail: adds the SV-98 heat ribbon.
- `mod_tactical` slot of the UZI StormWerkz lower handguard rail: adds the Zenit RK series foregrips.
- `mod_tactical` slot of the CR 50DS: adds the Zenit RK series foregrips, the KAC MWS bipod adapter and the BT10 V8 Atlas folding bipod.
- `mod_pistol_grip` slot of the M14 SAGE International M14ALCS (MOD-0) buttstock: adds the AR-15 Tactical Dynamics skeletonized pistol grip, the Tyrant Designs MOD Chevron AR-15 skeletonized pistol grips (black / yellow / red) and the AS VAL Rotor 43 pistol grip with buffer tube adapter.
- `mod_reciever` upper receiver slot of the PPSh-41: adds the TAPCO Intrafuse SKS chassis kit and the Fab Defence UAS SKS chassis kit (both share the same slot that carries the PPSh-41 dust cover, so they are inherently mutually exclusive with it).

## Conflicts

- The PPSh-41 dust cover is incompatible with the HUXWRX HX-QD 7.62x51 suppressors (including the tan variant).
- The two SKS chassis kits in the PPSh-41 receiver slot (TAPCO Intrafuse, Fab Defence UAS) are incompatible with every PPSh-41 stock except the KS-23 metal stock (PPSh wooden stock, Benelli M3 telescopic stock, PKM wooden stock, Zenit PT-2, PKP polymer stock, Ultima MP-155 pistol grip, M14ALCS (MOD-0) buttstock, Zveno PK buffer tube adapter).
- The two SKS chassis kits are incompatible with the PPSh-41 71-round drum magazine.
- The KS-23 metal stock is incompatible with the parts accepted by the two SKS chassis kits' stock slots (TAPCO Intrafuse buffer tube, Fab Defence UAS folding stock).
- The TKPD railed dust cover is incompatible with the SKS, OP-SKS, SVT-40 and AVT-40 weapons themselves.
