using System;
using System.Runtime.InteropServices;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace BetterPartyFinder;

// Code taken from: https://git.anna.lgbt/anna/XivCommon/src/branch/main/XivCommon/Functions/PartyFinder.cs
public unsafe class HookManager
{
    [Signature("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 40 0F 10 81")]
    private readonly delegate* unmanaged<AgentLookingForGroup*, byte, byte> RequestPartyFinderListings = null!;

    public HookManager()
    {
        Plugin.GameInteropProvider.InitializeFromAttributes(this);
    }

    public void RefreshListings() {
        if (this.RequestPartyFinderListings == null) {
            throw new InvalidOperationException("Could not find signature for Party Finder listings");
        }

        // Can be replaced with <https://github.com/aers/FFXIVClientStructs/pull/1087> after merge
        const int categoryOffset = 0x3103;

        var agent = AgentLookingForGroup.Instance();
        var categoryIdx = Marshal.ReadByte((nint) agent, categoryOffset);
        RequestPartyFinderListings(agent, categoryIdx);
    }
}