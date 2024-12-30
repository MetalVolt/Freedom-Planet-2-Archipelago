using UnityEngine;
using UnityEngine.SceneManagement;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuSpawnerPatcher
    {
        /// <summary>
        /// Deactivates the Debug Menu and copies its textTime TextMesh to create our labels and login textboxes.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuStageDebug), "Start")]
        static void DeleteDebugMenu(ref TextMesh ___textTime)
        {
            // Deactivate the actual Debug Menu.
            UnityEngine.Object.FindObjectOfType<MenuStageDebug>().gameObject.SetActive(false);

            // Create and configure our copies of the TextMesh.
            SpawnTextMesh(___textTime, new Vector3(86, -16, 0), "Freedom Planet 2 Archipelago Connection");
            SpawnTextMesh(___textTime, new Vector3(15, -40, 0), "Host:");
            SpawnTextMesh(___textTime, new Vector3(76, -40, 0), $"[{Plugin.serverAddress}]", "hostname");
            SpawnTextMesh(___textTime, new Vector3(15, -64, 0), "Slot:");
            SpawnTextMesh(___textTime, new Vector3(76, -64, 0), $"[{Plugin.slotName}]", "slotname");
            SpawnTextMesh(___textTime, new Vector3(15, -88, 0), "Password:");
            SpawnTextMesh(___textTime, new Vector3(124, -88, 0), $"[{Plugin.password}]", "passwordname");
        }

        /// <summary>
        /// Creates a copy of a TextMesh stolen from the Debug Menu.
        /// </summary>
        /// <param name="___textTime">The actual TextMesh referenced from the Debug Menu.</param>
        /// <param name="position">The position to place this TextMesh at on the screen.</param>
        /// <param name="text">The text that this TextMesh should display.</param>
        /// <param name="name">The name of this TextMesh object, if needed.</param>
        private static void SpawnTextMesh(TextMesh ___textTime, Vector3 position, string text, string name = null)
        {
            // Create this TextMesh.
            TextMesh textMesh = UnityEngine.Object.Instantiate(___textTime);

            // Set the position of this TextMesh.
            textMesh.gameObject.transform.position = position;

            // Set the text on this TextMesh.
            textMesh.text = text;

            // If a name is specified, then set it.
            if (name != null)
                textMesh.name = name;
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
