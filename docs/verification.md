# 实机验证

## 启动自检

服务端启动后，日志（`user/logs/spt/sptYYYYMMDD.log`）应出现本模组的加载记录：

```
[Info][ModValidator] 模组：PJ568's Weapon Cross Modding 版本… 已加载
```

随后是各条注入记录：

```
[Info][...] WeaponCrossModding: added '<物品 id>' to slot '<槽位名>' (<宿主物品名>)
```

槽位名应涵盖 `mod_scope`、`mod_sight_front`、`mod_sight_rear`、`mod_barrel`、`mod_stock`、`mod_tactical`、`mod_tactical_000`、`mod_tactical_001`、`mod_pistol_grip`、`mod_mount`、`mod_muzzle`、`mod_reciever`；互斥关系另会输出 `added conflicting item '…' to '…'`。

## 验证步骤

1. 部署 dll 并重启 `pj568-spt-server`。
2. 打开 `user/logs/spt/sptYYYYMMDD.log`，确认出现上述加载行与注入行。
3. 进入游戏，在武器改装界面检查目标组合是否可安装，例如：
   - 在 UZI StormWerkz 顶盖导轨上安装 ELCAN SpecterDR 或 SIG Sauer BRAVO4；
   - 在 CR 200DS / CR 50DS 前准星槽安装 StormWerkz 顶盖导轨或 MP-18 瞄具基座；
   - 在 PPSh-41 上安装莫辛纳甘、M700、ORSIS T-5000M、SKS / OP-SKS 520mm 或 SVT-40 625mm 枪管，安装 M14ALCS (MOD-0) 枪托，或安装 Zveno PK 缓冲管转接器；
   - 在 PPSh-41 的机匣槽安装 TAPCO Intrafuse SKS 枪身套件或 Fab Defence UAS SKS 枪身套件；
   - 在 SKS / OP-SKS 照门固定环或 SVT-40 625mm 枪管的照门槽安装 TKPD 导轨防尘盖；
   - 在 RPD 520mm 枪管的枪口装置槽安装 AKM PBS-1 7.62x39 消音器；
   - 在 730mm 标准莫辛枪管或 SVT-40 标准枪口装置的前准星槽安装 MDR BLK LBL ALX 脚架（16 或 20 型号）；
   - 在 AA-12 457mm 枪管的导轨槽安装 M60 脚架；
   - 在 MP-18 或 Marlin MXLR 的枪托槽安装 KS-23 金属枪托；
   - 尝试同时安装 PPSh-41 防尘盖与 HUXWRX HX-QD 消音器，确认二者互斥；
   - 尝试在 PPSh-41 上同时安装 SKS 枪身套件与任一非 KS-23 枪托（含 Zveno 缓冲管转接器），确认二者互斥；
   - 尝试在 PPSh-41 上同时安装 SKS 枪身套件与 71 发弹鼓，确认二者互斥；
   - 尝试在 PPSh-41 上同时安装 KS-23 金属枪托与 TAPCO Intrafuse 缓冲管 / Fab Defence UAS 折叠枪托，确认二者互斥；
   - 尝试同时安装 TKPD 导轨防尘盖与 SKS / OP-SKS / SVT-40 / AVT-40 枪本体，确认二者互斥。
4. 若修改了物品 id，重新运行单元测试确认夹具仍与代码一致。

## 排查建议

- 所有运行日志都写入 `user/logs/spt/sptYYYYMMDD.log`，遇到异常先看该文件。
- **看不到注入记录**：确认 dll 位于 `user/mods/WeaponCrossModding/`，且日志中没有本模组的加载错误。
- **出现 `not found in Items` 警告**：该宿主物品 id 在当前数据库不存在，对应条目被跳过；多为数据版本或第三方模组差异所致。
- **日志只有 `Debug`**：说明该 id 已存在（幂等生效），属正常情况。
- **互斥未生效**：若 HUXWRX 消音器由第三方模组注入而当前缺失，插件会静默跳过消音器一侧，但防尘盖一侧的冲突记录仍会建立；确认防尘盖的 `ConflictingItems` 已包含消音器 id。

===============================================================

# In-Game Verification

## Startup Self-Check

After the server starts, the log (`user/logs/spt/sptYYYYMMDD.log`) should show this mod's load record:

```
[Info][ModValidator] 模组：PJ568's Weapon Cross Modding 版本… 已加载
```

followed by the individual injections:

```
[Info][...] WeaponCrossModding: added '<item id>' to slot '<slot name>' (<owner item name>)
```

The slot names should cover `mod_scope`, `mod_sight_front`, `mod_sight_rear`, `mod_barrel`, `mod_stock`, `mod_tactical`, `mod_tactical_000`, `mod_tactical_001`, `mod_pistol_grip`, `mod_mount`, `mod_muzzle`, `mod_reciever`; conflicts also print `added conflicting item '…' to '…'`.

## Verification Steps

1. Deploy the dll and restart `pj568-spt-server`.
2. Open `user/logs/spt/sptYYYYMMDD.log` and confirm the load line and injection lines above.
3. Enter the game and check the target combinations in the weapon modding screen, for example:
   - mount the ELCAN SpecterDR or SIG Sauer BRAVO4 on the UZI StormWerkz top cover rail;
   - mount the StormWerkz top cover rail or MP-18 scope base on the CR 200DS / CR 50DS front sight slot;
   - mount a Mosin, M700, ORSIS T-5000M, SKS / OP-SKS 520 mm or SVT-40 625 mm barrel, mount the M14ALCS (MOD-0) buttstock, or mount the Zveno PK buffer tube adapter, on the PPSh-41;
   - mount a TAPCO Intrafuse SKS chassis kit or a Fab Defence UAS SKS chassis kit in the PPSh-41 receiver slot;
   - mount the TKPD railed dust cover on the sight slot of the SKS / OP-SKS rear sight blocks or the SVT-40 625 mm barrel;
   - mount an AKM PBS-1 7.62x39 suppressor on the muzzle device slot of the RPD 520 mm barrel;
   - mount an MDR BLK LBL ALX bipod (16 or 20 model) on the front sight slot of the 730 mm standard Mosin barrel or the SVT-40 standard muzzle;
   - mount an M60 bipod on the rail slot of the AA-12 457 mm barrel;
   - mount a KS-23 metal stock on the stock slot of the MP-18 or Marlin MXLR;
   - try mounting the PPSh-41 dust cover and a HUXWRX HX-QD suppressor together and confirm they are mutually exclusive;
   - try mounting an SKS chassis kit on the PPSh-41 together with any non-KS-23 stock (including the Zveno buffer tube adapter) and confirm they are mutually exclusive;
   - try mounting an SKS chassis kit on the PPSh-41 together with the 71-round drum magazine and confirm they are mutually exclusive;
   - try mounting the KS-23 metal stock together with the TAPCO Intrafuse buffer tube / Fab Defence UAS folding stock and confirm they are mutually exclusive;
   - try mounting the TKPD railed dust cover together with the SKS / OP-SKS / SVT-40 / AVT-40 weapons and confirm they are mutually exclusive.
4. If you changed any item id, re-run the unit tests to confirm the fixtures still match the code.

## Troubleshooting

- All runtime logs are written to `user/logs/spt/sptYYYYMMDD.log`; check that file first when something looks wrong.
- **No injection records**: confirm the dll is at `user/mods/WeaponCrossModding/` and that the log contains no load error for this mod.
- **A `not found in Items` warning appears**: the owner item id is absent from the current database and that entry was skipped; this is usually a data-version or third-party-mod difference.
- **Only `Debug` entries appear**: the id already exists (idempotency in effect), which is normal.
- **A conflict does not apply**: if the HUXWRX suppressor is injected by a third-party mod and is currently absent, the plugin silently skips the suppressor side, but the dust cover side still records the conflict; verify the dust cover's `ConflictingItems` contains the suppressor id.
