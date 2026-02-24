using ECommons.Logging;

namespace VIWI.Utils
{
    public static class Log
    {
        public static void Info(string msg) => PluginLog.Information($"[VIWI] {msg}");
        public static void Error(string msg) => PluginLog.Error($"[VIWI] {msg}");
        public static void Warn(string msg) => PluginLog.Warning($"[VIWI] {msg}");
    }
}