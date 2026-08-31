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

槽位名应涵盖 `mod_scope`、`mod_sight_front`、`mod_barrel`、`mod_stock`、`mod_tactical`、`mod_tactical_000`、`mod_tactical_001`、`mod_pistol_grip`；互斥关系另会输出 `added conflicting item '…' to '…'`。

## 验证步骤

1. 部署 dll 并重启 `pj568-spt-server`。
2. 打开 `user/logs/spt/sptYYYYMMDD.log`，确认出现上述加载行与注入行。
3. 进入游戏，在武器改装界面检查目标组合是否可安装，例如：
   - 在 UZI StormWerkz 顶盖导轨上安装 ELCAN SpecterDR 或 SIG Sauer BRAVO4；
   - 在 CR 200DS / CR 50DS 前准星槽安装 StormWerkz 顶盖导轨或 MP-18 瞄具基座；
   - 在 PPSh-41 上安装莫辛纳甘枪管或 M14ALCS (MOD-0) 枪托；
   - 尝试同时安装 PPSh-41 防尘盖与 HUXWRX HX-QD 消音器，确认二者互斥。
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

The slot names should cover `mod_scope`, `mod_sight_front`, `mod_barrel`, `mod_stock`, `mod_tactical`, `mod_tactical_000`, `mod_tactical_001`, `mod_pistol_grip`; conflicts also print `added conflicting item '…' to '…'`.

## Verification Steps

1. Deploy the dll and restart `pj568-spt-server`.
2. Open `user/logs/spt/sptYYYYMMDD.log` and confirm the load line and injection lines above.
3. Enter the game and check the target combinations in the weapon modding screen, for example:
   - mount the ELCAN SpecterDR or SIG Sauer BRAVO4 on the UZI StormWerkz top cover rail;
   - mount the StormWerkz top cover rail or MP-18 scope base on the CR 200DS / CR 50DS front sight slot;
   - mount a Mosin barrel or the M14ALCS (MOD-0) buttstock on the PPSh-41;
   - try mounting the PPSh-41 dust cover and a HUXWRX HX-QD suppressor together and confirm they are mutually exclusive.
4. If you changed any item id, re-run the unit tests to confirm the fixtures still match the code.

## Troubleshooting

- All runtime logs are written to `user/logs/spt/sptYYYYMMDD.log`; check that file first when something looks wrong.
- **No injection records**: confirm the dll is at `user/mods/WeaponCrossModding/` and that the log contains no load error for this mod.
- **A `not found in Items` warning appears**: the owner item id is absent from the current database and that entry was skipped; this is usually a data-version or third-party-mod difference.
- **Only `Debug` entries appear**: the id already exists (idempotency in effect), which is normal.
- **A conflict does not apply**: if the HUXWRX suppressor is injected by a third-party mod and is currently absent, the plugin silently skips the suppressor side, but the dust cover side still records the conflict; verify the dust cover's `ConflictingItems` contains the suppressor id.
