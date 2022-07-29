using System.Runtime.InteropServices.ComTypes;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Text.SeStringHandling;

namespace DeepDungeonDex
{
	public class TargetData
	{
		public static uint NameID { get; set; }
		public static SeString Name { get; set; }

		public static void UpdateTargetData(GameObject target, out bool isValid)
		{
			if (target is BattleNpc bnpc)
			{
				Name = bnpc.Name;
				NameID = bnpc.NameId;

                isValid = true;
			}
			else
                isValid = false;
		}
	}
}