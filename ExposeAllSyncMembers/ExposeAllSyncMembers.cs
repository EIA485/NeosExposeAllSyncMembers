using HarmonyLib;
using NeosModLoader;
using FrooxEngine.LogiX;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ExposeAllSyncMembers
{
	public class ExposeAllSyncMembers : NeosMod
	{
		public override string Name => "ExposeAllSyncMembers";
		public override string Author => "eia485";
		public override string Version => "1.0.0";
		public override string Link => "https://github.com/EIA485/NeosExposeAllSyncMembers/";
		public override void OnEngineInit()
		{
			Harmony harmony = new Harmony("net.eia485.ExposeAllSyncMembers");
			harmony.PatchAll();
		}

		[HarmonyPatch(typeof(LogixInterfaceProxy))]
		class ExposeAllSyncMembersPatch
		{
			[HarmonyTranspiler]
			[HarmonyPatch("GetInterface")]
			static IEnumerable<CodeInstruction> GetInterfaceTranspiler(IEnumerable<CodeInstruction> instructions)
			{
				var codes = new List<CodeInstruction>(instructions);

				for (int i = 8; i < codes.Count - 12; i++)
				{
					if (codes[i].opcode == OpCodes.Ldstr & codes[i].operand == "Unsupported member type: ")
					{

						codes[i + 3] = codes[i + 2];
						codes[i + 2] = codes[i + 1];
						codes[i + 1] = new CodeInstruction(opcode: OpCodes.Pop);
						codes[i + 4] = codes[i - 8];
						codes[i + 5] = codes[i - 1];

						for (int si = i + 6; si < i + 12; si++)
						{
							codes[si] = new CodeInstruction(opcode: OpCodes.Nop);
						}

						break;
					}
				}
				return codes;
			}
			[HarmonyPrefix]
			[HarmonyPatch("ListMember")]
			static bool ListMemberPrefix(ref bool __result)
			{
				__result = true;
				return false;
			}

		}
	}
}