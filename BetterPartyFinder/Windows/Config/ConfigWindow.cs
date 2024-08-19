using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace BetterPartyFinder.Windows.Config;

public class ConfigWindow : Window, IDisposable
{
    private readonly Plugin Plugin;

    public ConfigWindow(Plugin plugin) : base($"{Plugin.Name} settings")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(320, 460),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        Plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        var openWithPf = Plugin.Config.ShowWhenPfOpen;
        if (ImGui.Checkbox("Open with PF", ref openWithPf))
        {
            Plugin.Config.ShowWhenPfOpen = openWithPf;
            Plugin.Config.Save();
        }

        var sideOptions = new[]
        {
            "Left",
            "Right",
        };
        var sideIdx = Plugin.Config.WindowSide == WindowSide.Left ? 0 : 1;

        ImGui.TextUnformatted("Side of PF window to dock to");
        if (ImGui.Combo("###window-side", ref sideIdx, sideOptions, sideOptions.Length))
        {
            Plugin.Config.WindowSide = sideIdx switch
            {
                0 => WindowSide.Left,
                1 => WindowSide.Right,
                _ => Plugin.Config.WindowSide,
            };

            Plugin.Config.Save();
        }
    }
}
