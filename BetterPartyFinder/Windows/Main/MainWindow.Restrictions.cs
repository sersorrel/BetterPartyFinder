using Dalamud.Game.Gui.PartyFinder.Types;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace BetterPartyFinder.Windows.Main;

public partial class MainWindow
{
    private void DrawRestrictionsTab(ConfigurationFilter filter)
    {
        using var tabItem = ImRaii.TabItem("Restrictions");
        if (!tabItem.Success)
            return;

        var practice = filter[ObjectiveFlags.Practice];
        if (ImGui.Checkbox("Practice", ref practice))
        {
            filter[ObjectiveFlags.Practice] = practice;
            Plugin.Config.Save();
        }

        var dutyCompletion = filter[ObjectiveFlags.DutyCompletion];
        if (ImGui.Checkbox("Duty completion", ref dutyCompletion))
        {
            filter[ObjectiveFlags.DutyCompletion] = dutyCompletion;
            Plugin.Config.Save();
        }

        var loot = filter[ObjectiveFlags.Loot];
        if (ImGui.Checkbox("Loot", ref loot))
        {
            filter[ObjectiveFlags.Loot] = loot;
            Plugin.Config.Save();
        }

        ImGui.Separator();

        var noCondition = filter[ConditionFlags.None];
        if (ImGui.Checkbox("No duty completion requirement", ref noCondition))
        {
            filter[ConditionFlags.None] = noCondition;
            Plugin.Config.Save();
        }

        var dutyIncomplete = filter[ConditionFlags.DutyIncomplete];
        if (ImGui.Checkbox("Duty incomplete", ref dutyIncomplete))
        {
            filter[ConditionFlags.DutyIncomplete] = dutyIncomplete;
            Plugin.Config.Save();
        }

        var dutyComplete = filter[ConditionFlags.DutyComplete];
        if (ImGui.Checkbox("Duty complete", ref dutyComplete))
        {
            filter[ConditionFlags.DutyComplete] = dutyComplete;
            Plugin.Config.Save();
        }

        ImGui.Separator();

        var undersized = filter[DutyFinderSettingsFlags.UndersizedParty];
        if (ImGui.Checkbox("Undersized party", ref undersized))
        {
            filter[DutyFinderSettingsFlags.UndersizedParty] = undersized;
            Plugin.Config.Save();
        }

        var minItemLevel = filter[DutyFinderSettingsFlags.MinimumItemLevel];
        if (ImGui.Checkbox("Minimum item level", ref minItemLevel))
        {
            filter[DutyFinderSettingsFlags.MinimumItemLevel] = minItemLevel;
            Plugin.Config.Save();
        }

        var silenceEcho = filter[DutyFinderSettingsFlags.SilenceEcho];
        if (ImGui.Checkbox("Silence Echo", ref silenceEcho))
        {
            filter[DutyFinderSettingsFlags.SilenceEcho] = silenceEcho;
            Plugin.Config.Save();
        }

        ImGui.Separator();

        var greedOnly = filter[LootRuleFlags.GreedOnly];
        if (ImGui.Checkbox("Greed only", ref greedOnly))
        {
            filter[LootRuleFlags.GreedOnly] = greedOnly;
            Plugin.Config.Save();
        }

        var lootmaster = filter[LootRuleFlags.Lootmaster];
        if (ImGui.Checkbox("Lootmaster", ref lootmaster))
        {
            filter[LootRuleFlags.Lootmaster] = lootmaster;
            Plugin.Config.Save();
        }

        ImGui.Separator();

        var dataCentre = filter[SearchAreaFlags.DataCentre];
        if (ImGui.Checkbox("Data centre parties", ref dataCentre))
        {
            filter[SearchAreaFlags.DataCentre] = dataCentre;
            Plugin.Config.Save();
        }

        var world = filter[SearchAreaFlags.World];
        if (ImGui.Checkbox("World-local parties", ref world))
        {
            filter[SearchAreaFlags.World] = world;
            Plugin.Config.Save();
        }

        var onePlayerPer = filter[SearchAreaFlags.OnePlayerPerJob];
        if (ImGui.Checkbox("One player per job", ref onePlayerPer))
        {
            filter[SearchAreaFlags.OnePlayerPerJob] = onePlayerPer;
            Plugin.Config.Save();
        }
    }
}