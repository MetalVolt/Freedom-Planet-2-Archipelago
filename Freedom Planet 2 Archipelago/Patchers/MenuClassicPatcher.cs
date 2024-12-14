namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuClassicPatcher
    {
        /// <summary>
        /// Overwrite the value from the Stage Reveal Check function to always return -1. This stops the map from unlocking stages in its normal progression.
        /// TODO: Find a way to make this unlock stages if a Progressive Chapter comes in while on the map.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuClassic), "StageRevealCheck")]
        static bool StopMapUnlockEffect(ref int __result)
        {
            __result = -1;
            return false;
        }
    }
}
