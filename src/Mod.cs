using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Tables;
using SPTarkov.Common.Models.Logging;

namespace Pj568.WeaponCrossModding;

/// <summary>
/// 服务端 mod：
///   1. 让 UZI StormWerkz 瞄具基座的 mod_scope 槽支持安装 ELCAN SpecterDR 1x/4x 及其 FDE 变体，
///      以及 SIG Sauer BRAVO4 4x30 瞄准镜；
///   2. 让 StormWerkz 顶盖导轨可安装到 CR 200DS 转轮手枪的前准星槽；
///   3. 让 CR 200DS 的前准星槽支持安装 MP-18 瞄具基座；
///   4. 让 PPSh-41 冲锋枪的枪管槽支持安装莫辛纳甘的全部 4 种尺寸枪管；
///   5. 让 PPSh-41 冲锋枪的枪托槽支持安装 Benelli M3 可伸缩枪托、PKM / PKP 枪托、Ultima MP-155 塑料手枪式握把与 KS-23 金属枪托；
///   6. 让 HUXWRX HX-QD 消音器（含黄褐色变体）与 PPSh-41 防尘盖互不兼容；
///   7. 让 Aim Sports“三轨”莫辛步枪导轨的第一个战术配件槽（mod_tactical_000）支持安装多种前握把（不含 KeyMod / M-LOK 型）；
///   8. 让 UZI StormWerkz 护木底轨的 mod_tactical 槽支持安装 Zenit RK 系列前握把；
///   9. 让 PPSh-41 冲锋枪的枪托槽支持安装 M14 SAGE International M14ALCS (MOD-0) 枪托；
///  10. 让 Aim Sports“三轨”的瞄具槽支持安装 M14 DCSB 瞄具基座；
///  11. 让 Aim Sports“三轨”的第一个战术配件槽支持安装 SV-98 隔热带与 Fortis Shift 战术前握把；
///  12. 让 Aim Sports“三轨”的第二个战术配件槽支持安装 SV-98 隔热带；
///  13. 让 M14 SAGE International M14ALCS (MOD-0) 枪托的握把位支持安装 AR-15 Tactical Dynamics 镂空手枪式握把、
///      Tyrant Designs MOD Chevron AR-15 镂空手枪式握把（黑 / 黄 / 红）与 AS VAL Rotor 43 手枪式握把附缓冲管转接器；
///  14. 让 CR 50DS 转轮手枪的前准星槽支持安装 StormWerkz 顶盖导轨与 MP-18 瞄具基座；
///  15. 让 CR 50DS 转轮手枪的战术设备槽支持安装 Zenit RK 系列前握把、KAC MWS 脚架转接器
///      与 BT10 V8 Atlas 折叠脚架；
///  16. 让 730mm 标准莫辛枪管的前准星槽支持安装 MDR BLK LBL ALX 脚架（16 与 20 型号）；
///  17. 让 AA-12 457mm 枪管的导轨槽（mod_mount）支持安装 M60 脚架；
///  18. 让 MP-18 与 Marlin MXLR 的枪托槽（mod_stock）支持安装 KS-23 金属枪托；
///  19. 让 RPD 520mm 枪管的枪口槽（mod_muzzle）支持安装 AKM PBS-1 消音器；
///  20. 让 PPSh-41 冲锋枪的枪管槽支持安装 M700 的全部 4 种枪管、ORSIS T-5000M 枪管
///      与 SKS / OP-SKS 的 520mm 枪管；
///  21. 让 PPSh-41 冲锋枪的枪托槽支持安装 Zveno PK 缓冲管转接器；
///  22. 让 PPSh-41 冲锋枪的机匣槽支持安装 TAPCO Intrafuse 与 Fab Defence UAS SKS 枪身套件；
///  23. 让两个 SKS 枪身套件与除 KS-23 金属枪托之外的全部 PPSh-41 枪托（含原装木制枪托与 Zveno 缓冲管转接器）互不兼容；
///  24. 让两个 SKS 枪身套件与 PPSh-41 的 71 发弹鼓互不兼容；
///  25. 让 KS-23 金属枪托与两个 SKS 枪身套件枪托槽可装的配件互不兼容；
///  26. 让 SKS 与 OP-SKS 照门固定环的照门槽（mod_sight_rear）支持安装 TKPD 导轨防尘盖；
///  27. 让 TKPD 导轨防尘盖与 SKS / OP-SKS 枪本体互不兼容；
///  28. 让 PPSh-41 冲锋枪的枪管槽支持安装 SVT-40 / AVT-40 共用的 7.62x54R 625mm 枪管；
///  29. 让 SVT-40 625mm 枪管的照门槽（mod_sight_rear）支持安装 TKPD 导轨防尘盖；
///  30. 让 TKPD 导轨防尘盖与 SVT-40 / AVT-40 枪本体双向互不兼容；
///  31. 让 SVT-40 标准枪口装置的准星槽（mod_sight_front）支持安装 MDR BLK LBL ALX 脚架（16 与 20 型号）；
///  32. 让 PPSh-41 冲锋枪的枪管槽支持安装 SVDS 照门固定环；
///  33. 将莫辛 200mm、M700 全部 4 种、ORSIS T-5000M 枪管自 PPSh-41 枪管槽移至 SVDS 照门固定环的护木槽（mod_handguard），
///      并让这些枪管与 SVDS 枪本体双向互不兼容；
///  34. 让 SVDS 照门固定环的照门槽（mod_sight_rear）支持安装 TKPD 导轨防尘盖，且该防尘盖与 SVDS 枪本体双向互不兼容；
///  35. 让 SVDS 照门固定环的护木槽（mod_handguard）支持安装 SVDS 枪管，且 SVDS 枪管与自身互斥；
///  36. 让 SVDS 照门固定环与 PPSh-41 防尘盖双向互不兼容；
///  37. 让 SVDS 照门固定环与 TAPCO Intrafuse SKS 枪身套件双向互不兼容；
///  38. 让 SVDS 照门固定环护木槽可装的 3 种原版 SVDS 护木与 PPSh-41 枪本体双向互不兼容；
///  39. 让 Fab Defence UAS SKS 枪身套件的下导轨（mod_tactical_002）支持安装 MDR BLK LBL ALX 20 脚架。
///
/// 做法：往目标槽的 SlotFilter.Filter（HashSet&lt;MongoId&gt;）追加物品 id（幂等）。
/// </summary>
[Injectable(InjectionType.Singleton, OnLoadOrder.Preload + 4)]
public class WeaponCrossModdingPlugin(
    ISptLogger<WeaponCrossModdingPlugin> logger,
    TemplateTable templateTable) : IOnLoad
{
    private const string ScopeSlotName = "mod_scope";
    private const string SightSlotName = "mod_sight_front";
    private const string SightRearSlotName = "mod_sight_rear";
    private const string BarrelSlotName = "mod_barrel";
    private const string StockSlotName = "mod_stock";
    private const string TacticalSlotName = "mod_tactical";
    private const string MountSlotName = "mod_mount";
    private const string MuzzleSlotName = "mod_muzzle";
    private const string RecieverSlotName = "mod_reciever"; // 游戏数据拼写即 reciever
    private const string HandguardSlotName = "mod_handguard";

    // UZI StormWerkz 瞄具基座（顶盖导轨）
    private const string StormwerkzTopCoverRailId = "6698c90829e062525d0ad8ad";

    // ELCAN SpecterDR 1x/4x（黑）与 FDE 变体
    private const string SpecterDrId = "57ac965c24597706be5f975c";
    private const string SpecterDrFdeId = "57aca93d2459771f2c7e26db";

    // SIG Sauer BRAVO4 4x30 瞄准镜
    private const string Bravo4Id = "57adff4f24597737f373b6e6";

    // CR 200DS 转轮手枪（Chiappa Rhino 200DS 9x19 revolver）—— 前准星槽。
    private const string Cr200DsId = "624c2e8614da335f1e034d8c";

    // CR 50DS 转轮手枪（Chiappa Rhino 50DS .357 revolver）—— 前准星槽与战术设备槽。
    private const string Cr50DsId = "61a4c8884f95bc3b2c5dc96f";

    // MP-18 瞄具基座
    private const string Mp18ScopeBaseId = "61f804acfcba9556ea304cb8";

    // KAC MWS 脚架转接器（自带 mod_bipod 槽）与 BT10 V8 Atlas 折叠脚架。
    private const string KacMwsBipodAdapterId = "676175bb48fa5c377e06fc36";
    private const string Bt10AtlasBipodId = "6644920d49817dc7d505ca71";

    // PPSh-41 冲锋枪（承载枪管槽与枪托槽）。
    private const string Ppsh41Id = "5ea03f7400685063ec28bfa8";

    // 莫辛纳甘枪管（4 种尺寸）
    private const string MosinBarrel200Id = "5bfd4cc90db834001d23e846"; // 200mm 锯短
    private const string MosinBarrel220ThreadedId = "5bfd4cd60db834001c38f095"; // 220mm 锯短螺纹
    private const string MosinBarrel514Id = "5bfd4cbe0db834001b73449f"; // 514mm 卡宾
    private const string MosinBarrel730Id = "5ae09bff5acfc4001562219d"; // 730mm 标准

    // MDR BLK LBL ALX 脚架（16 / 20 型号，由 WTT-ContentBackport 注入；安装于 730mm 标准莫辛枪管的前准星槽）。
    private const string AlxBipod16Id = "680f6d9a4d7624d36e06527b";
    private const string AlxBipod20Id = "680f7e4aeee716732708e84e";

    // PPSh-41 枪托槽兼容的枪托 / 握把。
    private const string BenelliM3TelescopicStockId = "6259c3387d6aab70bc23a18d"; // Benelli M3 可伸缩枪托
    private const string PkmWoodenStockId = "646371a9f2404ab67905c8e6"; // PKM 木制枪托
    private const string PkZenitPt2StockId = "6492d7847363b8a52206bc52"; // Zenit PT-2 "Klassika" PK 机枪枪托
    private const string PkpPolymerStockId = "6492e3a97df7d749100e29ee"; // PKP 聚合物枪托
    private const string UltimaMp155PistolGripId = "606eef46232e5a31c233d500"; // Ultima MP-155 塑料手枪式握把
    private const string Ks23MetalStockId = "5e848dc4e4dbc5266a4ec63d"; // KS-23 金属枪托（原版仅可装于 KS-23M 聚合物手枪式握把的 mod_stock 槽）

    // M14 SAGE International M14ALCS (MOD-0) 枪托。
    private const string M14AlcsButtstockId = "5addc7ac5acfc400194dbd90";

    // M14 SAGE International DCSB 瞄具基座。
    private const string M14DcsbMountId = "5addbffe5acfc4001714dfac";

    // PPSh-41 防尘盖（mod_reciever 槽承载物品）。
    private const string Ppsh41DustCoverId = "5ea03e5009aa976f2e7a514b";

    // HUXWRX HX-QD 7.62x51 消音器（黑 / 黄褐色变体）—— 由 WTT-ContentBackport 注入。
    private const string HuxwrxHxQdId = "6a158e4abf497aade10030e0";
    private const string HuxwrxHxQdTanId = "6a1eb32c6cd328ea90037455";

    // Aim Sports“三轨”莫辛步枪导轨；改第一个与第二个战术配件槽（mod_tactical_000 / mod_tactical_001）。
    private const string AimSportsTriRailId = "5bbdb811d4351e45020113c7";
    private const string AimSportsTriRailTacticalSlotName = "mod_tactical_000";
    private const string AimSportsTriRailSecondTacticalSlotName = "mod_tactical_001";
    private const string AimSportsTriRailLabel = "Aim Sports tri-rail";

    // Fab Defence UAS SKS 枪身套件的下导轨（原本兼容脚架的第三个战术槽）。
    private const string FabDefenceUasLowerRailSlotName = "mod_tactical_002";

    // UZI StormWerkz 护木底轨（mod_tactical 槽）。
    private const string StormwerkzLowerHandguardRailId = "66992f7d9950f5f4cd0602a8";
    private const string StormwerkzLowerHandguardLabel = "UZI StormWerkz lower handguard rail";

    // Zenit RK 系列前握把。
    private static readonly string[] ZenitRkForegripIds =
    [
        "5c1bc4812e22164bef5cfde7", // RK-0
        "5c1bc5612e221602b5429350", // RK-1
        "5c1bc5af2e221602b412949b", // RK-2
        "5c1bc5fb2e221602b1779b32", // RK-4
        "5c1bc7432e221602b412949d", // RK-5
        "5c1bc7752e221602b1779b34", // RK-6
    ];

    // Aim Sports“三轨”第一个战术配件槽兼容的前握把（RK 系列见上；不含 KeyMod / M-LOK 型）。
    private static readonly string[] AimSportsTriRailForegripIds =
    [
        .. ZenitRkForegripIds,
        // 镂空前握把
        "5f6340d3ca442212f4047eb2", // Tactical Dynamics 镂空前握把
        // 垂直前握把
        "5c7fc87d2e221644f31c0298", // BCM GUNFIGHTER MOD 3 vertical
        "5c87ca002e221600114cb150", // KAC vertical
        "5cda9bcfd7f00c0c0b53e900", // ASh-12 vertical
        // TangoDown Stubby BGV-MK46K
        "558032614bdc2de7118b4585", // (Black)
        "58c157be86f77403c74b2bb6", // (FDE)
        "58c157c886f774032749fb06", // (Stealth Grey)
    ];

    // Aim Sports“三轨”第一 / 第二个战术配件槽兼容的其它配件。
    private const string Sv98HeatRibbonId = "56083eab4bdc2d26448b456a"; // SV-98 隔热带（anti-heat ribbon）
    private const string FortisShiftForegripId = "59f8a37386f7747af3328f06"; // Fortis Shift 战术前握把

    // M14 SAGE International M14ALCS (MOD-0) 枪托的握把位兼容的握把。
    private const string M14AlcsPistolGripSlotName = "mod_pistol_grip";
    private const string TacticalDynamicsSkeletonizedGripId = "5b07db875acfc40dc528a5f6"; // AR-15 Tactical Dynamics 镂空手枪式握把

    // Tyrant Designs MOD Chevron AR-15 镂空手枪式握把（黑 / 黄 / 红变体）—— 由 WTT-ContentBackport 注入。
    private const string TyrantChevronGripBlackId = "6984b7d56be2752c150e6895";
    private const string TyrantChevronGripYellowId = "698dac21772d6f3dc00e4284";
    private const string TyrantChevronGripRedId = "6985eb089edef67ade080b72";

    // AS VAL Rotor 43 手枪式握把附缓冲管转接器（自带 AR 规格缓冲管与 mod_stock_000 槽）。
    private const string AsValRotor43GripId = "5a69a2ed8dc32e000d46d1f1";

    // AA-12 457mm 枪管（mod_mount 导轨槽宿主）。
    private const string Aa12Barrel457Id = "670fced86a7e274b1a0964e8";

    // MP-18 7.62x54R 单发步枪。
    private const string Mp18RifleId = "61f7c9e189e6fb1a5e3ea78d";

    // Marlin MXLR .308 ME 杠杆步枪（由 WTT-ContentBackport 注入）。
    private const string MarlinMxlrId = "67c6de3ce39861860909e8e5";

    // M60 脚架。
    private const string M60BipodId = "66012d9a3dff5074ed002e33";

    // RPD 520mm 枪管（mod_muzzle 枪口槽宿主）与 AKM PBS-1 消音器。
    private const string RpdBarrel520Id = "6513eff1e06849f06c0957d4";
    private const string AkmPbs1Id = "5a0d63621526d8dba31fe3bf";

    // M700 枪管（4 种）与 ORSIS T-5000M 枪管（可安装于 PPSh-41 枪管槽）。
    private const string M700Barrel660Id = "5bfebc250db834001a6694e1"; // 26 英寸
    private const string M700Barrel508ThreadedId = "5bfebc320db8340019668d79"; // 20 英寸螺纹
    private const string M700BarrelStainless660Id = "5d2702e88abbc31ed91efc44"; // 26 英寸不锈钢
    private const string M700BarrelStainless508ThreadedId = "5d2703038abbc3105103d94c"; // 20 英寸不锈钢螺纹
    private const string T5000Barrel660Id = "5df256570dee1b22f862e9c4"; // ORSIS T-5000M 660mm

    // SKS 与 OP-SKS 的 520mm 枪管（可安装于 PPSh-41 枪管槽）。
    private const string SksBarrel520Id = "634f02331f9f536910079b51"; // SKS（TOZ）
    private const string OpsksBarrel520Id = "634eff66517ccc8a960fc735"; // OP-SKS（Molot）

    // SKS 枪身套件（可安装于 PPSh-41 机匣槽）：TAPCO Intrafuse 与 Fab Defence UAS。
    private const string TapcoIntrafuseSksId = "5afd7ded5acfc40017541f5e";
    private const string FabDefenceUasSksId = "5d0236dad7ad1a0940739d29";

    // 上述两个套件的枪托槽分别只接受的配件（用于与 KS-23 金属枪托建立互斥）。
    private const string TapcoIntrafuseBufferTubeId = "5afd7e095acfc40017541f61";
    private const string FabDefenceUasFoldingStockId = "653ed132896b99b40a0292e6";

    // PPSh-41 原装木制枪托（用于与 SKS 枪身套件建立互斥）。
    private const string Ppsh41StockWoodId = "5ea03e9400685063ec28bfa4";

    // Zveno PK 缓冲管转接器（由 WTT-ContentBackport 注入；可安装于 PPSh-41 枪托槽）。
    private const string ZvenoBufferTubeId = "6a182c39b913af92800d8b5d";

    // PPSh-41 7.62x25 71 发弹鼓。
    private const string Ppsh71DrumMagId = "5ea034f65aad6446a939737e";

    // SKS / OP-SKS 照门固定环（mod_sight_rear 照门槽宿主）与 TKPD 导轨防尘盖（由 WTT 注入）。
    private const string SksRearSightBlockId = "634f04d82e5def262d0b30c6";
    private const string OpsksRearSightBlockId = "634f05a21f9f536910079b56";
    private const string TkpdRailedDustCoverId = "68aee8f8130c00663d08aeb3";

    // SKS 与 OP-SKS 枪本体（用于与 TKPD 导轨防尘盖建立互斥）。
    private const string SksWeaponId = "574d967124597745970e7c94";
    private const string OpsksWeaponId = "587e02ff24597743df3deaeb";

    // SVT-40 7.62x54R 625mm 枪管（可安装于 PPSh-41 枪管槽；其 mod_sight_rear 照门槽承载 TKPD 导轨防尘盖）。
    private const string Svt40Barrel625Id = "6410758c857473525b08bb77";

    // SVT-40 与 AVT-40 枪本体（用于与 TKPD 导轨防尘盖建立互斥）。
    private const string Svt40WeaponId = "643ea5b23db6f9f57107d9fd";
    private const string Avt40WeaponId = "6410733d5dd49d77bd07847e";

    // SVT-40 标准枪口装置（其 mod_sight_front 准星槽承载 MDR BLK LBL ALX 脚架；SVT-40 与 AVT-40 共用）。
    private const string Svt40MuzzleStdId = "64119d1f2c6d6f921a0929f8";

    // SVDS 照门固定环（mount_svd_izhmash_svd_s_lower_band_std，含 mod_handguard 护木槽与 mod_sight_rear 照门槽）。
    private const string SvdsRearSightBlockId = "5c471c2d2e22164bef5d077f";

    // SVDS 枪本体与 SVDS 枪管（枪管用于与自身建立互斥以防重复安装）。
    private const string SvdsWeaponId = "5c46fbd72e2216398b5a8c9c";
    private const string SvdsBarrelId = "5c471cb32e221602b177afaa";

    // 自 PPSh-41 枪管槽移至 SVDS 照门固定环护木槽的枪管（用于与 SVDS 枪本体建立互斥）。
    private static readonly (string Id, string Label)[] SvdsMovedBarrels =
    [
        (MosinBarrel200Id, "Mosin 200mm barrel"),
        (M700Barrel660Id, "M700 26-inch barrel"),
        (M700Barrel508ThreadedId, "M700 20-inch threaded barrel"),
        (M700BarrelStainless660Id, "M700 26-inch stainless barrel"),
        (M700BarrelStainless508ThreadedId, "M700 20-inch stainless threaded barrel"),
        (T5000Barrel660Id, "ORSIS T-5000M 660mm barrel"),
    ];

    // SVDS 照门固定环 mod_handguard 护木槽可装的原版护木（用于与 PPSh-41 枪本体建立双向互斥）。
    private static readonly (string Id, string Label)[] SvdsHandguards =
    [
        ("5e5699df2161e06ac158df6f", "SVDS CAA XRS DRG handguard"),
        ("5e56991336989c75ab4f03f6", "SVDS modernized kit handguard"),
        ("5c471c6c2e221602b66cd9ae", "SVDS standard handguard"),
    ];

    // PPSh-41 枪托槽可装的全部枪托（用于与 SKS 枪身套件建立双向互斥；不含 KS-23 金属枪托）。
    private static readonly (string Id, string Label)[] Ppsh41StockConflicts =
    [
        (Ppsh41StockWoodId, "PPSh-41 wooden stock"),
        (BenelliM3TelescopicStockId, "Benelli M3 telescopic stock"),
        (PkmWoodenStockId, "PKM wooden stock"),
        (PkZenitPt2StockId, "Zenit PT-2 stock"),
        (PkpPolymerStockId, "PKP polymer stock"),
        (UltimaMp155PistolGripId, "Ultima MP-155 pistol grip"),
        (M14AlcsButtstockId, "M14ALCS (MOD-0) stock"),
        (ZvenoBufferTubeId, "Zveno buffer tube adapter"),
    ];

    // PPSh-41 机匣槽可装的 SKS 枪身套件。
    private static readonly (string Id, string Label)[] SksChassisKits =
    [
        (TapcoIntrafuseSksId, "TAPCO Intrafuse SKS"),
        (FabDefenceUasSksId, "Fab Defence UAS SKS"),
    ];

    // 两个 SKS 枪身套件枪托槽可装的配件（用于与 KS-23 金属枪托建立双向互斥）。
    private static readonly (string Id, string Label)[] SksChassisStockParts =
    [
        (TapcoIntrafuseBufferTubeId, "TAPCO Intrafuse buffer tube"),
        (FabDefenceUasFoldingStockId, "Fab Defence UAS folding stock"),
    ];

    // 日志中用于标识槽位承载者的短名。
    private const string MountLabel = "mount";
    private const string Cr200DsLabel = "CR 200DS";
    private const string Cr50DsLabel = "CR 50DS";
    private const string Ppsh41Label = "PPSh-41";
    private const string M14ButtstockLabel = "M14ALCS (MOD-0) stock";
    private const string RpdBarrel520Label = "RPD 520mm barrel";
    private const string MosinBarrel730Label = "Mosin 730mm barrel";
    private const string Aa12Barrel457Label = "AA-12 457mm barrel";
    private const string Mp18RifleLabel = "MP-18 rifle";
    private const string MarlinMxlrLabel = "Marlin MXLR";
    private const string SksRearSightBlockLabel = "SKS rear sight block";
    private const string OpsksRearSightBlockLabel = "OP-SKS rear sight block";
    private const string TkpdRailedDustCoverLabel = "TKPD railed dust cover";
    private const string Svt40Barrel625Label = "SVT-40 625mm barrel";
    private const string Svt40MuzzleStdLabel = "SVT-40 muzzle";
    private const string SvdsRearSightBlockLabel = "SVDS rear sight block";
    private const string SvdsBarrelLabel = "SVDS barrel";
    private const string FabDefenceUasSksLabel = "Fab Defence UAS SKS";

    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            Dictionary<MongoId, TemplateItem> items = templateTable.Items;

            // UZI StormWerkz 顶盖导轨的 mod_scope 槽：追加 SpecterDR 与 BRAVO4 瞄具。
            AddItemIdsToSlot(items, StormwerkzTopCoverRailId, MountLabel, ScopeSlotName, SpecterDrId, SpecterDrFdeId, Bravo4Id);

            // CR 200DS 前准星槽：追加 StormWerkz 顶盖导轨与 MP-18 瞄具基座。
            AddItemIdsToSlot(items, Cr200DsId, Cr200DsLabel, SightSlotName, StormwerkzTopCoverRailId, Mp18ScopeBaseId);

            // CR 50DS 前准星槽：追加 StormWerkz 顶盖导轨与 MP-18 瞄具基座。
            AddItemIdsToSlot(items, Cr50DsId, Cr50DsLabel, SightSlotName, StormwerkzTopCoverRailId, Mp18ScopeBaseId);

            // CR 50DS 战术设备槽：追加 Zenit RK 系列前握把、KAC MWS 脚架转接器与 BT10 V8 Atlas 折叠脚架。
            AddItemIdsToSlot(items, Cr50DsId, Cr50DsLabel, TacticalSlotName, [.. ZenitRkForegripIds, KacMwsBipodAdapterId, Bt10AtlasBipodId]);

            // PPSh-41 枪管槽：追加莫辛纳甘 220mm 螺纹 / 514mm / 730mm 枪管、SKS / OP-SKS 的 520mm 枪管、
            // SVT-40 / AVT-40 共用的 625mm 枪管与 SVDS 照门固定环（原莫辛 200mm、M700、ORSIS T-5000M 已移至 SVDS 照门固定环）。
            AddItemIdsToSlot(items, Ppsh41Id, Ppsh41Label, BarrelSlotName,
                MosinBarrel220ThreadedId, MosinBarrel514Id, MosinBarrel730Id,
                SksBarrel520Id, OpsksBarrel520Id, Svt40Barrel625Id,
                SvdsRearSightBlockId);

            // PPSh-41 机匣槽（mod_reciever）：追加 TAPCO Intrafuse 与 Fab Defence UAS SKS 枪身套件。
            AddItemIdsToSlot(items, Ppsh41Id, Ppsh41Label, RecieverSlotName, TapcoIntrafuseSksId, FabDefenceUasSksId);

            // 730mm 标准莫辛枪管的前准星槽：追加 MDR BLK LBL ALX 脚架（16 与 20 型号）。
            AddItemIdsToSlot(items, MosinBarrel730Id, MosinBarrel730Label, SightSlotName, AlxBipod16Id, AlxBipod20Id);

            // SVT-40 标准枪口装置的 mod_sight_front 前准星槽：追加 MDR BLK LBL ALX 脚架（16 与 20 型号）。
            AddItemIdsToSlot(items, Svt40MuzzleStdId, Svt40MuzzleStdLabel, SightSlotName, AlxBipod16Id, AlxBipod20Id);

            // PPSh-41 枪托槽：追加 Benelli M3 可伸缩枪托、PKM / PKP 枪托、Ultima MP-155 握把、
            // KS-23 金属枪托、M14 SAGE International M14ALCS (MOD-0) 枪托与 Zveno PK 缓冲管转接器。
            AddItemIdsToSlot(items, Ppsh41Id, Ppsh41Label, StockSlotName, BenelliM3TelescopicStockId, PkmWoodenStockId, PkZenitPt2StockId, PkpPolymerStockId, UltimaMp155PistolGripId, Ks23MetalStockId, M14AlcsButtstockId, ZvenoBufferTubeId);

            // SKS / OP-SKS 照门固定环的 mod_sight_rear 照门槽：追加 TKPD 导轨防尘盖。
            AddItemIdsToSlot(items, SksRearSightBlockId, SksRearSightBlockLabel, SightRearSlotName, TkpdRailedDustCoverId);
            AddItemIdsToSlot(items, OpsksRearSightBlockId, OpsksRearSightBlockLabel, SightRearSlotName, TkpdRailedDustCoverId);

            // SVT-40 625mm 枪管的 mod_sight_rear 照门槽：追加 TKPD 导轨防尘盖。
            AddItemIdsToSlot(items, Svt40Barrel625Id, Svt40Barrel625Label, SightRearSlotName, TkpdRailedDustCoverId);

            // SVDS 照门固定环的 mod_handguard 护木槽：追加莫辛 200mm、M700 全部 4 种、ORSIS T-5000M 与 SVDS 枪管。
            AddItemIdsToSlot(items, SvdsRearSightBlockId, SvdsRearSightBlockLabel, HandguardSlotName,
                MosinBarrel200Id, M700Barrel660Id, M700Barrel508ThreadedId, M700BarrelStainless660Id, M700BarrelStainless508ThreadedId, T5000Barrel660Id,
                SvdsBarrelId);

            // SVDS 照门固定环的 mod_sight_rear 照门槽：追加 TKPD 导轨防尘盖。
            AddItemIdsToSlot(items, SvdsRearSightBlockId, SvdsRearSightBlockLabel, SightRearSlotName, TkpdRailedDustCoverId);

            // PPSh-41 防尘盖与 HUXWRX HX-QD 消音器互不兼容。
            AddDustCoverSuppressorConflict(items);

            // 两个 SKS 枪身套件（机匣槽）与除 KS-23 金属枪托之外的全部 PPSh-41 枪托互不兼容。
            AddSksChassisConflicts(items);

            // 两个 SKS 枪身套件与 PPSh-41 的 71 发弹鼓互不兼容。
            AddSksChassisDrumConflict(items);

            // KS-23 金属枪托与两个 SKS 枪身套件枪托槽可装的配件互不兼容。
            AddKs23StockSksChassisStockPartConflicts(items);

            // TKPD 导轨防尘盖与 SKS / OP-SKS 枪本体互不兼容。
            AddDustCoverSksConflicts(items);

            // TKPD 导轨防尘盖与 SVT-40 / AVT-40 枪本体双向互不兼容。
            AddDustCoverSvtAvtConflicts(items);

            // SVDS 照门固定环引入的互斥（移植枪管、TKPD 与 SVDS 枪本体；SVDS 枪管防重；照门固定环与 PPSh-41 防尘盖）。
            AddSvdsConflicts(items);

            // Aim Sports“三轨”的第一个战术配件槽：追加多种前握把、SV-98 隔热带与 Fortis Shift 前握把。
            AddItemIdsToSlot(items, AimSportsTriRailId, AimSportsTriRailLabel, AimSportsTriRailTacticalSlotName, [.. AimSportsTriRailForegripIds, Sv98HeatRibbonId, FortisShiftForegripId]);

            // Aim Sports“三轨”的第二个战术配件槽：追加 SV-98 隔热带。
            AddItemIdsToSlot(items, AimSportsTriRailId, AimSportsTriRailLabel, AimSportsTriRailSecondTacticalSlotName, Sv98HeatRibbonId);

            // Aim Sports“三轨”的瞄具槽：追加 M14 DCSB 瞄具基座。
            AddItemIdsToSlot(items, AimSportsTriRailId, AimSportsTriRailLabel, ScopeSlotName, M14DcsbMountId);

            // Fab Defence UAS SKS 枪身套件的下导轨（mod_tactical_002）：追加 MDR BLK LBL ALX 20 脚架。
            AddItemIdsToSlot(items, FabDefenceUasSksId, FabDefenceUasSksLabel, FabDefenceUasLowerRailSlotName, AlxBipod20Id);

            // M14ALCS (MOD-0) 枪托的握把位：追加 AR-15 Tactical Dynamics 镂空手枪式握把、
            // Tyrant Designs MOD Chevron 镂空手枪式握把（黑 / 黄 / 红）与 AS VAL Rotor 43 手枪式握把。
            AddItemIdsToSlot(items, M14AlcsButtstockId, M14ButtstockLabel, M14AlcsPistolGripSlotName,
                TacticalDynamicsSkeletonizedGripId, TyrantChevronGripBlackId, TyrantChevronGripYellowId, TyrantChevronGripRedId, AsValRotor43GripId);

            // UZI StormWerkz 护木底轨的 mod_tactical 槽：追加 Zenit RK 系列前握把。
            AddItemIdsToSlot(items, StormwerkzLowerHandguardRailId, StormwerkzLowerHandguardLabel, TacticalSlotName, ZenitRkForegripIds);

            // AA-12 457mm 枪管的 mod_mount 导轨槽：追加 M60 脚架。
            AddItemIdsToSlot(items, Aa12Barrel457Id, Aa12Barrel457Label, MountSlotName, M60BipodId);

            // RPD 520mm 枪管的 mod_muzzle 枪口槽：追加 AKM PBS-1 消音器。
            AddItemIdsToSlot(items, RpdBarrel520Id, RpdBarrel520Label, MuzzleSlotName, AkmPbs1Id);

            // MP-18 与 Marlin MXLR 的 mod_stock 枪托槽：追加 KS-23 金属枪托。
            AddItemIdsToSlot(items, Mp18RifleId, Mp18RifleLabel, StockSlotName, Ks23MetalStockId);
            AddItemIdsToSlot(items, MarlinMxlrId, MarlinMxlrLabel, StockSlotName, Ks23MetalStockId);
        }
        catch (Exception ex)
        {
            logger.Error("WeaponCrossModding failed: " + ex.Message);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 往 <paramref name="ownerId"/> 名为 <paramref name="slotName"/> 的槽的各 Filter 追加 <paramref name="itemIds"/>（幂等）。
    /// </summary>
    private void AddItemIdsToSlot(
        Dictionary<MongoId, TemplateItem> items,
        string ownerId,
        string ownerLabel,
        string slotName,
        params string[] itemIds)
    {
        if (!items.TryGetValue(ownerId, out TemplateItem? owner))
        {
            logger.Warning($"WeaponCrossModding: {ownerLabel} id '{ownerId}' not found in Items");
            return;
        }

        IEnumerable<Slot>? slots = owner.Properties?.Slots;
        if (slots is null)
        {
            logger.Warning($"WeaponCrossModding: {ownerLabel} has no slots");
            return;
        }

        foreach (Slot slot in slots.Where(s => s is not null && string.Equals(s.Name, slotName, StringComparison.OrdinalIgnoreCase)))
        {
            IEnumerable<SlotFilter>? filters = slot.Properties?.Filters;
            if (filters is null)
            {
                continue;
            }

            foreach (SlotFilter filter in filters)
            {
                foreach (string itemId in itemIds)
                {
                    AddToFilter(filter, itemId, owner, slot);
                }
            }
        }
    }

    /// <summary>
    /// 让 PPSh-41 防尘盖与 HUXWRX HX-QD 消音器（含黄褐色变体）互不兼容（幂等）。
    /// 防尘盖为原版物品、必定存在；消音器由 WTT-ContentBackport 注入，若不在 Items 中则跳过
    /// （防尘盖侧足以建立冲突，且避开了第三方 mod 的加载时序依赖）。
    /// </summary>
    private void AddDustCoverSuppressorConflict(Dictionary<MongoId, TemplateItem> items)
    {
        AddConflictingItems(items, Ppsh41DustCoverId, "PPSh-41 dust cover", HuxwrxHxQdId, HuxwrxHxQdTanId);
        AddConflictingItems(items, HuxwrxHxQdId, "HUXWRX HX-QD", Ppsh41DustCoverId);
        AddConflictingItems(items, HuxwrxHxQdTanId, "HUXWRX HX-QD (Tan)", Ppsh41DustCoverId);
    }

    /// <summary>
    /// 让两个 SKS 枪身套件（装于 PPSh-41 机匣槽）与除 KS-23 金属枪托之外的
    /// 全部 PPSh-41 枪托双向互不兼容（幂等）。
    /// Zveno 由 WTT-ContentBackport 注入，若缺失则跳过该项。
    /// </summary>
    private void AddSksChassisConflicts(Dictionary<MongoId, TemplateItem> items)
    {
        foreach ((string chassisId, string chassisLabel) in SksChassisKits)
        {
            AddConflictingItems(items, chassisId, chassisLabel, [.. Ppsh41StockConflicts.Select(x => x.Id)]);
            foreach ((string stockId, string stockLabel) in Ppsh41StockConflicts)
            {
                AddConflictingItems(items, stockId, stockLabel, chassisId);
            }
        }
    }

    /// <summary>
    /// 让两个 SKS 枪身套件（装于 PPSh-41 机匣槽）与 PPSh-41 的 71 发弹鼓双向互不兼容（幂等）。
    /// 两者均为原版物品、必定存在，故双向建立冲突。
    /// </summary>
    private void AddSksChassisDrumConflict(Dictionary<MongoId, TemplateItem> items)
    {
        foreach ((string chassisId, string chassisLabel) in SksChassisKits)
        {
            AddConflictingItems(items, chassisId, chassisLabel, Ppsh71DrumMagId);
            AddConflictingItems(items, Ppsh71DrumMagId, "PPSh-41 71-round drum", chassisId);
        }
    }

    /// <summary>
    /// 让 KS-23 金属枪托与两个 SKS 枪身套件枪托槽可装的配件双向互不兼容（幂等）。
    /// 所有相关物品均为原版物品、必定存在，故双向建立冲突。
    /// </summary>
    private void AddKs23StockSksChassisStockPartConflicts(Dictionary<MongoId, TemplateItem> items)
    {
        foreach ((string partId, string partLabel) in SksChassisStockParts)
        {
            AddConflictingItems(items, Ks23MetalStockId, "KS-23 metal stock", partId);
            AddConflictingItems(items, partId, partLabel, Ks23MetalStockId);
        }
    }

    /// <summary>
    /// 让 TKPD 导轨防尘盖与 SKS / OP-SKS 枪本体双向互不兼容（幂等）。
    /// SKS / OP-SKS 为原版物品；TKPD 导轨防尘盖由 WTT-ContentBackport 注入，若缺失则跳过。
    /// </summary>
    private void AddDustCoverSksConflicts(Dictionary<MongoId, TemplateItem> items)
    {
        AddConflictingItems(items, TkpdRailedDustCoverId, TkpdRailedDustCoverLabel, SksWeaponId, OpsksWeaponId);
        AddConflictingItems(items, SksWeaponId, "SKS", TkpdRailedDustCoverId);
        AddConflictingItems(items, OpsksWeaponId, "OP-SKS", TkpdRailedDustCoverId);
    }

    /// <summary>
    /// 让 TKPD 导轨防尘盖与 SVT-40 / AVT-40 枪本体双向互不兼容（幂等）。
    /// SVT-40 / AVT-40 为原版物品；TKPD 导轨防尘盖由 WTT-ContentBackport 注入，若缺失则跳过。
    /// </summary>
    private void AddDustCoverSvtAvtConflicts(Dictionary<MongoId, TemplateItem> items)
    {
        AddConflictingItems(items, TkpdRailedDustCoverId, TkpdRailedDustCoverLabel, Svt40WeaponId, Avt40WeaponId);
        AddConflictingItems(items, Svt40WeaponId, "SVT-40", TkpdRailedDustCoverId);
        AddConflictingItems(items, Avt40WeaponId, "AVT-40", TkpdRailedDustCoverId);
    }

    /// <summary>
    /// SVDS 照门固定环引入的互斥（幂等）：
    /// 移植到护木槽的枪管与 SVDS 枪本体双向互斥（避免经照门固定环装到原版 SVDS 上）；
    /// TKPD 导轨防尘盖与 SVDS 枪本体双向互斥；SVDS 枪管与自身互斥（防止装两个）；
    /// SVDS 照门固定环与 PPSh-41 防尘盖、TAPCO Intrafuse SKS 枪身套件双向互斥；
    /// SVDS 照门固定环护木槽的原版护木与 PPSh-41 枪本体双向互斥。
    /// TKPD 与 SVDS 枪管由 WTT-ContentBackport 注入，若缺失则跳过其侧。
    /// </summary>
    private void AddSvdsConflicts(Dictionary<MongoId, TemplateItem> items)
    {
        AddConflictingItems(items, SvdsWeaponId, "SVDS", [.. SvdsMovedBarrels.Select(x => x.Id)]);
        foreach ((string barrelId, string barrelLabel) in SvdsMovedBarrels)
        {
            AddConflictingItems(items, barrelId, barrelLabel, SvdsWeaponId);
        }

        AddConflictingItems(items, TkpdRailedDustCoverId, TkpdRailedDustCoverLabel, SvdsWeaponId);
        AddConflictingItems(items, SvdsWeaponId, "SVDS", TkpdRailedDustCoverId);

        AddConflictingItems(items, SvdsBarrelId, SvdsBarrelLabel, SvdsBarrelId);

        AddConflictingItems(items, SvdsRearSightBlockId, SvdsRearSightBlockLabel, Ppsh41DustCoverId);
        AddConflictingItems(items, Ppsh41DustCoverId, "PPSh-41 dust cover", SvdsRearSightBlockId);

        AddConflictingItems(items, SvdsRearSightBlockId, SvdsRearSightBlockLabel, TapcoIntrafuseSksId);
        AddConflictingItems(items, TapcoIntrafuseSksId, "TAPCO Intrafuse SKS", SvdsRearSightBlockId);

        AddConflictingItems(items, Ppsh41Id, Ppsh41Label, [.. SvdsHandguards.Select(x => x.Id)]);
        foreach ((string handguardId, string handguardLabel) in SvdsHandguards)
        {
            AddConflictingItems(items, handguardId, handguardLabel, Ppsh41Id);
        }
    }

    /// <summary>
    /// 往 <paramref name="ownerId"/> 的 ConflictingItems 追加 <paramref name="conflictIds"/>（幂等）。
    /// </summary>
    private void AddConflictingItems(
        Dictionary<MongoId, TemplateItem> items,
        string ownerId,
        string ownerLabel,
        params string[] conflictIds)
    {
        if (!items.TryGetValue(ownerId, out TemplateItem? owner))
        {
            logger.Debug($"WeaponCrossModding: {ownerLabel} id '{ownerId}' not in Items, skip conflicting items");
            return;
        }

        owner.Properties ??= new TemplateItemProperties();
        owner.Properties.ConflictingItems ??= new HashSet<MongoId>();

        foreach (string conflictId in conflictIds)
        {
            if (owner.Properties.ConflictingItems.Add(conflictId))
            {
                logger.Info($"WeaponCrossModding: added conflicting item '{conflictId}' to '{ownerLabel}' ({owner.Name})");
            }
            else
            {
                logger.Debug($"WeaponCrossModding: conflicting item '{conflictId}' already on '{ownerLabel}' ({owner.Name})");
            }
        }
    }

    private void AddToFilter(SlotFilter? filter, string itemId, TemplateItem parent, Slot slot)
    {
        if (filter is null)
        {
            return;
        }

        filter.Filter ??= new HashSet<MongoId>();

        if (filter.Filter.Add(itemId))
        {
            logger.Info($"WeaponCrossModding: added '{itemId}' to slot '{slot.Name}' ({parent.Name})");
        }
        else
        {
            logger.Debug($"WeaponCrossModding: '{itemId}' already in slot '{slot.Name}' ({parent.Name})");
        }
    }
}
