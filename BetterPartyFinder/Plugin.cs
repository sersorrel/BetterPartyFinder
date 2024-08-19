using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace BetterPartyFinder {
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Plugin : IDalamudPlugin {
        internal static string Name => "Better Party Finder";

        [PluginService] public static IDalamudPluginInterface Interface { get; private set; } = null!;
        [PluginService] public static IChatGui ChatGui { get; private set; } = null!;
        [PluginService] public static IClientState ClientState { get; private set; } = null!;
        [PluginService] public static ICommandManager CommandManager { get; private set; } = null!;
        [PluginService] public static IDataManager DataManager { get; private set; } = null!;
        [PluginService] public static IGameGui GameGui { get; private set; } = null!;
        [PluginService] public static IPartyFinderGui PartyFinderGui { get; private set; } = null!;
        [PluginService] public static IGameInteropProvider GameInteropProvider { get; private set; } = null!;
        [PluginService] public static IPluginLog PluginLog { get; private set; } = null!;

        internal Configuration Config { get; }
        private Filter Filter { get; }
        internal PluginUi Ui { get; }
        private Commands Commands { get; }
        internal HookManager HookManager { get; }

        public Plugin() {
            this.Config = Configuration.Load(this) ?? new Configuration();
            this.Config.Initialise(this);

            this.HookManager = new HookManager();
            this.Filter = new Filter(this);
            this.Ui = new PluginUi(this);
            this.Commands = new Commands(this);

            // start task to determine maximum item level (based on max chestpiece)
            Util.CalculateMaxItemLevel(DataManager);
        }

        public void Dispose() {
            this.Commands.Dispose();
            this.Ui.Dispose();
            this.Filter.Dispose();
        }
    }
}
