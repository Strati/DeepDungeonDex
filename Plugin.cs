using System;
using Dalamud.Data;
using Dalamud.Game.ClientState;
using Dalamud.Plugin;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects;
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

        private PluginUI MainUI;
        private ConfigUI ConfigUI;

        public static DataRepo<MobData> MobRepo { get; private set; }
        public static DataRepo<JobData> JobRepo { get; private set; }

        [PluginService] internal static DalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService] internal static ClientState ClientState { get; private set; } = null!;
        [PluginService] internal static CommandManager CommandManager { get; private set; } = null!;
        [PluginService] internal static Condition Condition { get; private set; } = null!;
        [PluginService] internal static Framework Framework { get; private set; } = null!;
        [PluginService] internal static TargetManager Targets { get; private set; } = null!;

        public string Name => "DeepDungeonDex";

        public Plugin()
        {
            Config = (Configuration)PluginInterface.GetPluginConfig() ?? new Configuration();
            Config.Initialize(PluginInterface);

            MobRepo = DataRepo<MobData>.Create(PluginInterface, "mob-data").Load();
            JobRepo = DataRepo<JobData>.Create(PluginInterface, "job-data").Load();

            MainUI = new PluginUI();
            ConfigUI = new ConfigUI(Config.Opacity, Config.IsClickthrough, Config.HideRedVulns, Config.HideBasedOnJob, Config);
            
            PluginInterface.UiBuilder.Draw += MainUI.Draw;
            PluginInterface.UiBuilder.Draw += ConfigUI.Draw;
            PluginInterface.UiBuilder.OpenConfigUi += OpenConfig;

            CommandManager.AddHandler("/pddd", new CommandInfo((c,a) => OpenConfig())
            {
                HelpMessage = "DeepDungeonDex config"
            });
            
            Framework.Update += GetData;
        }

        public void OpenConfig()
        {
            ConfigUI.IsVisible = true;
        }

        public void GetData(Framework framework)
        {
            if (!Condition[ConditionFlag.InDeepDungeon])
            {
                MainUI.IsVisible = false;
                return;
            }
            var target = Targets.Target;

            TargetData.UpdateTargetData(target, out var valid);
            MainUI.IsVisible = valid;
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            CommandManager.RemoveHandler("/pddd");

            PluginInterface.SavePluginConfig(Config);

            PluginInterface.UiBuilder.Draw -= MainUI.Draw;
            PluginInterface.UiBuilder.Draw -= ConfigUI.Draw;

            Framework.Update -= GetData;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}