using Dalamud.Configuration;
using System;

namespace VIWI.Modules.AoEasy
{
    [Serializable]
    public class AoEasyConfig : IPluginConfiguration
    {
        public int Version { get; set; } = 1;

        public bool Enabled = false;

        public void Save()
        {
            VIWI.Core.VIWIContext.PluginInterface.SavePluginConfig(this);
        }
    }
}
