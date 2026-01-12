using ECommons.DalamudServices;
using ECommons.EzIpcManager;
using System;
using System.Linq;

namespace VIWI.IPC;

#nullable disable
public class AutoRetainerIPC 
{
    public string Name => "AutoRetainer";
    public string Repo => "https://love.puni.sh/ment.json";
    public bool IsLoaded => Svc.PluginInterface.InstalledPlugins.Any(p => p.InternalName == Name && p.IsLoaded);
    public AutoRetainerIPC() => EzIPC.Init(this, Name);

    [EzIPC("PluginState.%m")] public readonly Func<bool> IsBusy;
    [EzIPC("PluginState.%m")] public readonly Func<int> GetInventoryFreeSlotCount;
    [EzIPC] public readonly Func<bool> GetMultiModeEnabled;
    [EzIPC] public readonly Action<bool> SetMultiModeEnabled;
    [EzIPC] public readonly Func<bool> GetSuppressed;
    [EzIPC] public readonly Action<bool> SetSuppressed;
    [EzIPC("GC.%m")] public readonly Action EnqueueInitiation;
}