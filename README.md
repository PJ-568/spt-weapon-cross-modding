# PJ568's Weapon Cross Modding

> 一个 SPT 4.1.x 服务端插件：往目标物品槽的过滤器白名单里追加配件 id，让指定武器、瞄具、枪托与握把实现跨武器兼容。

本插件在服务端加载时，向目标物品槽的 `Slot.Properties.Filters[].Filter`（`HashSet<MongoId>`）白名单追加物品 id；
对互斥关系，则往 `TemplateItem.Properties.ConflictingItems` 追加物品 id。
所有注入均幂等：id 已存在时不重复添加。

## 效果

| UZI StormWerkz 顶盖导轨与护木底轨 | CR 200DS 前准星槽 |
| --- | --- |
| ![IWI UZI 9x19 冲锋枪](assets/UZI.webp) | ![Chiappa 犀牛 200DS 左轮手枪](assets/200DS.webp) |
| 顶盖导轨装 ELCAN SpecterDR；护木底轨装 RK 系列前握把。 | 前准星槽装 StormWerkz 顶盖导轨与 MP-18 瞄具基座。 |

| CR 50DS 前准星槽与战术设备槽 | PPSh-41 枪管与枪托槽 |
| --- | --- |
| ![Chiappa 犀牛 50DS 左轮手枪](assets/50DS.webp) | ![PPSh-41 冲锋枪](assets/PPSh.webp) |
| 前准星槽装瞄具基座；战术设备槽装 RK 系列前握把。 | 枪管槽装莫辛纳甘枪管并挂三轨；枪托槽装 M14ALCS (MOD-0) 枪托；730mm 莫辛枪管的准星槽可装 MDR BLK LBL ALX 脚架（16 / 20）。 |

## 文档

- [功能与互斥](docs/features.md)
- [原理](docs/principle.md)
- [构建、测试与发布](docs/development.md)
- [实机验证](docs/verification.md)
- [兼容性与限制](docs/compatibility.md)

## 支持版本

SPT `4.1.x`。

## 许可证

[MIT](LICENSE)

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

# PJ568's Weapon Cross Modding

> An SPT 4.1.x server plugin that appends accessory ids to slot filter whitelists, enabling cross-weapon compatibility between selected weapons, optics, stocks and grips.

On load, the plugin appends item ids to the `Slot.Properties.Filters[].Filter` (`HashSet<MongoId>`) whitelist of the target item slots;
for mutual exclusions, it appends item ids to `TemplateItem.Properties.ConflictingItems`.
All injections are idempotent: an id already present is never added twice.

## Effect

| UZI StormWerkz top cover rail and lower handguard rail | CR 200DS front sight slot |
| --- | --- |
| ![IWI UZI 9x19 SMG](assets/UZI.webp) | ![Chiappa Rhino 200DS revolver](assets/200DS.webp) |
| The top cover rail takes an ELCAN SpecterDR; the lower handguard rail takes an RK-series foregrip. | The front sight slot takes the StormWerkz top cover rail and the MP-18 scope base. |

| CR 50DS front sight and tactical slots | PPSh-41 barrel and stock slots |
| --- | --- |
| ![Chiappa Rhino 50DS revolver](assets/50DS.webp) | ![PPSh-41 SMG](assets/PPSh.webp) |
| The front sight slot takes a scope base; the tactical slot takes an RK-series foregrip. | The barrel slot takes a Mosin barrel with the tri-rail mounted on it; the stock slot takes the M14ALCS (MOD-0) buttstock; the 730 mm Mosin barrel's front sight slot takes an MDR BLK LBL ALX bipod (16 / 20). |

## Documentation

- [Features and conflicts](docs/features.md)
- [Principles](docs/principle.md)
- [Build, test and release](docs/development.md)
- [In-game verification](docs/verification.md)
- [Compatibility and limitations](docs/compatibility.md)

## Supported versions

SPT `4.1.x`.

## License

[MIT](LICENSE)

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
