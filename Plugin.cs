using System;
using Dalamud.Data;
using Dalamud.Game.ClientState;
using Dalamud.Plugin;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Command;
using Dalamud.Game;
using Dalamud.IoC;
using DeepDungeonDex.Data;
using DeepDungeonDex.UI;

namespace DeepDungeonDex
{
    public class Plugin : IDalamudPlugin
    {
        public static Configuration Config;

        private PluginUI ui;
        private ConfigUI cui;
        private GameObject previousTarget;

        [PluginService] internal static DalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService] internal static ClientState ClientState { get; private set; } = null!;
        [PluginService] internal static CommandManager CommandManager { get; private set; } = null!;
        [PluginService] internal static Condition Condition { get; private set; } = null!;
        [PluginService] internal static Framework Framework { get; private set; } = null!;
        [PluginService] internal static TargetManager Targets { get; private set; } = null!;

        [PluginService] internal static DataManager DataManager { get; private set; } = null!;

        public string Name => "DeepDungeonDex";

        public Plugin()
        {
            Config = (Configuration)PluginInterface.GetPluginConfig() ?? new Configuration();
            Config.Initialize(PluginInterface);

            DataRepo.Load();

            this.ui = new PluginUI();
            this.cui = new ConfigUI(Config.Opacity, Config.IsClickthrough, Config.HideRedVulns, Config.HideBasedOnJob, Config);
            
            PluginInterface.UiBuilder.Draw += this.ui.Draw;
            PluginInterface.UiBuilder.Draw += this.cui.Draw;

            CommandManager.AddHandler("/pddd", new CommandInfo(OpenConfig)
            {
                HelpMessage = "DeepDungeonDex config"
            });
            
            Framework.Update += this.GetData;
        }

        public void OpenConfig(string command, string args)
        {
            cui.IsVisible = true;
        }

        public void GetData(Framework framework)
        {
            if (!Condition[ConditionFlag.InDeepDungeon])
            {
                ui.IsVisible = false;
                return;
            }
            GameObject target = Targets.Target;

            TargetData t = new TargetData();
            if (!t.IsValidTarget(target))
            {
                ui.IsVisible = false;
                return;
            }
            else
            { 
                previousTarget = target;
                ui.IsVisible = true;
            }
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            CommandManager.RemoveHandler("/pddd");

            PluginInterface.SavePluginConfig(Config);

            PluginInterface.UiBuilder.Draw -= this.ui.Draw;
            PluginInterface.UiBuilder.Draw -= this.cui.Draw;

            Framework.Update -= this.GetData;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}