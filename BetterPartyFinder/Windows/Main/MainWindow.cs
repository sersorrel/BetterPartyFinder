using System;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;

namespace BetterPartyFinder.Windows.Main;

public unsafe partial class MainWindow : Window, IDisposable
{
    private readonly Plugin Plugin;

    private bool IsCollapsed;
    private AtkUnitBase* Addon = null;

    private string PresetName { get; set; } = string.Empty;
    private string DutySearchQuery { get; set; } = string.Empty;

    public MainWindow(Plugin plugin) : base(Plugin.Name)
    {
        Size = new Vector2(550, 510);
        SizeCondition = ImGuiCond.FirstUseEver;

        Flags = ImGuiWindowFlags.NoDocking;

        Plugin = plugin;
    }

    public void Dispose() { }

    public override void PreOpenCheck()
    {
        Addon = null;

        var addonPtr = Plugin.GameGui.GetAddonByName("LookingForGroup");
        if (Plugin.Config.ShowWhenPfOpen && addonPtr != IntPtr.Zero)
            Addon = (AtkUnitBase*) addonPtr;

        IsOpen = IsOpen || Addon != null && Addon->IsVisible;
    }

    public override void PreDraw()
    {
        IsCollapsed = true;

        if (Position.HasValue)
            ImGui.SetNextWindowPos(Position.Value, ImGuiCond.Always);

        Position = null;
        base.PreDraw();
    }

    public override void PostDraw()
    {
        if (IsCollapsed && Addon != null && Addon->IsVisible)
        {
            // wait until addon is initialised to show
            var rootNode = Addon->RootNode;
            if (rootNode == null)
                return;

            Position = ImGuiHelpers.MainViewport.Pos + new Vector2(Addon->X, Addon->Y - ImGui.GetFrameHeight());
        }
        base.PostDraw();
    }

    public override void Draw()
    {
        IsCollapsed = false;

        if (Addon != null && Plugin.Config.WindowSide == WindowSide.Right)
        {
            var rootNode = Addon->RootNode;
            if (rootNode != null)
                ImGui.SetWindowPos(ImGuiHelpers.MainViewport.Pos + new Vector2(Addon->X + rootNode->Width, Addon->Y));
        }

        var selected = Plugin.Config.SelectedPreset;

        string selectedName;
        if (selected == null)
        {
            selectedName = "<none>";
        }
        else
        {
            if (Plugin.Config.Presets.TryGetValue(selected.Value, out var preset))
            {
                selectedName = preset.Name;
            }
            else
            {
                Plugin.Config.SelectedPreset = null;
                selectedName = "<invalid preset>";
            }
        }

        ImGui.TextUnformatted("Preset");
        using (var combo = ImRaii.Combo("###preset", selectedName))
        {
            if (combo.Success)
            {
                if (ImGui.Selectable("<none>"))
                {
                    Plugin.Config.SelectedPreset = null;
                    Plugin.Config.Save();

                    Plugin.HookManager.RefreshListings();
                }

                foreach (var preset in Plugin.Config.Presets)
                {
                    if (!ImGui.Selectable(preset.Value.Name))
                        continue;

                    Plugin.Config.SelectedPreset = preset.Key;
                    Plugin.Config.Save();

                    Plugin.HookManager.RefreshListings();
                }
            }
        }

        ImGui.SameLine();

        if (Helper.IconButton(FontAwesomeIcon.Plus, "add-preset"))
        {
            var id = Guid.NewGuid();

            Plugin.Config.Presets.Add(id, ConfigurationFilter.Create());
            Plugin.Config.SelectedPreset = id;
            Plugin.Config.Save();
        }

        ImGui.SameLine();

        if (Helper.IconButton(FontAwesomeIcon.Trash, "delete-preset") && selected != null)
        {
            Plugin.Config.Presets.Remove(selected.Value);
            Plugin.Config.Save();
        }

        ImGui.SameLine();

        if (Helper.IconButton(FontAwesomeIcon.PencilAlt, "edit-preset") && selected != null)
        {
            if (Plugin.Config.Presets.TryGetValue(selected.Value, out var editPreset))
            {
                PresetName = editPreset.Name;

                ImGui.OpenPopup("###rename-preset");
            }
        }

        using (var modal = ImRaii.PopupModal("Rename preset###rename-preset"))
        {
            if (modal.Success && selected != null)
            {
                if (Plugin.Config.Presets.TryGetValue(selected.Value, out var editPreset))
                {
                    ImGui.TextUnformatted("Preset name");
                    ImGui.PushItemWidth(-1f);
                    var name = PresetName;
                    if (ImGui.InputText("###preset-name", ref name, 1_000))
                        PresetName = name;
                    ImGui.PopItemWidth();

                    if (ImGui.Button("Save") && PresetName.Trim().Length > 0)
                    {
                        editPreset.Name = PresetName;
                        Plugin.Config.Save();
                        ImGui.CloseCurrentPopup();
                    }
                }
            }
        }

        ImGui.SameLine();

        if (Helper.IconButton(FontAwesomeIcon.Copy, "copy") && selected != null)
        {
            if (Plugin.Config.Presets.TryGetValue(selected.Value, out var copyFilter))
            {
                var guid = Guid.NewGuid();

                var copied = copyFilter.Clone();
                copied.Name += " (copy)";
                Plugin.Config.Presets.Add(guid, copied);
                Plugin.Config.SelectedPreset = guid;
                Plugin.Config.Save();
            }
        }

        ImGui.SameLine();

        if (Helper.IconButton(FontAwesomeIcon.Cog, "settings"))
            Plugin.ConfigWindow.Toggle();

        ImGui.Separator();

        if (selected != null && Plugin.Config.Presets.TryGetValue(selected.Value, out var filter))
            DrawPresetConfiguration(filter);

        if (Addon != null && Plugin.Config.WindowSide == WindowSide.Left)
        {
            var rootNode = Addon->RootNode;
            if (rootNode != null)
            {
                var currentWidth = ImGui.GetWindowWidth();
                ImGui.SetWindowPos(ImGuiHelpers.MainViewport.Pos + new Vector2(Addon->X - currentWidth, Addon->Y));
            }
        }
    }

    private void DrawPresetConfiguration(ConfigurationFilter filter)
    {
        using var tabBar = ImRaii.TabBar("bpf-tabs");
        if (!tabBar.Success)
            return;

        DrawCategoriesTab(filter);

        DrawDutiesTab(filter);

        DrawItemLevelTab(filter);

        DrawJobsTab(filter);

        DrawRestrictionsTab(filter);

        DrawPlayersTab(filter);
    }
}
