using System;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.UI;
using static VIWI.Core.VIWIContext;

namespace VIWI.Modules.Workshoppa;

internal sealed partial class WorkshoppaModule
{
    private unsafe void SelectYesNoPostSetup(AddonEvent type, AddonArgs args)
    {
        PluginLog.Verbose("SelectYesNo post-setup");

        AddonSelectYesno* addonSelectYesNo = (AddonSelectYesno*)args.Addon.Address;
        string text = MemoryHelper.ReadSeString(&addonSelectYesNo->PromptText->NodeText).ToString()
            .Replace("\n", "", StringComparison.Ordinal)
            .Replace("\r", "", StringComparison.Ordinal);
        PluginLog.Verbose($"YesNo prompt: '{text}'");

        if (_repairKitWindow.IsOpen)
        {
            PluginLog.Verbose($"Checking for Repair Kit YesNo ({_repairKitWindow.AutoBuyEnabled}, {_repairKitWindow.IsAwaitingYesNo})");
            // Some localizations (Japanese, etc.) may produce slightly different prompt text that
            // the regex doesn't catch. Accept a fallback substring match for common phrases used
            // by purchase confirmations so the auto-buy flow doesn't get stuck on the Yes/No prompt.
            if (_repairKitWindow.AutoBuyEnabled && _repairKitWindow.IsAwaitingYesNo &&
                (_gameStrings.PurchaseItemForGil.IsMatch(text) ||
                 text.Contains("購入します", StringComparison.Ordinal) ||
                 text.Contains("Purchase", StringComparison.Ordinal)))
            {
                PluginLog.Information($"Selecting 'yes' ({text})");
                _repairKitWindow.IsAwaitingYesNo = false;
                addonSelectYesNo->AtkUnitBase.FireCallbackInt(0);
            }
            else
            {
                PluginLog.Verbose("Not a purchase confirmation match");
            }
        }
        else if (_ceruleumTankWindow.IsOpen)
        {
            PluginLog.Verbose($"Checking for Ceruleum Tank YesNo ({_ceruleumTankWindow.AutoBuyEnabled}, {_ceruleumTankWindow.IsAwaitingYesNo})");
            if (_ceruleumTankWindow.AutoBuyEnabled && _ceruleumTankWindow.IsAwaitingYesNo &&
                (_gameStrings.PurchaseItemForCompanyCredits.IsMatch(text) ||
                 text.Contains("購入します", StringComparison.Ordinal) ||
                 text.Contains("Purchase", StringComparison.Ordinal)))
            {
                PluginLog.Information($"Selecting 'yes' ({text})");
                _ceruleumTankWindow.IsAwaitingYesNo = false;
                addonSelectYesNo->AtkUnitBase.FireCallbackInt(0);
            }
            else
            {
                PluginLog.Verbose("Not a purchase confirmation match");
            }
        }
        else if (_grindstoneShopWindow.IsOpen)
        {
            PluginLog.Verbose($"Checking for Mudstone YesNo ({_grindstoneShopWindow.AutoBuyEnabled}, {_grindstoneShopWindow.IsAwaitingYesNo})");
            if (_grindstoneShopWindow.AutoBuyEnabled && _grindstoneShopWindow.IsAwaitingYesNo &&
                (_gameStrings.PurchaseItemForCompanyCredits.IsMatch(text) ||
                 text.Contains("購入します", StringComparison.Ordinal) ||
                 text.Contains("Purchase", StringComparison.Ordinal)))
            {
                PluginLog.Information($"Selecting 'yes' ({text})");
                _grindstoneShopWindow.IsAwaitingYesNo = false;
                addonSelectYesNo->AtkUnitBase.FireCallbackInt(0);
            }
            else
            {
                PluginLog.Verbose("Not a purchase confirmation match");
            }
        }
        else if (CurrentStage != Stage.Stopped)
        {
            // General fallback for crafting start/produce confirmations that may be localized.
            // Example JP: "製作します。よろしいですか？". If we see that phrase while running, auto-confirm.
            if (text.Contains("製作します", StringComparison.Ordinal) ||
                text.Contains("Start production", StringComparison.Ordinal) ||
                text.Contains("Start crafting", StringComparison.Ordinal) ||
                text.Contains("Make", StringComparison.Ordinal))
            {
                PluginLog.Information($"Selecting 'yes' for craft-start prompt ({text})");
                addonSelectYesNo->AtkUnitBase.FireCallbackInt(0);

                // Mirror behavior of ConfirmCraft: mark current item as started and advance stage
                if (_configuration.CurrentlyCraftedItem != null)
                {
                    try
                    {
                        _configuration.CurrentlyCraftedItem.StartedCrafting = true;
                        SaveConfig();
                    }
                    catch (Exception ex)
                    {
                        PluginLog.Warning(ex, "Failed to save config after auto-starting craft");
                    }
                }

                CurrentStage = Stage.TargetFabricationStation;
                _continueAt = DateTime.Now.AddSeconds(0.5);

                return;
            }
            if (CurrentStage == Stage.ConfirmMaterialDelivery && _gameStrings.TurnInHighQualityItem == text)
            {
                PluginLog.Information($"Selecting 'yes' ({text})");
                addonSelectYesNo->AtkUnitBase.FireCallbackInt(0);
            }
            else if (CurrentStage == Stage.ConfirmMaterialDelivery && _gameStrings.ContributeItems.IsMatch(text))
            {
                PluginLog.Information($"Selecting 'yes' ({text})");
                addonSelectYesNo->AtkUnitBase.FireCallbackInt(0);

                ConfirmMaterialDeliveryFollowUp();
            }
            else if (CurrentStage == Stage.ConfirmCollectProduct && _gameStrings.RetrieveFinishedItem.IsMatch(text))
            {
                PluginLog.Information($"Selecting 'yes' ({text})");
                addonSelectYesNo->AtkUnitBase.FireCallbackInt(0);

                ConfirmCollectProductFollowUp();
            }
            else if (CurrentStage == Stage.MergeStacks && _gameStrings.WorkshopMenuExit == text)
            {
                PluginLog.Information($"Selecting ({text})");
                addonSelectYesNo->AtkUnitBase.FireCallbackInt(0);
                _continueAt = DateTime.Now.AddSeconds(1);
            }
            else if (CurrentStage == Stage.DiscontinueProject && _gameStrings.DiscontinueItem.IsMatch(text))
            {
                PluginLog.Information($"Selecting 'yes' ({text})");
                addonSelectYesNo->AtkUnitBase.FireCallbackInt(0);
                if (_configuration.Mode == WorkshoppaConfig.TurnInMode.Leveling)
                {
                    ResetLevelingProject();
                }
                _configuration.CurrentlyCraftedItem = null;
                // Save and return to the start of the work loop so the next queued item is processed
                SaveConfig();
                CurrentStage = Stage.TakeItemFromQueue;
                _continueAt = DateTime.Now.AddSeconds(1);
            }
        }
    }

    private void ConfirmCollectProductFollowUp()
    {
        _configuration.CurrentlyCraftedItem = null;
        SaveConfig();

        CurrentStage = Stage.TakeItemFromQueue;
        _continueAt = DateTime.Now.AddSeconds(0.5);
    }
}
