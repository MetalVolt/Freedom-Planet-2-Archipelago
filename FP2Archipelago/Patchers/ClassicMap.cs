using Archipelago.MultiClient.Net.Models;

namespace FP2Archipelago.Patchers
{
    internal class ClassicMap
    {
        /// <summary>
        /// Get the items from the multiworld upon initially loading a file.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuClassic), "Start")]
        static void GetItemsFromMulti()
        {
            // Check if we haven't already recieved our items from the multiworld.
            if (!GlobalValues.RecievedFirstLoadItems)
            {
                // Loop through each item the server has sent and recieve it.
                foreach (NetworkItem item in GlobalValues.Session.Items.AllItemsReceived)
                {
                    Plugin.RecieveItem(GlobalValues.Session.Items.GetItemName(item.Item), true);
                    GlobalValues.Session.Items.DequeueItem();
                }

                // Set the recieved start items flag to true.
                GlobalValues.RecievedFirstLoadItems = true;

                // Remove any Gold Gems that have previously been recieved already.
                FPSaveManager.totalGoldGems -= GlobalValues.Items.GoldGems;
            }
        }

        /// <summary>
        /// Locks stages based on Star Card requirements.
        /// TODO: Require the Chapter Unlocks as well.
        /// TODO: Visual locks? Tried to replicate what Globe Opera 1, Bakunawa Chase and Weapon's Core do to no avail (even in the decomp).
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuClassic), "Start")]
        static void LockStages()
        {
            // Find the classic mode menu.
            MenuClassic menu = UnityEngine.Object.FindObjectOfType<MenuClassic>();

            // Check we actually found a menu to be safe.
            if (menu != null)
            {
                // Loop through each stage.
                foreach (MenuClassicTile stage in menu.stages)
                {
                    // Set values depending on the stage name.
                    switch (stage.icon.name)
                    {
                        // Mystery of the Frozen North.
                        case "StageIcon_Falls":
                        case "StageIcon_Graveyard":
                        case "StageIcon_Armory":
                        case "StageIcon_Snowfields":
                            break;

                        // Sky Pirate Panic.
                        case "StageIcon_Museum":
                        case "StageIcon_Sigwada":
                            break;
                        
                        // Enter The Battlesphere.
                        case "StageIcon_Highway":
                        case "StageIcon_ZaoLand":
                        case "StageIcon_Arena":
                            break;

                        // Globe Opera.
                        case "StageIcon_Opera1":
                        case "StageIcon_Opera2":
                        case "StageIcon_Auditorium":
                        case "StageIcon_Merga":
                        case "StageIcon_Gate":
                            stage.starCardRequirement = 7;
                            break;

                        // Justice in the Sky Paradise.
                        case "StageIcon_Bridge":
                        case "StageIcon_Tower":
                            stage.starCardRequirement = 7;
                            break;

                        // Robot Wars! Snake VS Tarsier.
                        case "StageIcon_Jungle":
                        case "StageIcon_Lake":
                            stage.starCardRequirement = 7;
                            break;

                        // Echoes of the Dragon War.
                        case "StageIcon_Forge":
                        case "StageIcon_Starscape":
                        case "StageIcon_Diamond":
                            stage.starCardRequirement = 7;
                            break;

                        // Bakunawa.
                        case "StageIcon_Bubble":
                        case "StageIcon_Chase":
                        case "StageIcon_Baku1":
                        case "StageIcon_Navigation":
                        case "StageIcon_Baku2":
                        case "StageIcon_Baku3":
                        case "StageIcon_Baku4":
                        case "StageIcon_Engineering":
                            stage.starCardRequirement = 18;
                            break;

                        // Weapon's Core.
                        case "StageIcon_Baku5":
                            stage.starCardRequirement = 25;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Hotkeys to add items, for when things inevitably go wrong.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuClassic), "Update")]
        static void AddItemDebug()
        {
            // Add/Remove Star Cards.
            if (Input.GetKeyDown(KeyCode.KeypadPlus)) GlobalValues.StarCards++;
            if (Input.GetKeyDown(KeyCode.KeypadMinus)) GlobalValues.StarCards--;

            // Add/Remove Crystals.
            if (Input.GetKeyDown(KeyCode.Equals)) FPSaveManager.totalCrystals += 500;
            if (Input.GetKeyDown(KeyCode.Minus)) FPSaveManager.totalCrystals -= 500;

            // Find the shop menu.
            MenuShop shopMenu = UnityEngine.Object.FindObjectOfType<MenuShop>();

            // If we found the shop menu, then update the displayed crystal count.
            if (shopMenu != null)
                shopMenu.playerCrystals.GetComponent<TextMesh>().text = FPSaveManager.totalCrystals.ToString();

            // Add Time Capsules.
            if (Input.GetKeyDown(KeyCode.KeypadMultiply) && GlobalValues.TimeCapsules < 13)
            {
                // Increment the Time Capsule count.
                GlobalValues.TimeCapsules++;

                // Update the save manager.
                for (int i = 0; i < GlobalValues.TimeCapsules; i++)
                    FPSaveManager.timeCapsules[i] = 1;

                // Print the count as the game is a bit wonky for showing it.
                Console.WriteLine($"Time Capsules: {GlobalValues.TimeCapsules}");
            }

            // Remove Time Capsules.
            if (Input.GetKeyDown(KeyCode.KeypadDivide) && GlobalValues.TimeCapsules > 0)
            {
                // Decrement the Time Capsule count.
                GlobalValues.TimeCapsules--;

                // Update the save manager.
                for (int i = GlobalValues.TimeCapsules - 1; i >= 0; i--)
                    FPSaveManager.timeCapsules[i] = 1;

                // Print the count as the game is a bit wonky for showing it.
                Console.WriteLine($"Time Capsules: {GlobalValues.TimeCapsules}");
            }
        }

        /// <summary>
        /// Unlocks Weapon's Core when the right stipulation is hit.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuClassic), "Update")]
        static void UnlockWeaponsCore()
        {
            // Check if Weapon's Core needs unlocking.
            if (GlobalValues.TimeCapsules >= 13 && GlobalValues.StarCards >= 25)
            {
                // Find the classic mode menu.
                MenuClassic menu = UnityEngine.Object.FindObjectOfType<MenuClassic>();

                // Check we actually found a menu, then loop the stage list until we find Weapon's Core and set its Star Card requirement to 0.
                if (menu != null)
                {
                    foreach (MenuClassicTile stage in menu.stages)
                    {
                        if (stage.icon.name == "StageIcon_Baku5")
                        {
                            stage.starCardRequirement = 0;
                            stage.needsTimeCapsules = false;
                        }
                    }
                }
            }
        }
    }
}
