using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using XivCommon;

namespace BetterPartyFinder {
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Plugin : IDalamudPlugin {
        internal static string Name => "Better Party Finder";

        [PluginService]
        internal IDalamudPluginInterface Interface { get; init; } = null!;

        [PluginService]
        internal IChatGui ChatGui { get; init; } = null!;

        [PluginService]
        internal IClientState ClientState { get; init; } = null!;

        [PluginService]
        internal ICommandManager CommandManager { get; init; } = null!;

        [PluginService]
        internal IDataManager DataManager { get; init; } = null!;

        [PluginService]
        internal IGameGui GameGui { get; init; } = null!;

        [PluginService]
        internal IPartyFinderGui PartyFinderGui { get; init; } = null!;

        [PluginService]
        internal IPluginLog PluginLog { get; init; } = null!;

        internal Configuration Config { get; }
        private Filter Filter { get; }
        internal PluginUi Ui { get; }
        private Commands Commands { get; }
        internal XivCommonBase Common { get; }
        private JoinHandler JoinHandler { get; }

        public Plugin() {
            this.Config = Configuration.Load(this) ?? new Configuration();
            this.Config.Initialise(this);

            this.Common = new XivCommonBase(this.Interface, Hooks.PartyFinder);
            this.Filter = new Filter(this);
            this.JoinHandler = new JoinHandler(this);
            this.Ui = new PluginUi(this);
            this.Commands = new Commands(this);

            // start task to determine maximum item level (based on max chestpiece)
            Util.CalculateMaxItemLevel(this.DataManager);
        }

        public void Dispose() {
            this.Commands.Dispose();
            this.Ui.Dispose();
            this.JoinHandler.Dispose();
            this.Filter.Dispose();
            this.Common.Dispose();
        }
    }
}
