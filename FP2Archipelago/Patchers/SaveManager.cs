namespace FP2Archipelago.Patchers
{
    public class SaveManager
    {
        /// <summary>
        /// Replaces the Save Manager's Total Star Cards calculation with the amount from the multiworld.
        /// </summary>
        [HarmonyPatch(typeof(FPSaveManager), nameof(FPSaveManager.TotalStarCards))]
        static void Postfix(ref int __result) => __result = GlobalValues.StarCards;

        /// <summary>
        /// Stops the Save Manager from unequipping items that haven't been acquired in the shop.
        /// </summary>
        [HarmonyPatch(typeof(FPSaveManager), nameof(FPSaveManager.SanitizeItemSets))]
        static bool Prefix() => false;
    }
}
