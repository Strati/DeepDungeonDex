﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace DeepDungeonDex.UI
{
    public class ConfigUI
    {
        public bool IsVisible { get; set; }
        private float opacity;
        private bool isClickthrough;
        private bool HideRedVulns;
        private bool HideBasedOnJob;
        private Configuration config;

        public ConfigUI(float opacity, bool isClickthrough, bool HideRedVulns, bool HideBasedOnJob, Configuration config)
        {
            this.config = config;
            this.opacity = opacity;
            this.isClickthrough = isClickthrough;
            this.HideRedVulns = HideRedVulns;
            this.HideBasedOnJob = HideBasedOnJob;
        }

        public void Draw()
        {
            if (!IsVisible)
                return;
            var flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.AlwaysAutoResize;
            ImGui.SetNextWindowSizeConstraints(new Vector2(250, 100), new Vector2(400, 300));
            ImGui.Begin("config", flags);
            if (ImGui.SliderFloat("Opacity", ref opacity, 0.0f, 1.0f))
            {
                config.Opacity = opacity;
            }
            if (ImGui.Checkbox("Enable clickthrough", ref isClickthrough))
            {
                config.IsClickthrough = isClickthrough;
            }
            if (ImGui.Checkbox("Hide vulnerabilities that can't be inflicted", ref HideRedVulns))
            {
                config.HideRedVulns = HideRedVulns;
            }
            if (ImGui.Checkbox("Hide vulnerabilities based on current class/job", ref HideBasedOnJob))
            {
                config.HideBasedOnJob = HideBasedOnJob;
            }
            ImGui.NewLine();
            if (ImGui.Button("Save"))
            {
                IsVisible = false;
                config.Save();
            }
            ImGui.SameLine();
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos(400f);
                ImGui.TextWrapped("Thanks to the Deep Dungeons Discord server for a lot of community resources. Thanks to everyone who's taken the time to report incorrect or missing data! Special shoutouts to Maygi for writing the best Deep Dungeon guides out there!");
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }; 
            ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.Button, 0xFF5E5BFF);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0xFF5E5BAA);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0xFF5E5BDD);
            ImGui.PopStyleColor(3);
            ImGui.End();
        }
    }
}
