using System.Linq;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;

namespace BetterPartyFinder.Windows.Main;

public partial class MainWindow
{
    private int SelectedWorld;
    private string PlayerName = string.Empty;

    private void DrawPlayersTab(ConfigurationFilter filter)
    {
        var player = Plugin.ClientState.LocalPlayer;
        using var tabItem = ImRaii.TabItem("Players");
        if (player == null || !tabItem.Success)
            return;

        ImGui.PushItemWidth(ImGui.GetWindowWidth() / 3f);
        ImGui.InputText("###player-name", ref PlayerName, 64);

        ImGui.SameLine();

        var worlds = Util.WorldsOnDataCentre(player).OrderBy(world => world.Name.RawString).ToList();
        var worldNames = worlds.Select(world => world.Name.ToString()).ToArray();

        ImGui.Combo("###player-world", ref SelectedWorld, worldNames, worldNames.Length);
        ImGui.PopItemWidth();

        ImGui.SameLine();

        if (Helper.IconButton(FontAwesomeIcon.Plus, "add-player"))
        {
            var name = PlayerName.Trim();
            if (name.Length != 0)
            {
                var world = worlds[SelectedWorld];
                filter.Players.Add(new PlayerInfo(name, world.RowId));
                Plugin.Config.Save();
            }
        }

        PlayerInfo? deleting = null;
        foreach (var info in filter.Players)
        {
            var world = Plugin.DataManager.GetExcelSheet<World>()!.GetRow(info.World);
            ImGui.TextUnformatted($"{info.Name}@{world?.Name}");
            ImGui.SameLine();
            if (Helper.IconButton(FontAwesomeIcon.Trash, $"delete-player-{info.GetHashCode()}"))
                deleting = info;
        }

        if (deleting != null)
        {
            filter.Players.Remove(deleting);
            Plugin.Config.Save();
        }
    }
}