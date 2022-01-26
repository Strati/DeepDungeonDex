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

        private bool PrintSingleVuln(ref bool? isVulnerable, string message, bool? jobOkay)
        {
            if (jobOkay == false && Plugin.Config.HideBasedOnJob)
                return false;

            if (isVulnerable == false && Plugin.Config.HideRedVulns)
                return false;

            var color = isVulnerable switch
            {
                true => 0xFF00FF00,
                false => 0xFF0000FF,
                _ => (uint)0x50FFFFFF
            };

            var dataChanged = false;

            ImGui.PushStyleColor(ImGuiCol.Text, color);
            
            if (ImGui.Selectable(message, false))
            {
                //move next state
                isVulnerable = MoveNextState(isVulnerable);
                dataChanged = true;
            }

            ImGui.PopStyleColor();

            return dataChanged;
        }

        private bool? MoveNextState(bool? state)
        {
            return state switch
            {
                true => false,
                false => null,
                _ => true
            };
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

            var dataChanged = false;

            dataChanged |= PrintSingleVuln(ref mobData.Vuln.CanStun, "Stun", jobData?.CanStun);
            dataChanged |= PrintSingleVuln(ref mobData.Vuln.CanSleep, "Sleep", jobData?.CanSleep);
            dataChanged |= PrintSingleVuln(ref mobData.Vuln.CanBind, "Bind", jobData?.CanBind);
            dataChanged |= PrintSingleVuln(ref mobData.Vuln.CanHeavy, "Heavy", jobData?.CanHeavy);
            dataChanged |= PrintSingleVuln(ref mobData.Vuln.CanSlow, "Slow", jobData?.CanSlow);

            if (!(TargetData.NameID >= 7262 && TargetData.NameID <= 7610))
            {
                dataChanged |= PrintSingleVuln(ref mobData.Vuln.IsUndead, "Undead", null);
            }

            if (dataChanged)
                DataRepo.SaveOverride(TargetData.NameID, mobData);

            ImGui.NextColumn();
            ImGui.Columns(1);
            ImGui.NewLine();
            ImGui.TextWrapped(mobData.MobNotes);
            ImGui.End();
        }
    }
}
