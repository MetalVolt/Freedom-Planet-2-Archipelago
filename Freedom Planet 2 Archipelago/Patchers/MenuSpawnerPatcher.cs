using UnityEngine.SceneManagement;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuSpawnerPatcher
    {
        /// <summary>
        /// Detects whether the active scene is the Stage Debug Menu one and, if so, stops the MenuSpawner's start function from running.
        /// </summary>
        /// <returns>Whether or not to run the MenuSpawner's start function.</returns>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuSpawner), "Start")]
        static bool DeleteDebugMenu()
        {
            if (SceneManager.GetActiveScene().name == "StageDebugMenu")
                return false;
            else
                return true;
        }
    }
}
