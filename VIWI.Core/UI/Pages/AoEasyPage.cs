using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using ECommons.ImGuiMethods;
using System.Numerics;
using VIWI.Helpers;
using VIWI.Modules.AoEasy;

namespace VIWI.UI.Pages
{
    public sealed class AoEasyPage : IDashboardPage
    {
        public string DisplayName => "AoEasy";
        public string Category => "Modules";
        public string Version => AoEasyModule.ModuleVersion;
        public bool SupportsEnableToggle => true;

        public bool IsEnabled
        {
            get => AoEasyModule.Enabled;
        }

        public void SetEnabled(bool value)
        {
            AoEasyModule.Instance?.SetEnabled(value);
        }

        public void Draw()
        {
            var module = AoEasyModule.Instance;
            var config = module?._configuration;
            if (config == null)
            {
                ImGui.TextDisabled("AoEasy is not initialized yet.");
                return;
            }
            ImGuiHelpers.ScaledDummy(4f);
            ImGui.TextUnformatted($"AoEasy - V{Version}");
            ImGui.SameLine();
            ImGui.TextColored(GradientColor.Get(ImGuiHelper.RainbowColorStart, ImGuiHelper.RainbowColorEnd, 500), "Stop Running Away From Me!");
            ImGui.TextUnformatted("Enabled:");
            ImGui.SameLine();
            ImGui.TextColored(
                config.Enabled ? new Vector4(0.3f, 1f, 0.3f, 1f) : new Vector4(1f, 0.3f, 0.3f, 1f),
                config.Enabled ? "Yes" : "No - Click the OFF button to Enable AoEasy!!"
            );
            ImGuiHelpers.ScaledDummy(4f);
            ImGui.Separator();

            ImGuiHelpers.ScaledDummy(8f);
            ImGui.TextUnformatted("Description:");
            ImGuiHelpers.ScaledDummy(4f);
            ImGui.TextWrapped(
                "STILL IN DEVELOPMENT!!!"
            );
            ImGui.Separator();

            ImGuiHelpers.ScaledDummy(8f);

        }
    }
}
