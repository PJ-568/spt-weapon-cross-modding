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
    private const string BarrelSlotName = "mod_barrel";
    private const string StockSlotName = "mod_stock";
    private const string TacticalSlotName = "mod_tactical";
    private const string MountSlotName = "mod_mount";

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

    // 日志中用于标识槽位承载者的短名。
    private const string MountLabel = "mount";
    private const string Cr200DsLabel = "CR 200DS";
    private const string Cr50DsLabel = "CR 50DS";
    private const string Ppsh41Label = "PPSh-41";
    private const string M14ButtstockLabel = "M14ALCS (MOD-0) stock";
    private const string MosinBarrel730Label = "Mosin 730mm barrel";
    private const string Aa12Barrel457Label = "AA-12 457mm barrel";
    private const string Mp18RifleLabel = "MP-18 rifle";
    private const string MarlinMxlrLabel = "Marlin MXLR";

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

            // PPSh-41 枪管槽：追加莫辛纳甘的全部尺寸枪管。
            AddItemIdsToSlot(items, Ppsh41Id, Ppsh41Label, BarrelSlotName, MosinBarrel200Id, MosinBarrel220ThreadedId, MosinBarrel514Id, MosinBarrel730Id);

            // 730mm 标准莫辛枪管的前准星槽：追加 MDR BLK LBL ALX 脚架（16 与 20 型号）。
            AddItemIdsToSlot(items, MosinBarrel730Id, MosinBarrel730Label, SightSlotName, AlxBipod16Id, AlxBipod20Id);

            // PPSh-41 枪托槽：追加 Benelli M3 可伸缩枪托、PKM / PKP 枪托、Ultima MP-155 握把、
            // KS-23 金属枪托，以及 M14 SAGE International M14ALCS (MOD-0) 枪托。
            AddItemIdsToSlot(items, Ppsh41Id, Ppsh41Label, StockSlotName, BenelliM3TelescopicStockId, PkmWoodenStockId, PkZenitPt2StockId, PkpPolymerStockId, UltimaMp155PistolGripId, Ks23MetalStockId, M14AlcsButtstockId);

            // PPSh-41 防尘盖与 HUXWRX HX-QD 消音器互不兼容。
            AddDustCoverSuppressorConflict(items);

            // Aim Sports“三轨”的第一个战术配件槽：追加多种前握把、SV-98 隔热带与 Fortis Shift 前握把。
            AddItemIdsToSlot(items, AimSportsTriRailId, AimSportsTriRailLabel, AimSportsTriRailTacticalSlotName, [.. AimSportsTriRailForegripIds, Sv98HeatRibbonId, FortisShiftForegripId]);

            // Aim Sports“三轨”的第二个战术配件槽：追加 SV-98 隔热带。
            AddItemIdsToSlot(items, AimSportsTriRailId, AimSportsTriRailLabel, AimSportsTriRailSecondTacticalSlotName, Sv98HeatRibbonId);

            // Aim Sports“三轨”的瞄具槽：追加 M14 DCSB 瞄具基座。
            AddItemIdsToSlot(items, AimSportsTriRailId, AimSportsTriRailLabel, ScopeSlotName, M14DcsbMountId);

            // M14ALCS (MOD-0) 枪托的握把位：追加 AR-15 Tactical Dynamics 镂空手枪式握把、
            // Tyrant Designs MOD Chevron 镂空手枪式握把（黑 / 黄 / 红）与 AS VAL Rotor 43 手枪式握把。
            AddItemIdsToSlot(items, M14AlcsButtstockId, M14ButtstockLabel, M14AlcsPistolGripSlotName,
                TacticalDynamicsSkeletonizedGripId, TyrantChevronGripBlackId, TyrantChevronGripYellowId, TyrantChevronGripRedId, AsValRotor43GripId);

            // UZI StormWerkz 护木底轨的 mod_tactical 槽：追加 Zenit RK 系列前握把。
            AddItemIdsToSlot(items, StormwerkzLowerHandguardRailId, StormwerkzLowerHandguardLabel, TacticalSlotName, ZenitRkForegripIds);

            // AA-12 457mm 枪管的 mod_mount 导轨槽：追加 M60 脚架。
            AddItemIdsToSlot(items, Aa12Barrel457Id, Aa12Barrel457Label, MountSlotName, M60BipodId);

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
