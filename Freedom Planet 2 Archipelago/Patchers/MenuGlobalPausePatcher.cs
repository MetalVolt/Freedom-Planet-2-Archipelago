namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuGlobalPausePatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuGlobalPause), "Start")]
        static void ReplaceCoreDisplay(ref GameObject[] ___itemCounters, ref GameObject[] ___overviewCounters)
        {
            // Find and replace the sprite for the core count.
            // This feels very dumb.
            UnityEngine.GameObject.Find("MapPauseMenu(Clone)").transform
                                  .GetChild(6).transform
                                  .GetChild(0).transform
                                  .GetChild(2).transform
                                  .GetChild(6).transform
                                  .GetChild(3)
                                  .GetComponent<SpriteRenderer>().sprite
                                  =
            UnityEngine.GameObject.Find("MapPauseMenu(Clone)").transform
                                    .GetChild(6).transform
                                    .GetChild(0).transform
                                    .GetChild(2).transform
                                    .GetChild(10).transform
                                    .GetChild(3)
                                    .GetComponent<SpriteRenderer>().sprite;

            // Replace the count of the core counters with the Time Capsule count.
            ___overviewCounters[3].GetComponent<TextMesh>().text = FPSaveManager.TotalLogs().ToString();
            ___itemCounters[3].GetComponent<TextMesh>().text = FPSaveManager.TotalLogs().ToString();
        }
    }
}
