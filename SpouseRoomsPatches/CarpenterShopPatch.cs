using HarmonyLib;
using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SpouseRooms.carpenterpatch
{   
    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.carpenters))]
    public static class CarpenterShopTranspilerPatch
    {
        private static bool IsLoadLocal(CodeInstruction instr)
        {       
            return instr.opcode == OpCodes.Ldloc
                || instr.opcode == OpCodes.Ldloc_S
                || instr.opcode == OpCodes.Ldloc_0
                || instr.opcode == OpCodes.Ldloc_1
                || instr.opcode == OpCodes.Ldloc_2
                || instr.opcode == OpCodes.Ldloc_3;
        }

        private static readonly ConstructorInfo ResponseCtor =
            AccessTools.Constructor(typeof(Response), new[] { typeof(string), typeof(string) });

        private static readonly MethodInfo AddMethod =
            AccessTools.Method(typeof(List<Response>), nameof(List<Response>.Add));

        private static CodeInstruction[] MakeAddStack(CodeInstruction loadListInstr)
        {
            var loadForThis = new CodeInstruction(loadListInstr.opcode, loadListInstr.operand);
            
            return new[]
            {
                loadForThis,

                new CodeInstruction(OpCodes.Ldstr, "MoveSpouseRooms"),
                new CodeInstruction(OpCodes.Ldstr, "Move Spouse Rooms"),
                new CodeInstruction(OpCodes.Newobj, ResponseCtor),
                new CodeInstruction(OpCodes.Callvirt, AddMethod)
            };
        }
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var list = new List<CodeInstruction>(instructions);

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].opcode == OpCodes.Ldstr && (string)list[i].operand == "Leave")
                {
                    CodeInstruction? loadListInstr = null;

                    for (int j = i - 1; j >= 0; j--)
                    {
                        if(IsLoadLocal(list[j]))
                        {
                            loadListInstr = list[j];
                            break;
                        }
                    }
                    if (loadListInstr == null)
                        break;

                    list.InsertRange(i, MakeAddStack(loadListInstr!));
                    break;
                }
            }

            return list;
        }
    }
}




