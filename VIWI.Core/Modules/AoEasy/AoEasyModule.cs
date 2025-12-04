using ECommons.Logging;
using VIWI.Core;

namespace VIWI.Modules.AoEasy
{
    public sealed class AoEasyModule : IVIWIModule
    {
        public const string ModuleName = "AoEasy";
        public const string ModuleVersion = "1.0.0";

        public string Name => ModuleName;
        public string Version => ModuleVersion;

        public static AoEasyConfig Config { get; private set; } = null!;
        public static bool Enabled => Config?.Enabled ?? false;

        public void Initialize()
        {
            PluginLog.Information("[VIWI.AoEasy] Initializing...");

            LoadConfig();
            JobData.InitializeJobs();
            JobData.InitializeAbilities();

            PluginLog.Information("[VIWI.AoEasy] Initialized successfully.");
        }

        public void Dispose()
        {
            PluginLog.Information("[VIWI.AoEasy] Disposed.");
        }

        public static void LoadConfig()
        {
            Config = VIWIContext.PluginInterface.GetPluginConfig() as AoEasyConfig
                     ?? new AoEasyConfig();

            SaveConfig();
        }

        public static void SaveConfig()
        {
            Config.Save();
        }
        public void Update()
        {
            var player = VIWIContext.PlayerState;
            if (player != null && JobData.TryGet(player.ClassJob.RowId, out var jobInfo))
            {
                PluginLog.Information($"Current job: {jobInfo.Name} ({jobInfo.Abbreviation}), Role={jobInfo.Role}");
            }
        }
    }
}
