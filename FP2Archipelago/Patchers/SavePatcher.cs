using BepInEx;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace FP2Archipelago.Patchers
{
    // Taken from FP2Lib mostly unmodified https://github.com/Kuborros/FP2Lib/blob/master/FP2Lib/Saves/SavePatches.cs.
    // TODO: Can I use this code?
    internal class SavePatches
    {
        static string getSavesPath() => $@"{Paths.GameRootPath}\Archipelago Saves";

        static string fancifyJson(UnityEngine.Object obj) => JsonUtility.ToJson(obj, true);

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPSaveManager), "SaveToFile", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PatchJsonStyle(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && (codes[i - 1].opcode == OpCodes.Ldloc_0 || codes[i - 1].opcode == OpCodes.Ldloc_1) && codes[i - 2].opcode == OpCodes.Stfld)
                {
                    codes[i] = Transpilers.EmitDelegate(fancifyJson);
                }
            }
            return codes;
        }


        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPSaveManager), "SaveToFile", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PatchSaveWrite(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i - 1].opcode == OpCodes.Ldc_I4_0 && codes[i - 2].opcode == OpCodes.Dup)
                {
                    codes[i] = Transpilers.EmitDelegate(getSavesPath);
                }
            }
            return codes;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPSaveManager), "LoadFromFile", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PatchSaveLoad(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i - 1].opcode == OpCodes.Ldc_I4_0 && codes[i - 2].opcode == OpCodes.Dup)
                {
                    codes[i] = Transpilers.EmitDelegate(getSavesPath);
                }
            }
            return codes;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPSaveManager), "DeleteFile", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PatchSaveDelete(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i - 1].opcode == OpCodes.Ldc_I4_0 && codes[i - 2].opcode == OpCodes.Dup)
                {
                    codes[i] = Transpilers.EmitDelegate(getSavesPath);
                }
            }
            return codes;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MenuFile), "GetFileInfo", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PatchFileInfo(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i - 1].opcode == OpCodes.Ldc_I4_0 && codes[i - 2].opcode == OpCodes.Dup)
                {
                    codes[i] = Transpilers.EmitDelegate(getSavesPath);
                }
            }
            return codes;
        }

    }
}
