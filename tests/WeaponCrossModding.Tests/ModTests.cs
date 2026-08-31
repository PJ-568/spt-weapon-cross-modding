using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using SPTarkov.Common.Models.Logging;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Tables;

namespace Pj568.WeaponCrossModding.Tests;

/// <summary>
/// 针对 <see cref="WeaponCrossModdingPlugin"/> 的单元测试：
/// 用最小的物品/槽位夹具直接驱动 <c>OnLoadAsync</c>，断言目标槽 Filter 的注入与幂等。
/// 物品 id 与 src/Mod.cs 中的常量保持一致；若上游数据 id 变更，这里会先失败。
/// </summary>
public class WeaponCrossModdingPluginTests
{
    private const string Ppsh41Id = "5ea03f7400685063ec28bfa8";
    private const string Ppsh41BarrelId = "5ea02bb600685063ec28bfa1";
    private const string StormwerkzRailId = "6698c90829e062525d0ad8ad";
    private const string SpecterDrId = "57ac965c24597706be5f975c";
    private const string SpecterDrFdeId = "57aca93d2459771f2c7e26db";
    private const string Bravo4Id = "57adff4f24597737f373b6e6";
    private const string Cr200DsId = "624c2e8614da335f1e034d8c";
    private const string Cr50DsId = "61a4c8884f95bc3b2c5dc96f";
    private const string Mp18Id = "61f804acfcba9556ea304cb8";
    private const string KacMwsBipodAdapterId = "676175bb48fa5c377e06fc36";
    private const string Bt10AtlasBipodId = "6644920d49817dc7d505ca71";

    private static readonly string[] MosinBarrelIds =
    [
        "5bfd4cc90db834001d23e846",
        "5bfd4cd60db834001c38f095",
        "5bfd4cbe0db834001b73449f",
        "5ae09bff5acfc4001562219d",
    ];

    private const string Ppsh41StockId = "5ea03e9400685063ec28bfa4";

    private static readonly string[] Ppsh41StockCompatIds =
    [
        "6259c3387d6aab70bc23a18d",
        "646371a9f2404ab67905c8e6",
        "6492d7847363b8a52206bc52",
        "6492e3a97df7d749100e29ee",
        "606eef46232e5a31c233d500",
        "5e848dc4e4dbc5266a4ec63d",
        "5addc7ac5acfc400194dbd90", // M14ALCS (MOD-0) 枪托
    ];

    private const string Ppsh41DustCoverId = "5ea03e5009aa976f2e7a514b";
    private const string HuxwrxHxQdId = "6a158e4abf497aade10030e0";
    private const string HuxwrxHxQdTanId = "6a1eb32c6cd328ea90037455";

    private const string AimSportsTriRailId = "5bbdb811d4351e45020113c7";
    private const string StormwerkzLowerHandguardRailId = "66992f7d9950f5f4cd0602a8";

    private const string M14DcsbMountId = "5addbffe5acfc4001714dfac";

    private const string Sv98HeatRibbonId = "56083eab4bdc2d26448b456a";
    private const string FortisShiftForegripId = "59f8a37386f7747af3328f06";
    private const string M14AlcsButtstockId = "5addc7ac5acfc400194dbd90";
    private const string M14AlcsPistolGripId = "5addc7db5acfc4001669f279";

    // 与 src/Mod.cs 对应：M14ALCS (MOD-0) 枪托握把位的新增握把。
    private static readonly string[] M14AlcsPistolGripCompatIds =
    [
        "5b07db875acfc40dc528a5f6", // AR-15 Tactical Dynamics 镂空手枪式握把
        "6984b7d56be2752c150e6895", // Tyrant Designs MOD Chevron 镂空手枪式握把（黑）
        "698dac21772d6f3dc00e4284", // Tyrant Designs MOD Chevron 镂空手枪式握把（黄）
        "6985eb089edef67ade080b72", // Tyrant Designs MOD Chevron 镂空手枪式握把（红）
        "5a69a2ed8dc32e000d46d1f1", // AS VAL Rotor 43 手枪式握把附缓冲管转接器
    ];

    // 与 src/Mod.cs 的 ZenitRkForegripIds 对应。
    private static readonly string[] ZenitRkForegripIds =
    [
        "5c1bc4812e22164bef5cfde7",
        "5c1bc5612e221602b5429350",
        "5c1bc5af2e221602b412949b",
        "5c1bc5fb2e221602b1779b32",
        "5c1bc7432e221602b412949d",
        "5c1bc7752e221602b1779b34",
    ];

    // 与 src/Mod.cs 的 AimSportsTriRailForegripIds 对应（不含 MP9 与 Steyr AUG，也不含 KeyMod / M-LOK 型）。
    private static readonly string[] AimSportsTriRailForegripIds =
    [
        "5c1bc4812e22164bef5cfde7",
        "5c1bc5612e221602b5429350",
        "5c1bc5af2e221602b412949b",
        "5c1bc5fb2e221602b1779b32",
        "5c1bc7432e221602b412949d",
        "5c1bc7752e221602b1779b34",
        "5f6340d3ca442212f4047eb2",
        "5c7fc87d2e221644f31c0298",
        "5c87ca002e221600114cb150",
        "5cda9bcfd7f00c0c0b53e900",
        "558032614bdc2de7118b4585",
        "58c157be86f77403c74b2bb6",
        "58c157c886f774032749fb06",
    ];

    private static WeaponCrossModdingPlugin BuildPlugin(Dictionary<MongoId, TemplateItem> items)
    {
        var logger = new Mock<ISptLogger<WeaponCrossModdingPlugin>>();
        return new WeaponCrossModdingPlugin(logger.Object, CreateTemplateTable(items));
    }

    // TemplateTable（NuGet 4.1.2）带多个 required 成员，测试只关心 Items；
    // 用反射实例化以绕过编译期 required 校验。
    private static TemplateTable CreateTemplateTable(Dictionary<MongoId, TemplateItem> items)
    {
        var table = (TemplateTable)Activator.CreateInstance(typeof(TemplateTable))!;
        typeof(TemplateTable).GetProperty(nameof(TemplateTable.Items))!.SetValue(table, items);
        return table;
    }

    private static TemplateItem ItemWithSlot(string name, string slotName, params string[] initialFilter)
    {
        var filter = new SlotFilter
        {
            Filter = new HashSet<MongoId>(initialFilter.Select(id => new MongoId(id))),
        };
        var slot = new Slot
        {
            Name = slotName,
            Properties = new SlotProperties { Filters = [filter] },
        };
        return new TemplateItem
        {
            Name = name,
            Properties = new TemplateItemProperties { Slots = [slot] },
        };
    }

    private static TemplateItem ItemWithConflicts(string name) => new()
    {
        Name = name,
        Properties = new TemplateItemProperties { ConflictingItems = [] },
    };

    private static HashSet<MongoId> FilterOf(TemplateItem item, string slotName) =>
        item.Properties!.Slots!.Single(s => s.Name == slotName).Properties!.Filters!.Single().Filter!;

    [Fact]
    public async Task AddsAllMosinBarrelsToPpsh41BarrelSlot()
    {
        var ppsh = ItemWithSlot("weapon_zis_ppsh41_762x25", "mod_barrel", Ppsh41BarrelId);
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(Ppsh41Id)] = ppsh };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(ppsh, "mod_barrel");
        Assert.Contains(new MongoId(Ppsh41BarrelId), filter);
        foreach (var id in MosinBarrelIds)
        {
            Assert.Contains(new MongoId(id), filter);
        }

        Assert.Equal(1 + MosinBarrelIds.Length, filter.Count);
    }

    [Fact]
    public async Task IsIdempotentWhenLoadedTwice()
    {
        var ppsh = ItemWithSlot("weapon_zis_ppsh41_762x25", "mod_barrel", Ppsh41BarrelId);
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(Ppsh41Id)] = ppsh };
        var plugin = BuildPlugin(items);

        await plugin.OnLoadAsync(CancellationToken.None);
        await plugin.OnLoadAsync(CancellationToken.None);

        Assert.Equal(1 + MosinBarrelIds.Length, FilterOf(ppsh, "mod_barrel").Count);
    }

    [Fact]
    public async Task AddsScopesToStormwerkzMountScopeSlot()
    {
        var mount = ItemWithSlot("mount_uzi_stormwerkz_top_cover_rail", "mod_scope");
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(StormwerkzRailId)] = mount };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(mount, "mod_scope");
        Assert.Contains(new MongoId(SpecterDrId), filter);
        Assert.Contains(new MongoId(SpecterDrFdeId), filter);
        Assert.Contains(new MongoId(Bravo4Id), filter);
        Assert.Equal(3, filter.Count);
    }

    [Fact]
    public async Task AddsMountsToCr200DsSightSlot()
    {
        var revolver = ItemWithSlot("weapon_chiappa_rhino_200ds_9x19", "mod_sight_front");
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(Cr200DsId)] = revolver };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(revolver, "mod_sight_front");
        Assert.Contains(new MongoId(StormwerkzRailId), filter);
        Assert.Contains(new MongoId(Mp18Id), filter);
        Assert.Equal(2, filter.Count);
    }

    [Fact]
    public async Task AddsMountsToCr50DsSightSlot()
    {
        var revolver = ItemWithSlot("weapon_chiappa_rhino_50ds_9x33R", "mod_sight_front");
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(Cr50DsId)] = revolver };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(revolver, "mod_sight_front");
        Assert.Contains(new MongoId(StormwerkzRailId), filter);
        Assert.Contains(new MongoId(Mp18Id), filter);
        Assert.Equal(2, filter.Count);
    }

    [Fact]
    public async Task AddsPartsToCr50DsTacticalSlot()
    {
        var revolver = ItemWithSlot("weapon_chiappa_rhino_50ds_9x33R", "mod_tactical");
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(Cr50DsId)] = revolver };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(revolver, "mod_tactical");
        foreach (var id in ZenitRkForegripIds)
        {
            Assert.Contains(new MongoId(id), filter);
        }

        Assert.Contains(new MongoId(KacMwsBipodAdapterId), filter);
        Assert.Contains(new MongoId(Bt10AtlasBipodId), filter);
        Assert.Equal(ZenitRkForegripIds.Length + 2, filter.Count);
    }

    [Fact]
    public async Task AddsStocksToPpsh41StockSlot()
    {
        var ppsh = ItemWithSlot("weapon_zis_ppsh41_762x25", "mod_stock", Ppsh41StockId);
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(Ppsh41Id)] = ppsh };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(ppsh, "mod_stock");
        Assert.Contains(new MongoId(Ppsh41StockId), filter);
        foreach (var id in Ppsh41StockCompatIds)
        {
            Assert.Contains(new MongoId(id), filter);
        }

        Assert.Equal(1 + Ppsh41StockCompatIds.Length, filter.Count);
    }

    [Fact]
    public async Task MakesPpsh41DustCoverConflictWithHuxwrxSuppressors()
    {
        var dustCover = ItemWithConflicts("PPSH-41 dust cover");
        var black = ItemWithConflicts("HUXWRX HX-QD 7.62x51 sound suppressor");
        var tan = ItemWithConflicts("HUXWRX HX-QD 7.62x51 sound suppressor (Tan)");
        var items = new Dictionary<MongoId, TemplateItem>
        {
            [new MongoId(Ppsh41DustCoverId)] = dustCover,
            [new MongoId(HuxwrxHxQdId)] = black,
            [new MongoId(HuxwrxHxQdTanId)] = tan,
        };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        Assert.Contains(new MongoId(HuxwrxHxQdId), dustCover.Properties!.ConflictingItems!);
        Assert.Contains(new MongoId(HuxwrxHxQdTanId), dustCover.Properties!.ConflictingItems!);
        Assert.Contains(new MongoId(Ppsh41DustCoverId), black.Properties!.ConflictingItems!);
        Assert.Contains(new MongoId(Ppsh41DustCoverId), tan.Properties!.ConflictingItems!);
    }

    [Fact]
    public async Task AddsPartsToAimSportsTriRailFirstTacticalSlot()
    {
        var rail = ItemWithSlot("mount_mosin_aim_sports_tri_rail", "mod_tactical_000");
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(AimSportsTriRailId)] = rail };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(rail, "mod_tactical_000");
        Assert.Equal(AimSportsTriRailForegripIds.Length + 2, filter.Count);
        foreach (var id in AimSportsTriRailForegripIds)
        {
            Assert.Contains(new MongoId(id), filter);
        }

        Assert.Contains(new MongoId(Sv98HeatRibbonId), filter);
        Assert.Contains(new MongoId(FortisShiftForegripId), filter);
    }

    [Fact]
    public async Task AddsPartsToAimSportsTriRailSecondTacticalSlot()
    {
        var rail = ItemWithSlot("mount_mosin_aim_sports_tri_rail", "mod_tactical_001");
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(AimSportsTriRailId)] = rail };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(rail, "mod_tactical_001");
        Assert.Contains(new MongoId(Sv98HeatRibbonId), filter);
        Assert.Single(filter);
    }

    [Fact]
    public async Task AddsGripsToM14AlcsButtstockPistolGripSlot()
    {
        var buttstock = ItemWithSlot("stock_m14_sage_ebr_m14alcs_buttstock", "mod_pistol_grip", M14AlcsPistolGripId);
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(M14AlcsButtstockId)] = buttstock };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(buttstock, "mod_pistol_grip");
        Assert.Contains(new MongoId(M14AlcsPistolGripId), filter);
        foreach (var id in M14AlcsPistolGripCompatIds)
        {
            Assert.Contains(new MongoId(id), filter);
        }

        Assert.Equal(1 + M14AlcsPistolGripCompatIds.Length, filter.Count);
    }

    [Fact]
    public async Task AddsZenitRkForegripsToStormwerkzLowerHandguardRail()
    {
        var rail = ItemWithSlot("handguard_uzi_stormwerkz_lower_rail", "mod_tactical");
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(StormwerkzLowerHandguardRailId)] = rail };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(rail, "mod_tactical");
        Assert.Equal(ZenitRkForegripIds.Length, filter.Count);
        foreach (var id in ZenitRkForegripIds)
        {
            Assert.Contains(new MongoId(id), filter);
        }
    }

    [Fact]
    public async Task AddsM14DcsbMountToTriRailScopeSlot()
    {
        var rail = ItemWithSlot("mount_mosin_aim_sports_tri_rail", "mod_scope");
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(AimSportsTriRailId)] = rail };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(rail, "mod_scope");
        Assert.Single(filter);
        Assert.Contains(new MongoId(M14DcsbMountId), filter);
    }

    [Fact]
    public async Task MissingItemsDoNotThrow()
    {
        await BuildPlugin(new Dictionary<MongoId, TemplateItem>()).OnLoadAsync(CancellationToken.None);
    }
}
