using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace BetterPartyFinder.Windows.Main;

public partial class MainWindow
{
    private void DrawItemLevelTab(ConfigurationFilter filter)
    {
        using var tabItem = ImRaii.TabItem("Item level");
        if (!tabItem.Success)
            return;

        var hugePfs = filter.AllowHugeItemLevel;
        if (ImGui.Checkbox("Show PFs above maximum item level", ref hugePfs))
        {
            filter.AllowHugeItemLevel = hugePfs;
            Plugin.Config.Save();
        }

        var minLevel = (int?)filter.MinItemLevel ?? 0;
        ImGui.TextUnformatted("Minimum item level (0 to disable)");
        ImGui.PushItemWidth(-1);
        if (ImGui.InputInt("###min-ilvl", ref minLevel))
        {
            filter.MinItemLevel = minLevel == 0 ? null : (uint)minLevel;
            Plugin.Config.Save();
        }
        ImGui.PopItemWidth();

        var maxLevel = (int?)filter.MaxItemLevel ?? 0;
        ImGui.TextUnformatted("Maximum item level (0 to disable)");
        ImGui.PushItemWidth(-1);
        if (ImGui.InputInt("###max-ilvl", ref maxLevel))
        {
            filter.MaxItemLevel = maxLevel == 0 ? null : (uint)maxLevel;
            Plugin.Config.Save();
        }
        ImGui.PopItemWidth();
    }
}