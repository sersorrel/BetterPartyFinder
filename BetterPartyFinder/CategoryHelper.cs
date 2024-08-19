using Dalamud.Game.Gui.PartyFinder.Types;
using Lumina.Excel.GeneratedSheets;

namespace BetterPartyFinder;

public enum UiCategory
{
    None,
    DutyRoulette,
    Dungeons,
    Guildhests,
    Trials,
    Raids,
    HighEndDuty,
    Pvp,
    QuestBattles,
    Fates,
    TreasureHunt,
    TheHunt,
    GatheringForays,
    DeepDungeons,
    AdventuringForays,
    VCDungeon,
}

internal static class UiCategoryExt
{
    internal static string? Name(this UiCategory category)
    {
        var ct = Plugin.DataManager.GetExcelSheet<ContentType>()!;
        var addon = Plugin.DataManager.GetExcelSheet<Addon>()!;

        return category switch
        {
            UiCategory.None => addon.GetRow(1_562)?.Text.ToString(), // best guess
            UiCategory.DutyRoulette => ct.GetRow((uint) ContentType2.DutyRoulette)?.Name.ToString(),
            UiCategory.Dungeons => ct.GetRow((uint) ContentType2.Dungeons)?.Name.ToString(),
            UiCategory.Guildhests => ct.GetRow((uint) ContentType2.Guildhests)?.Name.ToString(),
            UiCategory.Trials => ct.GetRow((uint) ContentType2.Trials)?.Name.ToString(),
            UiCategory.Raids => ct.GetRow((uint) ContentType2.Raids)?.Name.ToString(),
            UiCategory.HighEndDuty => addon.GetRow(10_822)?.Text.ToString(), // best guess
            UiCategory.Pvp => ct.GetRow((uint) ContentType2.Pvp)?.Name.ToString(),
            UiCategory.QuestBattles => ct.GetRow((uint) ContentType2.QuestBattles)?.Name.ToString(),
            UiCategory.Fates => ct.GetRow((uint) ContentType2.Fates)?.Name.ToString(),
            UiCategory.TreasureHunt => ct.GetRow((uint) ContentType2.TreasureHunt)?.Name.ToString(),
            UiCategory.TheHunt => addon.GetRow(8_613)?.Text.ToString(),
            UiCategory.GatheringForays => addon.GetRow(2_306)?.Text.ToString(),
            UiCategory.DeepDungeons => ct.GetRow((uint) ContentType2.DeepDungeons)?.Name.ToString(),
            UiCategory.AdventuringForays => addon.GetRow(2_307)?.Text.ToString(),
            UiCategory.VCDungeon => ct.GetRow((uint)ContentType2.VCDungeon)?.Name.ToString(),
            _ => null,
        };
    }

    internal static bool ListingMatches(this UiCategory category, IPartyFinderListing listing)
    {
        var cr = Plugin.DataManager.GetExcelSheet<ContentRoulette>()!;

        var isDuty = listing.Category is DutyCategory.None or DutyCategory.DutyRoulette or DutyCategory.Dungeon
            or DutyCategory.Guildhest or DutyCategory.Trial or DutyCategory.Raid or DutyCategory.HighEndDuty
            or DutyCategory.PvP; // tldr: "high byte is 0"
        var isNormal = listing.DutyType == DutyType.Normal;
        var isOther = listing.DutyType == DutyType.Other;
        var isNormalDuty = isNormal && isDuty;

        Plugin.Log.Verbose($"name {category.Name()}/{listing.Name.TextValue}, isduty {isDuty} {isNormal} {isOther} {isNormalDuty}, cat {listing.Category}, type {listing.DutyType}, raw {listing.RawDuty}");

        var result = category switch
        {
            UiCategory.None => isOther && isDuty && listing.RawDuty == 0,
            UiCategory.DutyRoulette => listing.DutyType == DutyType.Roulette && isDuty && (!cr.GetRow(listing.RawDuty)?.IsPvP ?? false),
            UiCategory.Dungeons => isNormalDuty && listing.Duty.Value.ContentType.Row == (uint) ContentType2.Dungeons,
            UiCategory.Guildhests => isNormalDuty && listing.Duty.Value.ContentType.Row == (uint) ContentType2.Guildhests,
            UiCategory.Trials => isNormalDuty && !listing.Duty.Value.HighEndDuty && listing.Duty.Value.ContentType.Row == (uint) ContentType2.Trials,
            UiCategory.Raids => isNormalDuty && !listing.Duty.Value.HighEndDuty && listing.Duty.Value.ContentType.Row == (uint) ContentType2.Raids,
            UiCategory.HighEndDuty => isNormalDuty && listing.Duty.Value.HighEndDuty,
            UiCategory.Pvp => listing.DutyType == DutyType.Roulette && isDuty && (cr.GetRow(listing.RawDuty)?.IsPvP ?? false)
                              || isNormalDuty && listing.Duty.Value.ContentType.Row == (uint) ContentType2.Pvp,
            UiCategory.QuestBattles => isOther && listing.Category == DutyCategory.GoldSaucer,
            UiCategory.Fates => isOther && listing.Category == DutyCategory.Fate,
            UiCategory.TreasureHunt => isOther && listing.Category == DutyCategory.TreasureHunt,
            UiCategory.TheHunt => isOther && listing.Category == DutyCategory.TheHunt,
            UiCategory.GatheringForays => isNormal && listing.Category == DutyCategory.GatheringForay,
            UiCategory.DeepDungeons => isOther && listing.Category == DutyCategory.DeepDungeon,
            UiCategory.AdventuringForays => isNormal && listing.Category == DutyCategory.FieldOperation,
            UiCategory.VCDungeon => isNormal && listing.Duty.Value.ContentType.Row == (uint) ContentType2.VCDungeon,
            _ => false,
        };

        Plugin.Log.Verbose($"result: {result}");

        return result;
    }

    private enum ContentType2
    {
        DutyRoulette = 1,
        Dungeons = 2,
        Guildhests = 3,
        Trials = 4,
        Raids = 5,
        Pvp = 6,
        QuestBattles = 7,
        Fates = 8,
        TreasureHunt = 9,
        Levequests = 10,
        GrandCompany = 11,
        Companions = 12,
        BeastTribeQuests = 13,
        OverallCompletion = 14,
        PlayerCommendation = 15,
        DisciplesOfTheLand = 16,
        DisciplesOfTheHand = 17,
        RetainerVentures = 18,
        GoldSaucer = 19,
        DeepDungeons = 21,
        WondrousTails = 24,
        CustomDeliveries = 25,
        Eureka = 26,
        UltimateRaids = 28,
        VCDungeon = 30,
    }
}