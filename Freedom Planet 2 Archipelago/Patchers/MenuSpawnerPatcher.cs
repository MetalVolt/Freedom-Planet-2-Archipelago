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

        /// <summary>
        /// Redirects the main menu to the debug menu.
        /// TODO: I'd like this to disconnect the player, but I can't seem to find a way to do that?
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuSpawner), "Start")]
        static void ReturnToConnectMenu()
        {
            // Check if we've been sent to the main menu for whatever reason.
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                // Load the Stage Debug Menu to act as a connector menu.
                SceneManager.LoadScene("StageDebugMenu");
            }
        }
    }
}
