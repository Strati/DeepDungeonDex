using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DeepDungeonDex.Data;
using ImGuiNET;

namespace DeepDungeonDex.UI
{
    public class PluginUI
    {
        public bool IsVisible { get; set; }

        public PluginUI() { }

        private void PrintSingleVuln(bool? isVulnerable, string message, bool? jobOkay = null)
        {
            if (jobOkay == false && Plugin.Config.HideBasedOnJob)
                return;

            switch (isVulnerable)
            {
                case true:
                    ImGui.PushStyleColor(ImGuiCol.Text, 0xFF00FF00);
                    ImGui.Text(message);
                    ImGui.PopStyleColor();
                    break;
                case false:
                    if (!Plugin.Config.HideRedVulns)
                    {
                        ImGui.PushStyleColor(ImGuiCol.Text, 0xFF0000FF);
                        ImGui.Text(message);
                        ImGui.PopStyleColor();
                    }
                    break;
                default:
                    ImGui.PushStyleColor(ImGuiCol.Text, 0x50FFFFFF);
                    ImGui.Text(message);
                    ImGui.PopStyleColor();
                    break;
            }
        }

        public void Draw()
        {
            if (!IsVisible)
                return;

            var mobData = DataRepo.GetMob(TargetData.NameID);
            if (mobData == null) return;

            var cjid = Plugin.ClientState.LocalPlayer?.ClassJob.GameData?.RowId ?? 0;
            var jobData = DataRepo.GetJob(cjid);

            var flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoTitleBar;
            if (Plugin.Config.IsClickthrough)
            {
                flags |= ImGuiWindowFlags.NoInputs;
            }
            ImGui.SetNextWindowSizeConstraints(new Vector2(250, 0), new Vector2(9001, 9001));
            ImGui.SetNextWindowBgAlpha(Plugin.Config.Opacity);

            ImGui.Begin("cool strati window", flags);
            ImGui.Text("Name:\n"+TargetData.Name);
            ImGui.NewLine();
            ImGui.Columns(3, null, false);
            ImGui.Text("Aggro Type:\n");
            ImGui.Text(mobData.Aggro.ToString());
            ImGui.NextColumn();
            ImGui.Text("Threat:\n");
            switch (mobData.Threat)
            {
                case MobData.ThreatLevel.Easy:
                    ImGui.PushStyleColor(ImGuiCol.Text, 0xFF00FF00);
                    ImGui.Text("Easy");
                    ImGui.PopStyleColor();
                    break;
                case MobData.ThreatLevel.Caution:
                    ImGui.PushStyleColor(ImGuiCol.Text, 0xFF00FFFF);
                    ImGui.Text("Caution");
                    ImGui.PopStyleColor();
                    break;
                case MobData.ThreatLevel.Dangerous:
                    ImGui.PushStyleColor(ImGuiCol.Text, 0xFF0000FF);
                    ImGui.Text("Dangerous");
                    ImGui.PopStyleColor();
                    break;
                case MobData.ThreatLevel.Vicious:
                    ImGui.PushStyleColor(ImGuiCol.Text, 0xFFFF00FF);
                    ImGui.Text("Vicious");
                    ImGui.PopStyleColor();
                    break;
                default:
                    ImGui.Text("Undefined");
                    break;
            }
            ImGui.NextColumn();

            PrintSingleVuln(mobData.Vuln.CanStun, "Stun", jobData?.CanStun);
            PrintSingleVuln(mobData.Vuln.CanSleep, "Sleep", jobData?.CanSleep);
            PrintSingleVuln(mobData.Vuln.CanBind, "Bind", jobData?.CanBind);
            PrintSingleVuln(mobData.Vuln.CanHeavy, "Heavy", jobData?.CanHeavy);
            PrintSingleVuln(mobData.Vuln.CanSlow, "Slow", jobData?.CanSlow);

            if (!(TargetData.NameID >= 7262 && TargetData.NameID <= 7610))
            {
                PrintSingleVuln(mobData.Vuln.IsUndead, "Undead");
            }

            ImGui.NextColumn();
            ImGui.Columns(1);
            ImGui.NewLine();
            ImGui.TextWrapped(mobData.MobNotes);
            ImGui.End();
        }
    }
}
