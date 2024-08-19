using System;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace BetterPartyFinder.Windows.Main;

public partial class MainWindow
{
    private void DrawCategoriesTab(ConfigurationFilter filter)
    {
        using var tabItem = ImRaii.TabItem("Categories");
        if (!tabItem.Success)
            return;

        foreach (var category in (UiCategory[])Enum.GetValues(typeof(UiCategory)))
        {
            var selected = filter.Categories.Contains(category);
            if (!ImGui.Selectable(category.Name(), ref selected))
                continue;

            if (selected)
                filter.Categories.Add(category);
            else
                filter.Categories.Remove(category);

            Plugin.Config.Save();
        }
    }
}