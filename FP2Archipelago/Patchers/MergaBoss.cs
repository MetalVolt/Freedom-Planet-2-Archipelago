namespace FP2Archipelago.Patchers
{
    internal class MergaBoss
    {
        /// <summary>
        /// Nulls out the two cutscene values on Merga so that the ending doesn't play.
        /// </summary>
        /// <param name="___cutsceneOnVictory"></param>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerBossMerga), "State_KO2")]
        static void DisableEnding(ref FPBaseObject[] ___cutsceneOnVictory)
        {
            ___cutsceneOnVictory[0] = null;
            ___cutsceneOnVictory[1] = null;
        }
    }
}
