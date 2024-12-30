using UnityEngine.SceneManagement;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class PlayerSpawnPointPatcher
    {
        /// <summary>
        /// Changes the character on the save file when a player spawn is created to change who gets played as.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerSpawnPoint), "Start")]
        static void RandomiseCharacter()
        {
            // Check if our slot data's character value is set to 4.
            // Also check that we aren't in a Basic Tutorial, as there's versions for each character.
            if ((long)Plugin.SlotData["character"] == 4 && !SceneManager.GetActiveScene().name.Contains("Tutorial1"))
            {
                // Generate a random number between from 0 to 3 then set the save file's character based on the selected number.
                switch (Plugin.Randomiser.Next(0, 4))
                {
                    case 0: FPSaveManager.character = FPCharacterID.LILAC; break;
                    case 1: FPSaveManager.character = FPCharacterID.CAROL; break;
                    case 2: FPSaveManager.character = FPCharacterID.MILLA; break;
                    case 3: FPSaveManager.character = FPCharacterID.NEERA; break;
                }
            }
        }
    }
}
