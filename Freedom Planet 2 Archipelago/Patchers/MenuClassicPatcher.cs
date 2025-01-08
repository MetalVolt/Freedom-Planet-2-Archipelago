using System.Collections.Generic;
using System.Reflection;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuClassicPatcher
    {
        /// <summary>
        /// The tile sprites gathered from the world map, used to swap things around.
        /// </summary>
        static UnityEngine.Sprite[] tileSprites = [];

        /// <summary>
        /// Overwrite the value from the Stage Reveal Check function to always return -1. This stops the map from unlocking stages in its normal progression.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuClassic), "StageRevealCheck")]
        static bool StopMapUnlockEffect(ref int __result)
        {
            __result = -1;
            return false;
        }

        /// <summary>
        /// Stops the Time Capsule scene after Tidal Gate from playing.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuClassic), "State_Intro")]
        static bool StopTimeCapsuleScene()
        {
            // Get a reference to this menu.
            MenuClassic menu = UnityEngine.Object.FindObjectOfType<MenuClassic>();

            // Get and run the State_Default function to force the state to change.
            MethodInfo function = typeof(MenuClassic).GetMethod("State_Default", BindingFlags.NonPublic | BindingFlags.Instance);
            function.Invoke(menu, new object[] { });

            // Stop the original Intro state from running.
            return false;
        }

        /// <summary>
        /// Set the stage's Star Card requirements and replace their icon sprites with the lock one.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuClassic), "Start")]
        static void SetStageLocks(ref bool ___newMusic, ref MenuClassicTile[] ___stages)
        {
            // Actually play the map music after State_Intro being cut short stops it from doing so.
            ___newMusic = true;

            // Set the Star Card requirements for everything past Globe Opera 1 and Gravity Bubble, as only those two stages actually have one.
            ___stages[12].starCardRequirement = 11;
            ___stages[13].starCardRequirement = 11;
            ___stages[14].starCardRequirement = 11;
            ___stages[15].starCardRequirement = 11;
            ___stages[16].starCardRequirement = 11;
            ___stages[17].starCardRequirement = 11;
            ___stages[18].starCardRequirement = 11;
            ___stages[19].starCardRequirement = 11;
            ___stages[20].starCardRequirement = 11;
            ___stages[21].starCardRequirement = 11;
            ___stages[22].starCardRequirement = 11;
            ___stages[24].starCardRequirement = 23;
            ___stages[25].starCardRequirement = 23;
            ___stages[26].starCardRequirement = 23;
            ___stages[27].starCardRequirement = 23;
            ___stages[28].starCardRequirement = 23;
            ___stages[29].starCardRequirement = 23;
            ___stages[32].starCardRequirement = 23;

            // Weapons Core uses starCardRequirement to count the Time Capsules, so only set it to 32 when we don't have enough cards.
            if (Plugin.APSave.StarCardCount < 32)
                ___stages[30].starCardRequirement = 32;

            // Initialise the tile sprites array.
            tileSprites = new UnityEngine.Sprite[___stages.Length];

            // Loop through each stage in the stage list.
            for (int stageIndex = 0; stageIndex < ___stages.Length; stageIndex++)
            {
                // Save this stage's sprite to the list.
                tileSprites[stageIndex] = ___stages[stageIndex].icon.sprite;

                // If this stage isn't Dragon Valley, Shenlin Park or the Basic Tutorial, then replace its sprite with Globe Opera's locked one.
                if (stageIndex != 0 && stageIndex != 1 && stageIndex != 31 && stageIndex != 33)
                    ___stages[stageIndex].icon.sprite = ___stages[11].lockedPanel.sprite;
            }
        }

        /// <summary>
        /// Handle the stage name and record display and reverting the icon change.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuClassic), "State_Default")]
        static void CheckForStageLocks(ref MenuClassicTile[] ___stages, ref MenuText ___textLocation, ref MenuText ___textRecord, ref int ___currentTile)
        {
            // Undo the edit to Weapons Core if we meet the Star Card requirement.
            if (Plugin.APSave.StarCardCount >= 32 && ___stages[30].starCardRequirement > 11)
                ___stages[30].starCardRequirement = 11;

            // Hide the stage name and time.
            ___textLocation.gameObject.SetActive(false);
            ___textRecord.gameObject.SetActive(false);

            // Loop through each stage in the stage list.
            for (int stageIndex = 0; stageIndex < ___stages.Length; stageIndex++)
            {
                switch (stageIndex)
                {
                    // Sky Pirate Panic.
                    case 2:
                    case 3:
                        if (Plugin.APSave.UnlockedChapters[1])
                            ___stages[stageIndex].icon.sprite = tileSprites[stageIndex];
                        break;

                    // Mystery of the Frozen North.
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                        if (Plugin.APSave.UnlockedChapters[0])
                            ___stages[stageIndex].icon.sprite = tileSprites[stageIndex];
                        break;

                    // Enter The Battlesphere
                    case 8:
                    case 9:
                    case 10:
                        if (Plugin.APSave.UnlockedChapters[2])
                            ___stages[stageIndex].icon.sprite = tileSprites[stageIndex];
                        break;

                    // Globe Opera.
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                        if (Plugin.APSave.UnlockedChapters[3] && Plugin.APSave.StarCardCount >= 11)
                            ___stages[stageIndex].icon.sprite = tileSprites[stageIndex];
                        break;

                    // Robot Wars! Snake VS Tarsier.
                    case 16:
                    case 17:
                        if (Plugin.APSave.UnlockedChapters[5] && Plugin.APSave.StarCardCount >= 11)
                            ___stages[stageIndex].icon.sprite = tileSprites[stageIndex];
                        break;

                    // Justice in the Sky Paradise.
                    case 18:
                    case 19:
                        if (Plugin.APSave.UnlockedChapters[4] && Plugin.APSave.StarCardCount >= 11)
                            ___stages[stageIndex].icon.sprite = tileSprites[stageIndex];
                        break;

                    // Echoes of the Dragon War.
                    case 20:
                    case 21:
                    case 22:
                        if (Plugin.APSave.UnlockedChapters[6] && Plugin.APSave.StarCardCount >= 11)
                            ___stages[stageIndex].icon.sprite = tileSprites[stageIndex];
                        break;

                    // Bakunawa.
                    case 23:
                    case 24:
                    case 25:
                    case 26:
                    case 27:
                    case 28:
                    case 29:
                    case 32:
                        if (Plugin.APSave.UnlockedChapters[7] && Plugin.APSave.StarCardCount >= 23)
                            ___stages[stageIndex].icon.sprite = tileSprites[stageIndex];
                        break;

                    // Weapons Core.
                    case 30:
                        if (Plugin.APSave.UnlockedChapters[7] && Plugin.APSave.StarCardCount >= 32)
                            ___stages[stageIndex].icon.sprite = tileSprites[stageIndex];
                        break;
                }
            }

            // Handle showing the stage name and record based on the selected stage.
            switch (___currentTile)
            {
                // Dragon Valley, Shenlin Park and the two tutorials.
                case 0:
                case 1:
                case 31:
                case 33:
                    ShowText(___textLocation, ___textRecord);
                    break;

                // Sky Pirate Panic.
                case 2:
                case 3:
                    if (Plugin.APSave.UnlockedChapters[1]) ShowText(___textLocation, ___textRecord);
                    break;

                // Mystery of the Frozen North.
                case 4:
                case 5:
                case 6:
                case 7:
                    if (Plugin.APSave.UnlockedChapters[0]) ShowText(___textLocation, ___textRecord);
                    break;

                // Enter The Battlesphere.
                case 8:
                case 9:
                case 10:
                    if (Plugin.APSave.UnlockedChapters[2]) ShowText(___textLocation, ___textRecord);
                    break;

                // Globe Opera.
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                    if (Plugin.APSave.UnlockedChapters[3] && Plugin.APSave.StarCardCount >= 11) ShowText(___textLocation, ___textRecord);
                    break;

                // Robot Wars! Snake VS Tarsier.
                case 16:
                case 17:
                    if (Plugin.APSave.UnlockedChapters[5] && Plugin.APSave.StarCardCount >= 11) ShowText(___textLocation, ___textRecord);
                    break;

                // Justice in the Sky Paradise.
                case 18:
                case 19:
                    if (Plugin.APSave.UnlockedChapters[4] && Plugin.APSave.StarCardCount >= 11) ShowText(___textLocation, ___textRecord);
                    break;

                // Echoes of the Dragon War.
                case 20:
                case 21:
                case 22:
                    if (Plugin.APSave.UnlockedChapters[6] && Plugin.APSave.StarCardCount >= 11) ShowText(___textLocation, ___textRecord);
                    break;

                // Bakunawa.
                case 23:
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                case 32:
                    if (Plugin.APSave.UnlockedChapters[7] && Plugin.APSave.StarCardCount >= 23) ShowText(___textLocation, ___textRecord);
                    break;

                // Weapons Core.
                case 30:
                    if (Plugin.APSave.UnlockedChapters[7] && Plugin.APSave.StarCardCount >= 32) ShowText(___textLocation, ___textRecord);
                    break;
            }
        }

        /// <summary>
        /// Makes the stage name and record text visible.
        /// </summary>
        private static void ShowText(MenuText ___textLocation, MenuText ___textRecord)
        {
            ___textLocation.gameObject.SetActive(true);
            ___textRecord.gameObject.SetActive(true);
        }

        /// <summary>
        /// Replaces the collectable and vinyl icons with chest ones based on if there are any unopened chests in the stage and if the chest tracer is obtained.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuClassic), "UpdateHeader")]
        private static void SetChestIcons(ref int ___currentTile, ref SpriteRenderer[] ___hudCollectibles, ref Sprite[] ___hudCollectibleSprites)
        {           
            // If the selected stage is one that doesn't have any chests, then remove the collectable sprites and return.
            if (___currentTile is 7 or 10 or 13 or 22 or 25 or 29 or 30 or 31 or 32 or 33)
            {
                ___hudCollectibles[1].sprite = null;
                ___hudCollectibles[2].sprite = null;
                return;
            }

            // Set the chest and vinyl sprites to the unobtained chest icon.
            ___hudCollectibles[1].sprite = ___hudCollectibleSprites[4];
            ___hudCollectibles[2].sprite = ___hudCollectibleSprites[4];

            // Handle highlighting the sprites depending on the ID of the selected tile.
            switch (___currentTile)
            {
                case 0: GetChests(0, ChestLineTables.DragonValley, ___hudCollectibles, ___hudCollectibleSprites); break;
                case 1: GetChests(1, ChestLineTables.ShenlinPark, ___hudCollectibles, ___hudCollectibleSprites); break;
                case 2: GetChests(2, ChestLineTables.AvianMuseum, ___hudCollectibles, ___hudCollectibleSprites); break;
                case 3: GetChests(3, ChestLineTables.AirshipSigwada, ___hudCollectibles, ___hudCollectibleSprites); break;
                case 4: GetChests(4, ChestLineTables.TigerFalls, ___hudCollectibles, ___hudCollectibleSprites); break;
                case 5: GetChests(5, ChestLineTables.RobotGraveyard, ___hudCollectibles, ___hudCollectibleSprites); break;
                case 6: GetChests(6, ChestLineTables.ShadeArmory, ___hudCollectibles, ___hudCollectibleSprites); break;
                case 8: GetChests(7, ChestLineTables.PhoenixHighway, ___hudCollectibles, ___hudCollectibleSprites); break;
                case 9: GetChests(8, ChestLineTables.ZaoLand, ___hudCollectibles, ___hudCollectibleSprites); break;
                case 11: GetChests(9, ChestLineTables.GlobeOpera1, ___hudCollectibles, ___hudCollectibleSprites); break;
                case 12: GetChests(10, ChestLineTables.GlobeOpera2, ___hudCollectibles, ___hudCollectibleSprites); break;
                case 14: GetChests(11, ChestLineTables.PalaceCourtyard, ___hudCollectibles, ___hudCollectibleSprites); break;
                case 15: GetChests(12, ChestLineTables.TidalGate, ___hudCollectibles, ___hudCollectibleSprites); break;
                case 16: GetChests(13, ChestLineTables.ZulonJungle, ___hudCollectibles, ___hudCollectibleSprites); break;
                case 17: GetChests(14, ChestLineTables.NalaoLake, ___hudCollectibles, ___hudCollectibleSprites); break;
                case 18: GetChests(15, ChestLineTables.SkyBridge, ___hudCollectibles, ___hudCollectibleSprites); break;
                case 19: GetChests(16, ChestLineTables.LightningTower, ___hudCollectibles, ___hudCollectibleSprites); break;
                case 20: GetChests(17, ChestLineTables.AncestralForge, ___hudCollectibles, ___hudCollectibleSprites); break;
                case 21: GetChests(18, ChestLineTables.MagmaStarscape, ___hudCollectibles, ___hudCollectibleSprites); break;
                case 23: GetChests(19, ChestLineTables.GravityBubble, ___hudCollectibles, ___hudCollectibleSprites); break;
                case 24: GetChests(20, ChestLineTables.BakunawaRush, ___hudCollectibles, ___hudCollectibleSprites); break;
                case 26: GetChests(21, ChestLineTables.ClockworkArboretum, ___hudCollectibles, ___hudCollectibleSprites); break;
                case 27: GetChests(22, ChestLineTables.InversionDynamo, ___hudCollectibles, ___hudCollectibleSprites); break;
                case 28: GetChests(23, ChestLineTables.LunarCannon, ___hudCollectibles, ___hudCollectibleSprites); break;
            }

            // Local function to check the chest locations from the look up tables and handle the sprites accordingly.
            void GetChests(int stageIndex, Dictionary<string, Vector3> table, SpriteRenderer[] ___hudCollectibles, Sprite[] ___hudCollectibleSprites)
            {
                // If this stage's Chest Tracer is obtained, then highlight it.
                if (Plugin.APSave.ChestTracers[stageIndex])
                    ___hudCollectibles[1].sprite = ___hudCollectibleSprites[5];

                // Loop through each entry in the given table.
                foreach (KeyValuePair<string, Vector3> entry in table)
                {
                    // Get the location for this entry.
                    Location location = Array.Find(Plugin.APSave.Locations, location => location.Name == entry.Key);

                    // Check that the location exists. If it does, but hasn't been checked, then return.
                    if (location != null)
                        if (!location.Checked)
                            return;
                }

                // Set the sprite of the second chest icon to the collected chest icon.
                ___hudCollectibles[2].sprite = ___hudCollectibleSprites[5];
            }
        }
    }
}
