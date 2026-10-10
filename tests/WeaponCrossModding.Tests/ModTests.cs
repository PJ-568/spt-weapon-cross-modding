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

    // 与 src/Mod.cs 对应：可安装于 PPSh-41 枪管槽的 M700 枪管与 ORSIS T-5000M 枪管。
    private static readonly string[] M700BarrelIds =
    [
        "5bfebc250db834001a6694e1", // 26 英寸
        "5bfebc320db8340019668d79", // 20 英寸螺纹
        "5d2702e88abbc31ed91efc44", // 26 英寸不锈钢
        "5d2703038abbc3105103d94c", // 20 英寸不锈钢螺纹
    ];

    private const string T5000Barrel660Id = "5df256570dee1b22f862e9c4";

    // RPD 520mm 枪管（mod_muzzle 槽宿主）、其初始枪口配件，以及 AKM PBS-1 消音器。
    private const string RpdBarrel520Id = "6513eff1e06849f06c0957d4";
    private const string Rpd520MuzzleExistingId = "6513f0f5e63f29908d0ffab8";
    private const string AkmPbs1Id = "5a0d63621526d8dba31fe3bf";

    // SKS / OP-SKS 枪管（PPSh-41 枪管槽）。
    private const string SksBarrel520Id = "634f02331f9f536910079b51";
    private const string OpsksBarrel520Id = "634eff66517ccc8a960fc735";

    // SKS 枪身套件（PPSh-41 机匣槽）及其枪托槽配件。
    private const string TapcoIntrafuseSksId = "5afd7ded5acfc40017541f5e";
    private const string FabDefenceUasSksId = "5d0236dad7ad1a0940739d29";
    private const string TapcoIntrafuseBufferTubeId = "5afd7e095acfc40017541f61";
    private const string FabDefenceUasFoldingStockId = "653ed132896b99b40a0292e6";

    // Zveno PK 缓冲管转接器（PPSh-41 枪托槽）与 PPSh-41 71 发弹鼓。
    private const string ZvenoBufferTubeId = "6a182c39b913af92800d8b5d";
    private const string Ppsh71DrumMagId = "5ea034f65aad6446a939737e";

    // SKS / OP-SKS 照门固定环、TKPD 导轨防尘盖，以及 SKS / OP-SKS 枪本体。
    private const string SksRearSightBlockId = "634f04d82e5def262d0b30c6";
    private const string OpsksRearSightBlockId = "634f05a21f9f536910079b56";
    private const string TkpdRailedDustCoverId = "68aee8f8130c00663d08aeb3";
    private const string SksWeaponId = "574d967124597745970e7c94";
    private const string OpsksWeaponId = "587e02ff24597743df3deaeb";

    private const string Svt40Barrel625Id = "6410758c857473525b08bb77";
    private const string Svt40WeaponId = "643ea5b23db6f9f57107d9fd";
    private const string Avt40WeaponId = "6410733d5dd49d77bd07847e";
    private const string Svt40RearSightId = "64119d90dcf48d656f0aa275";

    private const string Svt40MuzzleStdId = "64119d1f2c6d6f921a0929f8";
    private const string Svt40FrontSightId = "64119d672c6d6f921a0929fb";

    private const string MosinBarrel200Id = "5bfd4cc90db834001d23e846";
    private const string MosinBarrel220ThreadedId = "5bfd4cd60db834001c38f095";
    private const string MosinBarrel514Id = "5bfd4cbe0db834001b73449f";
    private const string SvdsRearSightBlockId = "5c471c2d2e22164bef5d077f";
    private const string SvdsWeaponId = "5c46fbd72e2216398b5a8c9c";
    private const string SvdsBarrelId = "5c471cb32e221602b177afaa";
    private const string SvdsHandguardStdId = "5c471c6c2e221602b66cd9ae";
    private const string SvdsCaaXrsDrgId = "5e5699df2161e06ac158df6f";
    private const string SvdsModernizedKitId = "5e56991336989c75ab4f03f6";
    private const string SvdsRearSightId = "5c471b7e2e2216152006e46c";

    private const string SksGasCoverOpsksStdId = "634f03d40384a3ba4f06f874";
    private const string SksGasCoverSksWoodId = "634f08a21f9f536910079b5a";
    private const string SksGasCoverTapcoId = "653ecd065a1690d9d90491e6";
    private const string SksGasCoverFabId = "653ece125a1690d9d90491e8";
    private const string SksGasCoverAtiId = "653ecc425a1690d9d90491e4";
    private static readonly string[] SksGasCoverIds =
    [
        SksGasCoverOpsksStdId,
        SksGasCoverSksWoodId,
        SksGasCoverTapcoId,
        SksGasCoverFabId,
        SksGasCoverAtiId,
    ];

    private const string UltimakM1BId = "59ccfdba86f7747f2109a587";
    private static readonly string[] SvdsGasTubeItemIds =
    [
        SksGasCoverOpsksStdId,
        SksGasCoverSksWoodId,
        SksGasCoverAtiId,
        UltimakM1BId,
    ];

    private static readonly string[] ExclusiveGasTubeItemIds =
    [
        .. SksGasCoverIds,
        UltimakM1BId,
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
        "6a182c39b913af92800d8b5d", // Zveno PK 缓冲管转接器
    ];

    private const string Ppsh41DustCoverId = "5ea03e5009aa976f2e7a514b";
    private const string HuxwrxHxQdId = "6a158e4abf497aade10030e0";
    private const string HuxwrxHxQdTanId = "6a1eb32c6cd328ea90037455";

    private const string AimSportsTriRailId = "5bbdb811d4351e45020113c7";
    private const string StormwerkzLowerHandguardRailId = "66992f7d9950f5f4cd0602a8";

    private const string M14DcsbMountId = "5addbffe5acfc4001714dfac";
    private const string AlphaDogSuppressorId = "5a33a8ebc4a282000c5a950d";

    private const string Sv98HeatRibbonId = "56083eab4bdc2d26448b456a";
    private const string FortisShiftForegripId = "59f8a37386f7747af3328f06";
    private const string M14AlcsButtstockId = "5addc7ac5acfc400194dbd90";
    private const string M14AlcsPistolGripId = "5addc7db5acfc4001669f279";
    private const string MosinBarrel730Id = "5ae09bff5acfc4001562219d";
    private const string MosinFrontSightId = "5ae099875acfc4001714e593";
    private const string AlxBipod16Id = "680f6d9a4d7624d36e06527b";
    private const string AlxBipod20Id = "680f7e4aeee716732708e84e";

    private const string Aa12457BarrelId = "670fced86a7e274b1a0964e8";
    private const string Aa12MountExistingId = "6710cea62bb09af72f0e6bf8";
    private const string M60BipodId = "66012d9a3dff5074ed002e33";
    private const string Mp18RifleId = "61f7c9e189e6fb1a5e3ea78d";
    private const string Mp18StockExistingAId = "61f803b8ced75b2e852e35f8";
    private const string Mp18StockExistingBId = "61f7b234ea4ab34f2f59c3ec";

    // MTs-255-12（护木槽与枪托槽宿主）及其原装护木 / 枪托；MP-18 全部护木；雷明顿 Model 870 全部枪托。
    private const string Mts255Id = "60db29ce99594040e04c4a27";
    private const string Mts255HandguardExistingId = "6123649463849f3d843da7c4";
    private const string Mts255StockExistingId = "612781056f3d944a17348d60";
    private const string Mp18HandguardWoodId = "61f7b85367ddd414173fdb36";
    private const string Mp18HandguardPlasticId = "61f8024263dc1250e26eb029";
    private static readonly string[] M870StockIds =
    [
        "5a78813bc5856700186c4abe", // Magpul SGA
        "5a7880d0c5856700142fdd9d", // Remington SPS
        "5a788169c5856700142fdd9e", // Shockwave Technologies Raptor grip
    ];
    private const string MarlinMxlrId = "67c6de3ce39861860909e8e5";
    private const string EmtiId = "5dfe14f30b92095fd441edaf";
    private const string MxlrMagId = "67c5424826265106dd0697a4";
    private const string Ks23MetalStockId = "5e848dc4e4dbc5266a4ec63d";

    // 20x1mm 玩具枪、其原装弹匣与玩具弹，以及 7.62x25 托卡列夫全部 7 种弹药。
    private const string ToyGunId = "66015072e9f84d5680039678";
    private const string ToyGunMagId = "66015dc4aaad2f54cb04c56a";
    private const string ToyAmmoId = "6601546f86889319850bd566";
    private static readonly string[] Caliber762x25AmmoIds =
    [
        "5735fdcd2459776445391d61", // AKBS
        "5735ff5c245977640e39ba7e", // FMJ43
        "573601b42459776410737435", // LRN
        "573602322459776445391df1", // LRNPC
        "5736026a245977644601dc61", // P Gl
        "573603562459776430731618", // Pst gzh
        "573603c924597764442bd9cb", // T Gzh
    ];

    // Marlin MXLR .308 ME 杠杆步枪 mod_stock 槽的真实初始 filter。
    private static readonly string[] MarlinMxlrStockIds =
    [
        "67c541ba5b84f7f36c03e555",
        "67c541ca26265106dd0697a0",
        "67c541d45b84f7f36c03e557",
        "67ff1f9f32abb9a4280b5178",
        "67ff1fa88e8db1dcb80ccada",
        "67ff2c16c593c7b94a095e56",
    ];

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

    // 构造带膛室（Chambers）或弹匣装填位（Cartridges）的物品；二者结构均为 Slot 过滤器列表。
    private static TemplateItem ItemWithAmmoContainer(string name, bool isChamber, params string[] initialFilter)
    {
        var filter = new SlotFilter
        {
            Filter = new HashSet<MongoId>(initialFilter.Select(id => new MongoId(id))),
        };
        var container = new Slot
        {
            Name = isChamber ? "patron_in_weapon" : "cartridges",
            Properties = new SlotProperties { Filters = [filter] },
        };
        return new TemplateItem
        {
            Name = name,
            Properties = isChamber
                ? new TemplateItemProperties { Chambers = [container] }
                : new TemplateItemProperties { Cartridges = [container] },
        };
    }

    private static HashSet<MongoId> AmmoFilterOf(TemplateItem item, bool isChamber) =>
        (isChamber ? item.Properties!.Chambers! : item.Properties!.Cartridges!)
            .Single().Properties!.Filters!.Single().Filter!;

    private static HashSet<MongoId> FilterOf(TemplateItem item, string slotName) =>
        item.Properties!.Slots!.Single(s => s.Name == slotName).Properties!.Filters!.Single().Filter!;

    [Fact]
    public async Task AddsExpectedBarrelsAndSvdsBlockToPpsh41BarrelSlot()
    {
        var ppsh = ItemWithSlot("weapon_zis_ppsh41_762x25", "mod_barrel", Ppsh41BarrelId);
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(Ppsh41Id)] = ppsh };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(ppsh, "mod_barrel");
        Assert.Contains(new MongoId(Ppsh41BarrelId), filter);
        // 仍保留在 PPSh 的莫辛枪管（200mm 已移至 SVDS 照门固定环护木槽）。
        Assert.Contains(new MongoId(MosinBarrel220ThreadedId), filter);
        Assert.Contains(new MongoId(MosinBarrel514Id), filter);
        Assert.Contains(new MongoId(MosinBarrel730Id), filter);
        Assert.Contains(new MongoId(SksBarrel520Id), filter);
        Assert.Contains(new MongoId(OpsksBarrel520Id), filter);
        Assert.Contains(new MongoId(Svt40Barrel625Id), filter);
        Assert.Contains(new MongoId(SvdsRearSightBlockId), filter);

        // 已移至 SVDS 照门固定环护木槽，不应再出现在 PPSh 枪管槽。
        Assert.DoesNotContain(new MongoId(MosinBarrel200Id), filter);
        foreach (var id in M700BarrelIds)
        {
            Assert.DoesNotContain(new MongoId(id), filter);
        }
        Assert.DoesNotContain(new MongoId(T5000Barrel660Id), filter);

        Assert.Equal(1 + 3 + 2 + 1 + 1, filter.Count);
    }

    [Fact]
    public async Task IsIdempotentWhenLoadedTwice()
    {
        var ppsh = ItemWithSlot("weapon_zis_ppsh41_762x25", "mod_barrel", Ppsh41BarrelId);
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(Ppsh41Id)] = ppsh };
        var plugin = BuildPlugin(items);

        await plugin.OnLoadAsync(CancellationToken.None);
        await plugin.OnLoadAsync(CancellationToken.None);

        Assert.Equal(1 + 3 + 2 + 1 + 1, FilterOf(ppsh, "mod_barrel").Count);
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
    public async Task AddsAlxBipodsToMosin730mmBarrelSightSlot()
    {
        var barrel = ItemWithSlot("barrel_mosin_izhmash_mosin_std_730mm", "mod_sight_front", MosinFrontSightId);
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(MosinBarrel730Id)] = barrel };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(barrel, "mod_sight_front");
        Assert.Contains(new MongoId(MosinFrontSightId), filter);
        Assert.Contains(new MongoId(AlxBipod16Id), filter);
        Assert.Contains(new MongoId(AlxBipod20Id), filter);
        Assert.Equal(3, filter.Count);
    }

    [Fact]
    public async Task AddsAlxBipodsToSvt40MuzzleSightSlot()
    {
        var muzzle = ItemWithSlot("muzzle_svt40_toz_std_762x54r", "mod_sight_front", Svt40FrontSightId);
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(Svt40MuzzleStdId)] = muzzle };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(muzzle, "mod_sight_front");
        Assert.Contains(new MongoId(Svt40FrontSightId), filter);
        Assert.Contains(new MongoId(AlxBipod16Id), filter);
        Assert.Contains(new MongoId(AlxBipod20Id), filter);
        Assert.Equal(3, filter.Count);
    }

    [Fact]
    public async Task AddsAlxBipod20ToFabDefenceUasLowerRail()
    {
        var kit = ItemWithSlot("stock_sks_fab_defence_uas_sks", "mod_tactical_002", Bt10AtlasBipodId);
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(FabDefenceUasSksId)] = kit };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(kit, "mod_tactical_002");
        Assert.Contains(new MongoId(Bt10AtlasBipodId), filter);
        Assert.Contains(new MongoId(AlxBipod20Id), filter);
        Assert.DoesNotContain(new MongoId(AlxBipod16Id), filter);
        // 2 个原有项目 (BT10 Atlas 脚架 + ALX 20 脚架) + 32 个 AK-100 聚合物护木兼容前握把
        Assert.Equal(2 + 32, filter.Count);
        // 验证部分 AK-100 前握把已注入
        Assert.Contains(new MongoId("5c7fc87d2e221644f31c0298"), filter); // BCM GUNFIGHTER MOD 3 vertical
        Assert.Contains(new MongoId("5c1bc4812e22164bef5cfde7"), filter); // Zenit RK-0
        Assert.Contains(new MongoId("5f6340d3ca442212f4047eb2"), filter); // Tactical Dynamics 镂空前握把
        Assert.Contains(new MongoId("558032614bdc2de7118b4585"), filter); // TangoDown Stubby BGV-MK46K (Black)
    }

    [Fact]
    public async Task AddsAlxBipod20ToUltimakM1BTacticalSlot()
    {
        var ultimak = ItemWithSlot("gas_block_ak_ultimak_m1b", "mod_tactical_000");
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(UltimakM1BId)] = ultimak };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(ultimak, "mod_tactical_000");
        Assert.Equal(new MongoId(AlxBipod20Id), Assert.Single(filter));
    }

    [Fact]
    public async Task AddsMovedBarrelsAndSvdsBarrelToSvdsRearSightBlockHandguardSlot()
    {
        var block = ItemWithSlot("mount_svd_izhmash_svd_s_lower_band_std", "mod_handguard", SvdsHandguardStdId);
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(SvdsRearSightBlockId)] = block };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(block, "mod_handguard");
        Assert.Contains(new MongoId(SvdsHandguardStdId), filter);
        Assert.Contains(new MongoId(MosinBarrel200Id), filter);
        foreach (var id in M700BarrelIds)
        {
            Assert.Contains(new MongoId(id), filter);
        }
        Assert.Contains(new MongoId(T5000Barrel660Id), filter);
        Assert.Contains(new MongoId(SvdsBarrelId), filter);
        Assert.Contains(new MongoId(Svt40Barrel625Id), filter);
        Assert.Contains(new MongoId(SvdsRearSightBlockId), filter);
        Assert.Equal(1 + 1 + M700BarrelIds.Length + 1 + 1 + 1 + 1, filter.Count);
    }

    [Fact]
    public async Task AddsTkpdDustCoverToSvdsRearSightBlockSightSlot()
    {
        var block = ItemWithSlot("mount_svd_izhmash_svd_s_lower_band_std", "mod_sight_rear", SvdsRearSightId);
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(SvdsRearSightBlockId)] = block };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(block, "mod_sight_rear");
        Assert.Contains(new MongoId(SvdsRearSightId), filter);
        Assert.Contains(new MongoId(TkpdRailedDustCoverId), filter);
        foreach (string id in SvdsGasTubeItemIds)
        {
            Assert.Contains(new MongoId(id), filter);
        }
        // TAPCO 与 Fab Defence UAS 防尘盖已从该槽回退。
        Assert.DoesNotContain(new MongoId(SksGasCoverTapcoId), filter);
        Assert.DoesNotContain(new MongoId(SksGasCoverFabId), filter);
        Assert.Equal(2 + SvdsGasTubeItemIds.Length, filter.Count);
    }

    [Fact]
    public async Task MakesSvdsMovedBarrelsConflictWithSvdsWeapon()
    {
        var svds = ItemWithConflicts("SVDS");
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(SvdsWeaponId)] = svds };
        string[] barrelIds = [MosinBarrel200Id, .. M700BarrelIds, T5000Barrel660Id];
        foreach (string id in barrelIds)
        {
            items[new MongoId(id)] = ItemWithConflicts("moved barrel");
        }

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        foreach (string id in barrelIds)
        {
            Assert.Contains(new MongoId(id), svds.Properties!.ConflictingItems!);
            Assert.Contains(new MongoId(SvdsWeaponId), items[new MongoId(id)].Properties!.ConflictingItems!);
        }
    }

    [Fact]
    public async Task MakesTkpdDustCoverConflictWithSvdsWeapon()
    {
        var tkpdCover = ItemWithConflicts("TKPD railed dust cover");
        var svds = ItemWithConflicts("SVDS");
        var items = new Dictionary<MongoId, TemplateItem>
        {
            [new MongoId(TkpdRailedDustCoverId)] = tkpdCover,
            [new MongoId(SvdsWeaponId)] = svds,
        };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        Assert.Contains(new MongoId(SvdsWeaponId), tkpdCover.Properties!.ConflictingItems!);
        Assert.Contains(new MongoId(TkpdRailedDustCoverId), svds.Properties!.ConflictingItems!);
    }

    [Fact]
    public async Task MakesSvdsBarrelConflictWithItself()
    {
        var barrel = ItemWithConflicts("SVDS barrel");
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(SvdsBarrelId)] = barrel };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        Assert.Contains(new MongoId(SvdsBarrelId), barrel.Properties!.ConflictingItems!);
    }

    [Fact]
    public async Task MakesSvdsRearSightBlockConflictWithPpshDustCover()
    {
        var block = ItemWithConflicts("SVDS rear sight block");
        var dustCover = ItemWithConflicts("PPSh-41 dust cover");
        var items = new Dictionary<MongoId, TemplateItem>
        {
            [new MongoId(SvdsRearSightBlockId)] = block,
            [new MongoId(Ppsh41DustCoverId)] = dustCover,
        };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        Assert.Contains(new MongoId(Ppsh41DustCoverId), block.Properties!.ConflictingItems!);
        Assert.Contains(new MongoId(SvdsRearSightBlockId), dustCover.Properties!.ConflictingItems!);
    }

    [Fact]
    public async Task MakesSvdsRearSightBlockConflictWithTapcoIntrafuseKit()
    {
        var block = ItemWithConflicts("SVDS rear sight block");
        var tapco = ItemWithConflicts("TAPCO Intrafuse SKS");
        var items = new Dictionary<MongoId, TemplateItem>
        {
            [new MongoId(SvdsRearSightBlockId)] = block,
            [new MongoId(TapcoIntrafuseSksId)] = tapco,
        };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        Assert.Contains(new MongoId(TapcoIntrafuseSksId), block.Properties!.ConflictingItems!);
        Assert.Contains(new MongoId(SvdsRearSightBlockId), tapco.Properties!.ConflictingItems!);
    }

    [Fact]
    public async Task MakesSvdsHandguardsConflictWithPpsh41Weapon()
    {
        var ppsh = ItemWithConflicts("PPSh-41");
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(Ppsh41Id)] = ppsh };
        string[] handguardIds = [SvdsCaaXrsDrgId, SvdsModernizedKitId, SvdsHandguardStdId];
        foreach (string id in handguardIds)
        {
            items[new MongoId(id)] = ItemWithConflicts("SVDS handguard");
        }

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        foreach (string id in handguardIds)
        {
            Assert.Contains(new MongoId(id), ppsh.Properties!.ConflictingItems!);
            Assert.Contains(new MongoId(Ppsh41Id), items[new MongoId(id)].Properties!.ConflictingItems!);
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
    public async Task AddsTkpdDustCoverToSksRearSightBlockSightSlot()
    {
        var sksBlock = ItemWithSlot("mount_sks_toz_sks_rear_sight_block", "mod_sight_rear");
        var opsksBlock = ItemWithSlot("mount_sks_molot_sks_rear_sight_block", "mod_sight_rear");
        var items = new Dictionary<MongoId, TemplateItem>
        {
            [new MongoId(SksRearSightBlockId)] = sksBlock,
            [new MongoId(OpsksRearSightBlockId)] = opsksBlock,
        };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        foreach (var block in new[] { sksBlock, opsksBlock })
        {
            Assert.Contains(new MongoId(TkpdRailedDustCoverId), FilterOf(block, "mod_sight_rear"));
            Assert.Single(FilterOf(block, "mod_sight_rear"));
        }
    }

    [Fact]
    public async Task AddsM60BipodToAa12457BarrelMountSlot()
    {
        var barrel = ItemWithSlot("barrel_aa12_457mm", "mod_mount", Aa12MountExistingId);
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(Aa12457BarrelId)] = barrel };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(barrel, "mod_mount");
        Assert.Contains(new MongoId(Aa12MountExistingId), filter);
        Assert.Contains(new MongoId(M60BipodId), filter);
        Assert.Equal(2, filter.Count);
    }

    [Fact]
    public async Task AddsKs23StockToMp18StockSlot()
    {
        var rifle = ItemWithSlot("weapon_mp18_762x54r", "mod_stock", Mp18StockExistingAId, Mp18StockExistingBId);
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(Mp18RifleId)] = rifle };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(rifle, "mod_stock");
        Assert.Contains(new MongoId(Mp18StockExistingAId), filter);
        Assert.Contains(new MongoId(Mp18StockExistingBId), filter);
        Assert.Contains(new MongoId(Ks23MetalStockId), filter);
        Assert.Equal(3, filter.Count);
    }

    [Fact]
    public async Task AddsKs23StockToMarlinMxlrStockSlot()
    {
        var rifle = ItemWithSlot("weapon_marlin_mxlr_308", "mod_stock", MarlinMxlrStockIds);
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(MarlinMxlrId)] = rifle };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(rifle, "mod_stock");
        foreach (var id in MarlinMxlrStockIds)
        {
            Assert.Contains(new MongoId(id), filter);
        }

        Assert.Contains(new MongoId(Ks23MetalStockId), filter);
        Assert.Equal(MarlinMxlrStockIds.Length + 1, filter.Count);
    }

    [Fact]
    public async Task AddsAkmPbs1ToRpd520BarrelMuzzleSlot()
    {
        var barrel = ItemWithSlot("barrel_rpd_zid_rpd_520mm_762x39", "mod_muzzle", Rpd520MuzzleExistingId);
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(RpdBarrel520Id)] = barrel };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(barrel, "mod_muzzle");
        Assert.Contains(new MongoId(Rpd520MuzzleExistingId), filter);
        Assert.Contains(new MongoId(AkmPbs1Id), filter);
        Assert.Equal(2, filter.Count);
    }

    [Fact]
    public async Task AddsPartsToPpsh41ReceiverSlot()
    {
        var ppsh = ItemWithSlot("weapon_zis_ppsh41_762x25", "mod_reciever", Ppsh41DustCoverId);
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(Ppsh41Id)] = ppsh };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(ppsh, "mod_reciever");
        Assert.Contains(new MongoId(Ppsh41DustCoverId), filter);
        Assert.Contains(new MongoId(TapcoIntrafuseSksId), filter);
        Assert.Contains(new MongoId(FabDefenceUasSksId), filter);
        Assert.Equal(3, filter.Count);
    }

    [Fact]
    public async Task MakesSksChassisKitsConflictWithPpsh71Drum()
    {
        var tapco = ItemWithConflicts("TAPCO Intrafuse SKS");
        var fab = ItemWithConflicts("Fab Defence UAS SKS");
        var drum = ItemWithConflicts("PPSh-41 71-round drum");
        var items = new Dictionary<MongoId, TemplateItem>
        {
            [new MongoId(TapcoIntrafuseSksId)] = tapco,
            [new MongoId(FabDefenceUasSksId)] = fab,
            [new MongoId(Ppsh71DrumMagId)] = drum,
        };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        foreach (var chassis in new[] { tapco, fab })
        {
            Assert.Contains(new MongoId(Ppsh71DrumMagId), chassis.Properties!.ConflictingItems!);
        }

        Assert.Contains(new MongoId(TapcoIntrafuseSksId), drum.Properties!.ConflictingItems!);
        Assert.Contains(new MongoId(FabDefenceUasSksId), drum.Properties!.ConflictingItems!);
    }

    [Fact]
    public async Task MakesTkpdDustCoverConflictWithSksAndOpsksWeapons()
    {
        var tkpdCover = ItemWithConflicts("TKPD railed dust cover");
        var sks = ItemWithConflicts("SKS");
        var opsks = ItemWithConflicts("OP-SKS");
        var items = new Dictionary<MongoId, TemplateItem>
        {
            [new MongoId(TkpdRailedDustCoverId)] = tkpdCover,
            [new MongoId(SksWeaponId)] = sks,
            [new MongoId(OpsksWeaponId)] = opsks,
        };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        Assert.Contains(new MongoId(SksWeaponId), tkpdCover.Properties!.ConflictingItems!);
        Assert.Contains(new MongoId(OpsksWeaponId), tkpdCover.Properties!.ConflictingItems!);
        Assert.Contains(new MongoId(TkpdRailedDustCoverId), sks.Properties!.ConflictingItems!);
        Assert.Contains(new MongoId(TkpdRailedDustCoverId), opsks.Properties!.ConflictingItems!);
    }

    [Fact]
    public async Task AddsTkpdDustCoverToSvt40BarrelSightSlot()
    {
        var barrel = ItemWithSlot("barrel_svt40_toz_625mm_762x54r", "mod_sight_rear", Svt40RearSightId);
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(Svt40Barrel625Id)] = barrel };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(barrel, "mod_sight_rear");
        Assert.Contains(new MongoId(Svt40RearSightId), filter);
        Assert.Contains(new MongoId(TkpdRailedDustCoverId), filter);
        foreach (string id in SksGasCoverIds)
        {
            Assert.Contains(new MongoId(id), filter);
        }
        Assert.Equal(2 + SksGasCoverIds.Length, filter.Count);
    }

    [Fact]
    public async Task MakesTkpdDustCoverConflictWithSvtAndAvtWeapons()
    {
        var tkpdCover = ItemWithConflicts("TKPD railed dust cover");
        var svt = ItemWithConflicts("SVT-40");
        var avt = ItemWithConflicts("AVT-40");
        var items = new Dictionary<MongoId, TemplateItem>
        {
            [new MongoId(TkpdRailedDustCoverId)] = tkpdCover,
            [new MongoId(Svt40WeaponId)] = svt,
            [new MongoId(Avt40WeaponId)] = avt,
        };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        Assert.Contains(new MongoId(Svt40WeaponId), tkpdCover.Properties!.ConflictingItems!);
        Assert.Contains(new MongoId(Avt40WeaponId), tkpdCover.Properties!.ConflictingItems!);
        Assert.Contains(new MongoId(TkpdRailedDustCoverId), svt.Properties!.ConflictingItems!);
        Assert.Contains(new MongoId(TkpdRailedDustCoverId), avt.Properties!.ConflictingItems!);
    }

    [Fact]
    public async Task MakesTkpdDustCoverConflictWithItself()
    {
        var tkpdCover = ItemWithConflicts("TKPD railed dust cover");
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(TkpdRailedDustCoverId)] = tkpdCover };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        Assert.Contains(new MongoId(TkpdRailedDustCoverId), tkpdCover.Properties!.ConflictingItems!);
    }

    [Fact]
    public async Task MakesSvt40BarrelConflictWithSvdsWeapon()
    {
        var svtBarrel = ItemWithConflicts("SVT-40 625mm barrel");
        var svds = ItemWithConflicts("SVDS");
        var items = new Dictionary<MongoId, TemplateItem>
        {
            [new MongoId(Svt40Barrel625Id)] = svtBarrel,
            [new MongoId(SvdsWeaponId)] = svds,
        };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        Assert.Contains(new MongoId(SvdsWeaponId), svtBarrel.Properties!.ConflictingItems!);
        Assert.Contains(new MongoId(Svt40Barrel625Id), svds.Properties!.ConflictingItems!);
    }

    [Fact]
    public async Task MakesAlxBipod20ConflictWithItself()
    {
        var alxBipod20 = ItemWithConflicts("MDR BLK LBL ALX 20 bipod");
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(AlxBipod20Id)] = alxBipod20 };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        Assert.Contains(new MongoId(AlxBipod20Id), alxBipod20.Properties!.ConflictingItems!);
    }

    [Fact]
    public async Task MakesGasTubeItemsMutuallyExclusive()
    {
        var items = new Dictionary<MongoId, TemplateItem>();
        foreach (string id in ExclusiveGasTubeItemIds)
        {
            items[new MongoId(id)] = ItemWithConflicts("gas tube item");
        }

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        foreach (string id in ExclusiveGasTubeItemIds)
        {
            var conflicts = items[new MongoId(id)].Properties!.ConflictingItems!;
            foreach (string other in ExclusiveGasTubeItemIds)
            {
                Assert.Contains(new MongoId(other), conflicts);
            }
        }
    }

    [Fact]
    public async Task MakesSksChassisKitsConflictWithPpsh41Stocks()
    {
        string[] stockIds =
        [
            Ppsh41StockId,
            "6259c3387d6aab70bc23a18d", // Benelli M3 可伸缩枪托
            "646371a9f2404ab67905c8e6", // PKM 木制枪托
            "6492d7847363b8a52206bc52", // Zenit PT-2
            "6492e3a97df7d749100e29ee", // PKP 聚合物枪托
            "606eef46232e5a31c233d500", // Ultima MP-155 握把
            "5addc7ac5acfc400194dbd90", // M14ALCS (MOD-0) 枪托
            "6a182c39b913af92800d8b5d", // Zveno PK 缓冲管转接器
        ];
        var tapco = ItemWithConflicts("TAPCO Intrafuse SKS");
        var fab = ItemWithConflicts("Fab Defence UAS SKS");
        var items = new Dictionary<MongoId, TemplateItem>
        {
            [new MongoId(TapcoIntrafuseSksId)] = tapco,
            [new MongoId(FabDefenceUasSksId)] = fab,
        };
        foreach (string id in stockIds)
        {
            items[new MongoId(id)] = ItemWithConflicts("PPSh-41 stock");
        }

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        foreach (var chassis in new[] { tapco, fab })
        {
            foreach (string id in stockIds)
            {
                Assert.Contains(new MongoId(id), chassis.Properties!.ConflictingItems!);
            }
        }

        foreach (string id in stockIds)
        {
            Assert.Contains(new MongoId(TapcoIntrafuseSksId), items[new MongoId(id)].Properties!.ConflictingItems!);
            Assert.Contains(new MongoId(FabDefenceUasSksId), items[new MongoId(id)].Properties!.ConflictingItems!);
        }
    }

    [Fact]
    public async Task MakesKs23StockConflictWithSksChassisStockParts()
    {
        var ks23 = ItemWithConflicts("KS-23 metal stock");
        var tapcoTube = ItemWithConflicts("TAPCO Intrafuse buffer tube");
        var fabStock = ItemWithConflicts("Fab Defence UAS folding stock");
        var items = new Dictionary<MongoId, TemplateItem>
        {
            [new MongoId(Ks23MetalStockId)] = ks23,
            [new MongoId(TapcoIntrafuseBufferTubeId)] = tapcoTube,
            [new MongoId(FabDefenceUasFoldingStockId)] = fabStock,
        };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        Assert.Contains(new MongoId(TapcoIntrafuseBufferTubeId), ks23.Properties!.ConflictingItems!);
        Assert.Contains(new MongoId(FabDefenceUasFoldingStockId), ks23.Properties!.ConflictingItems!);
        Assert.Contains(new MongoId(Ks23MetalStockId), tapcoTube.Properties!.ConflictingItems!);
        Assert.Contains(new MongoId(Ks23MetalStockId), fabStock.Properties!.ConflictingItems!);
    }

    [Fact]
    public async Task AddsCaliber762x25ToToyGunChambersKeeping20x1mm()
    {
        var gun = ItemWithAmmoContainer("weapon_ussr_pd_20x1mm", isChamber: true, ToyAmmoId);
        gun.Properties!.AmmoCaliber = "Caliber20x1mm";
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(ToyGunId)] = gun };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = AmmoFilterOf(gun, isChamber: true);
        Assert.Contains(new MongoId(ToyAmmoId), filter);
        foreach (string id in Caliber762x25AmmoIds)
        {
            Assert.Contains(new MongoId(id), filter);
        }
        Assert.Equal(1 + Caliber762x25AmmoIds.Length, filter.Count);

        // 实验性最小改法：不动单值 ammoCaliber，仅补白名单，故 20x1mm 能力保留。
        Assert.Equal("Caliber20x1mm", gun.Properties.AmmoCaliber);
    }

    [Fact]
    public async Task ToyGunMagazineStillOnlyAccepts20x1mm()
    {
        // 玩具枪原装弹匣恢复原样：不再支持 7.62x25，改由 MXLR 弹仓链路供弹。
        var mag = ItemWithAmmoContainer("mag_pd_ussr_toygun_std_20x1mm_18", isChamber: false, ToyAmmoId);
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(ToyGunMagId)] = mag };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = AmmoFilterOf(mag, isChamber: false);
        Assert.Contains(new MongoId(ToyAmmoId), filter);
        foreach (string id in Caliber762x25AmmoIds)
        {
            Assert.DoesNotContain(new MongoId(id), filter);
        }
        Assert.Single(filter);
    }

    [Fact]
    public async Task ToyGunAmmoInjectionIsIdempotent()
    {
        var gun = ItemWithAmmoContainer("weapon_ussr_pd_20x1mm", isChamber: true, ToyAmmoId);
        var mag = ItemWithAmmoContainer("mag_pd_ussr_toygun_std_20x1mm_18", isChamber: false, ToyAmmoId);
        var items = new Dictionary<MongoId, TemplateItem>
        {
            [new MongoId(ToyGunId)] = gun,
            [new MongoId(ToyGunMagId)] = mag,
        };
        var plugin = BuildPlugin(items);

        await plugin.OnLoadAsync(CancellationToken.None);
        await plugin.OnLoadAsync(CancellationToken.None);

        Assert.Equal(1 + Caliber762x25AmmoIds.Length, AmmoFilterOf(gun, isChamber: true).Count);
        Assert.Single(AmmoFilterOf(mag, isChamber: false));
    }

    [Fact]
    public async Task AddsM14DcsbMountToAlphaDogScopeSlot()
    {
        var alphaDog = ItemWithSlot("silencer_all_alpha_dog_alpha_9_9x19", "mod_scope", "58d39d3d86f77445bb794ae7");
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(AlphaDogSuppressorId)] = alphaDog };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(alphaDog, "mod_scope");
        Assert.Contains(new MongoId("58d39d3d86f77445bb794ae7"), filter);
        Assert.Contains(new MongoId(M14DcsbMountId), filter);
        Assert.Equal(2, filter.Count);
    }

    [Fact]
    public async Task Adds762x25ToMarlinMxlrMagazine()
    {
        var mag = ItemWithAmmoContainer("mag_m1895_marlin_mxlr_784x49_5", isChamber: false, "67c540c3d0538d12ec036c08");
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(MxlrMagId)] = mag };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = AmmoFilterOf(mag, isChamber: false);
        Assert.Contains(new MongoId("67c540c3d0538d12ec036c08"), filter);
        foreach (string id in Caliber762x25AmmoIds)
        {
            Assert.Contains(new MongoId(id), filter);
        }
        Assert.Equal(1 + Caliber762x25AmmoIds.Length, filter.Count);
    }

    [Fact]
    public async Task AddsMarlinMxlrMagazineToToyGunMagazineSlot()
    {
        var gun = ItemWithSlot("weapon_ussr_pd_20x1mm", "mod_magazine", ToyGunMagId);
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(ToyGunId)] = gun };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(gun, "mod_magazine");
        Assert.Contains(new MongoId(ToyGunMagId), filter);
        Assert.Contains(new MongoId(MxlrMagId), filter);
        Assert.Equal(2, filter.Count);
    }

    [Fact]
    public async Task AddsM14DcsbMountToEmtiScopeSlot()
    {
        var emti = ItemWithSlot("mount_7mm_etmi_019", "mod_scope", EmtiId);
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(EmtiId)] = emti };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(emti, "mod_scope");
        Assert.Contains(new MongoId("5addbffe5acfc4001714dfac"), filter);
    }

    [Fact]
    public async Task AddsAllMp18HandguardsToMts255HandguardSlot()
    {
        var gun = ItemWithSlot("weapon_ckib_mc_255_12g", "mod_handguard", Mts255HandguardExistingId);
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(Mts255Id)] = gun };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(gun, "mod_handguard");
        Assert.Contains(new MongoId(Mts255HandguardExistingId), filter);
        Assert.Contains(new MongoId(Mp18HandguardWoodId), filter);
        Assert.Contains(new MongoId(Mp18HandguardPlasticId), filter);
        Assert.Equal(3, filter.Count);
    }

    [Fact]
    public async Task AddsAllM870StocksToMts255StockSlot()
    {
        var gun = ItemWithSlot("weapon_ckib_mc_255_12g", "mod_stock", Mts255StockExistingId);
        var items = new Dictionary<MongoId, TemplateItem> { [new MongoId(Mts255Id)] = gun };

        await BuildPlugin(items).OnLoadAsync(CancellationToken.None);

        var filter = FilterOf(gun, "mod_stock");
        Assert.Contains(new MongoId(Mts255StockExistingId), filter);
        foreach (string id in M870StockIds)
        {
            Assert.Contains(new MongoId(id), filter);
        }

        Assert.Equal(1 + M870StockIds.Length, filter.Count);
    }

    [Fact]
    public async Task MissingItemsDoNotThrow()
    {
        await BuildPlugin(new Dictionary<MongoId, TemplateItem>()).OnLoadAsync(CancellationToken.None);
    }
}
