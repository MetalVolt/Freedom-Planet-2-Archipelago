using BepInEx;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FP2Archipelago.Patchers
{
    internal class ShopMenu
    {
        /// <summary>
        /// Shows the items that the multiworld has placed in the shop(s).
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuShop), "UpdateItemList", new Type[] { typeof(bool) })]
        static void ShowAPItemInShop(ref int ___currentDetail, ref int ___detailListOffset, ref FPHudDigit[] ___powerups, ref FPHudDigit[] ___vinyls,
                                     ref FPPowerup[] ___itemsForSale, ref MenuText[] ___detailName, ref MenuText ___itemDescription, ref int[] ___itemCosts)
        {
            // Get the name of name of the item that's normally here.
            string originalItem = ___detailName[0].GetComponent<TextMesh>().text;

            // Calculated the selected item.
            int selectedItem = ___currentDetail + ___detailListOffset;

            // Determine if this is Milla's Item Shop or the Vinyl Shop.
            bool isItemShop = true;
            if (___itemsForSale[0] == FPPowerup.NONE)
                isItemShop = false;

            // Handle the item shop.
            if (isItemShop && GlobalValues.Locations.ContainsKey("Shop - Element Burst"))
            {
                // Loop through each icon.
                for (int powerupIndex = 0; powerupIndex < ___powerups.Length; powerupIndex++)
                {
                    // Check if this icon is not 1 (the ? icon).
                    if (___powerups[powerupIndex].digitValue != 1)
                    {
                        // Read and insert a custom sprite at position 37.
                        ___powerups[powerupIndex].digitFrames[37] = Plugin.GetItemSprite(GlobalValues.MillaShopItems[___detailListOffset + powerupIndex].Game, GlobalValues.MillaShopItems[___detailListOffset + powerupIndex].Item);

                        // Set this icon to slot 37.
                        ___powerups[powerupIndex].SetDigitValue(37);
                    }
                }

                // Check if the displayed item name is not the ? ? ? ? ? string.
                if (___detailName[0].GetComponent<TextMesh>().text != "? ? ? ? ?")
                {
                    // Check if the item in this slot is for another player in the multiworld.
                    if (GlobalValues.MillaShopItems[selectedItem].Player != GlobalValues.Session.Players.GetPlayerName(GlobalValues.Session.ConnectionInfo.Slot))
                    {
                        // Show the name of the item, as well as the player it's for.
                        ___detailName[0].GetComponent<TextMesh>().text = $"{GlobalValues.MillaShopItems[selectedItem].Player}'s {GlobalValues.MillaShopItems[selectedItem].Item}";

                        // Set the description of our item.
                        ___itemDescription.GetComponent<TextMesh>().text = FPStage.WrapText(GetItemDescription(GlobalValues.MillaShopItems[selectedItem].Game, GlobalValues.MillaShopItems[selectedItem].Item, $"An item for {GlobalValues.MillaShopItems[selectedItem].Player}'s {GlobalValues.MillaShopItems[selectedItem].Game}."), 40) + "\r\nShop - " + originalItem;
                    }
                    else
                    {
                        // Show the name of our item.
                        ___detailName[0].GetComponent<TextMesh>().text = $"{GlobalValues.MillaShopItems[selectedItem].Item}";

                        // Set the description of our item.
                        ___itemDescription.GetComponent<TextMesh>().text = FPStage.WrapText(GetItemDescription(GlobalValues.MillaShopItems[selectedItem].Game, GlobalValues.MillaShopItems[selectedItem].Item, "Put the right item description here."), 40) + "\r\nShop - " + originalItem;
                    }
                }

                // Loop through and set each item to only cost 1 Gold Gem.
                for (int costIndex = 0; costIndex < ___itemCosts.Length; costIndex++)
                    ___itemCosts[costIndex] = 1;
            }

            // Handle the vinyl shop.
            if (!isItemShop && GlobalValues.Locations.ContainsKey("Shop - Vinyl - Title Screen"))
            {
                // Loop through each icon.
                for (int vinylIndex = 0; vinylIndex < ___vinyls.Length; vinylIndex++)
                {
                    // Check if this icon is 0 (the ? icon).
                    if (___vinyls[vinylIndex].digitValue != 0)
                    {
                        // Read and insert a custom sprite at position 10.
                        ___vinyls[vinylIndex].digitFrames[10] = Plugin.GetItemSprite(GlobalValues.VinylShopItems[___detailListOffset + vinylIndex].Game, GlobalValues.VinylShopItems[___detailListOffset + vinylIndex].Item);

                        // Set this icon to slot 10.
                        ___vinyls[vinylIndex].SetDigitValue(10);
                    }
                }

                // Check if the displayed item name is not the ? ? ? ? ? string.
                if (___detailName[0].GetComponent<TextMesh>().text != "? ? ? ? ?")
                {
                    // Check if the item in this slot is for another player in the multiworld.
                    if (GlobalValues.VinylShopItems[selectedItem].Player != GlobalValues.Session.Players.GetPlayerName(GlobalValues.Session.ConnectionInfo.Slot))
                    {
                        // Show the name of the item, as well as the player it's for.
                        ___detailName[0].GetComponent<TextMesh>().text = $"{GlobalValues.VinylShopItems[selectedItem].Player}'s {GlobalValues.VinylShopItems[selectedItem].Item}";

                        // Set the description of our item.
                        ___itemDescription.GetComponent<TextMesh>().text = FPStage.WrapText(GetItemDescription(GlobalValues.VinylShopItems[selectedItem].Game,GlobalValues.VinylShopItems[selectedItem].Item, $"An item for {GlobalValues.VinylShopItems[selectedItem].Player}'s {GlobalValues.VinylShopItems[selectedItem].Game}."), 40) + "\r\nShop - Vinyl - " + originalItem;
                    }
                    else
                    {
                        // Show the name of our item.
                        ___detailName[0].GetComponent<TextMesh>().text = $"{GlobalValues.VinylShopItems[selectedItem].Item}";

                        // Set the description of our item.
                        ___itemDescription.GetComponent<TextMesh>().text = FPStage.WrapText(GetItemDescription(GlobalValues.VinylShopItems[selectedItem].Game, GlobalValues.VinylShopItems[selectedItem].Item, "Put the right item description here."), 40) + "\r\nShop - Vinyl - " + originalItem;
                    }
                }

                // Loop through and set each item to only cost 100 Gems.
                for (int costIndex = 0; costIndex < ___itemCosts.Length; costIndex++)
                    ___itemCosts[costIndex] = 100;
            }
        }

        /// <summary>
        /// Resorts the list in Milla's shop if its used.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuShop), "SortItems")]
        static void ResortItemList()
        {
            if (GlobalValues.Locations.ContainsKey("Shop - Element Burst"))
                GlobalValues.MillaShopItems = Resort(GlobalValues.MillaShopItems);
        }

        /// <summary>
        /// Resorts the list in the Vinyl shop if its used.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuShop), "SortItems")]
        static void ResortVinylList()
        {
            if (GlobalValues.Locations.ContainsKey("Shop - Vinyl - Title Screen"))
                GlobalValues.VinylShopItems = Resort(GlobalValues.VinylShopItems);
        }

        /// <summary>
        /// Force the game to save upon buying an item (maybe this'll fix the shop being out of whack idk man).
        /// TODO: Update the message and icon shown on this.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuItemGet), "Start")]
        static void ForceSaveUponBuy() => FPSaveManager.SaveToFile(FPSaveManager.fileSlot);

        /// <summary>
        /// Sorts a list of locations so that checked ones are placed on the bottom.
        /// </summary>
        /// <param name="locations">The list to sort.</param>
        /// <returns>The sorted list.</returns>
        private static List<LocationContents> Resort(List<LocationContents> locations)
        {
            // Set up arrays to sort with.
            LocationContents[] checkedItems = new LocationContents[locations.Count];
            LocationContents[] uncheckedItems = new LocationContents[locations.Count];

            // Set up an index value.
            int index = 0;

            // Loop through each item in the sort list.
            for (int i = 0; i < locations.Count; i++)
            {
                // Check if the item has been obtained.
                if (locations[i].Checked)
                {
                    // Add this item to the checkedItems array at the current index.
                    checkedItems[index] = (locations[i]);

                    // Increment our index.
                    index++;
                }
            }

            // Reset our index.
            index = 0;

            // Loop through each item in the sort list.
            for (int i = 0; i < locations.Count; i++)
            {
                // Check if the item ISN'T obtained.
                if (!locations[i].Checked)
                {
                    // Add this item to the uncheckedItems array at the current index.
                    uncheckedItems[index] = locations[i];

                    // Increment our index.
                    index++;
                }
            }

            // Set up a new index.
            int index2 = index;

            // Loop through and add each checked item to the end of the unchecked item array.
            for (; index < locations.Count; index++)
                uncheckedItems[index] = checkedItems[index - index2];

            // Return the sorted array as a list.
            return uncheckedItems.ToList();
        }

        /// <summary>
        /// Get a description for the given item.
        /// </summary>
        /// <param name="game">The game this item is for.</param>
        /// <param name="item">The name of the item.</param>
        /// <param name="defaultDescription">The description to fall back on if we don't have one in this chunk.</param>
        /// <returns>The description we want.</returns>
        private static string GetItemDescription(string game, string item, string defaultDescription)
        {
            // Check if this item is for Freedom Planet 2.
            // If so, then either read the game's own descriptions, or read my own.
            if (game == "Manual_FreedomPlanet2_Knuxfan24")
            {
                switch (item)
                {
                    case "Gold Gem": return "Currency in Milla's shop.";
                    case "Star Card": return "Keys for unlocking distant lands.";
                    case "Time Capsule": return "Cordelia's convenient vlogs that somehow act as the key to Bakunawa's core because yes.";
                    case "Extra Item Slot": return "Allows equipping an extra Brave Stone.";
                    case "Extra Potion Slots": return "Allows equipping two extra Potions.";
                    case "Potion - Extra Stock": return FPSaveManager.GetItemDescription(FPPowerup.EXTRA_STOCK);
                    case "Potion - Strong Revivals": return FPSaveManager.GetItemDescription(FPPowerup.STRONG_REVIVALS);
                    case "Potion - Cheaper Stocks": return FPSaveManager.GetItemDescription(FPPowerup.CHEAPER_STOCKS);
                    case "Potion - Healing Strike": return FPSaveManager.GetItemDescription(FPPowerup.REGENERATION);
                    case "Potion - Attack Up": return FPSaveManager.GetItemDescription(FPPowerup.ATTACK_UP);
                    case "Potion - Strong Shields": return FPSaveManager.GetItemDescription(FPPowerup.STRONG_SHIELDS);
                    case "Potion - Accelerator": return FPSaveManager.GetItemDescription(FPPowerup.SPEED_UP);
                    case "Potion - Super Feather": return FPSaveManager.GetItemDescription(FPPowerup.JUMP_UP);
                    case "Element Burst": return FPSaveManager.GetItemDescription(FPPowerup.ELEMENT_BURST);
                    case "Max Life Up": return FPSaveManager.GetItemDescription(FPPowerup.MAX_LIFE_UP);
                    case "Crystals to Petals": return FPSaveManager.GetItemDescription(FPPowerup.MORE_PETALS);
                    case "Powerup Start": return FPSaveManager.GetItemDescription(FPPowerup.POWERUP_START);
                    case "Shadow Guard": return FPSaveManager.GetItemDescription(FPPowerup.SHADOW_GUARD);
                    case "Payback Ring": return FPSaveManager.GetItemDescription(FPPowerup.PAYBACK_RING);
                    case "Wood Charm": return FPSaveManager.GetItemDescription(FPPowerup.WOOD_CHARM);
                    case "Earth Charm": return FPSaveManager.GetItemDescription(FPPowerup.EARTH_CHARM);
                    case "Water Charm": return FPSaveManager.GetItemDescription(FPPowerup.WATER_CHARM);
                    case "Fire Charm": return FPSaveManager.GetItemDescription(FPPowerup.FIRE_CHARM);
                    case "Metal Charm": return FPSaveManager.GetItemDescription(FPPowerup.METAL_CHARM);
                    case "No Stocks": return FPSaveManager.GetItemDescription(FPPowerup.STOCK_DRAIN);
                    case "Expensive Stocks": return FPSaveManager.GetItemDescription(FPPowerup.PRICY_STOCKS);
                    case "Double Damage": return FPSaveManager.GetItemDescription(FPPowerup.DOUBLE_DAMAGE);
                    case "No Revivals": return FPSaveManager.GetItemDescription(FPPowerup.NO_REVIVALS);
                    case "No Guarding": return FPSaveManager.GetItemDescription(FPPowerup.NO_GUARDING);
                    case "No Petals": return FPSaveManager.GetItemDescription(FPPowerup.NO_PETALS);
                    case "Time Limit": return FPSaveManager.GetItemDescription(FPPowerup.TIME_LIMIT);
                    case "Items To Bombs": return FPSaveManager.GetItemDescription(FPPowerup.ITEMS_TO_BOMBS);
                    case "Life Oscillation": return FPSaveManager.GetItemDescription(FPPowerup.BIPOLAR_LIFE);
                    case "One Hit KO": return FPSaveManager.GetItemDescription(FPPowerup.ONE_HIT_KO);
                    case "Petal Armor": return FPSaveManager.GetItemDescription(FPPowerup.PETAL_ARMOR);
                    case "Rainbow Charm": return FPSaveManager.GetItemDescription(FPPowerup.RAINBOW_CHARM);
                    case "Sky Pirate Panic": return "Allows access to Avian Museum and Airship Sigwada.";
                    case "Enter the Battlesphere": return "Allows access to Phoenix Highway, Zao Land and The Battlesphere.";
                    case "Mystery of the Frozen North": return "Allows access to Tiger Falls, Robot Graveyard, Shade Armory and Snowfields.";
                    case "Globe Opera": return "Allows access to Globe Opera 1, Globe Opera 2, Auditorium, Palace Courtyard and Tidal Gate.";
                    case "Robot Wars! Snake VS Tarsier": return "Allows access to Zulon Jungle and Nalao Lake.";
                    case "Echoes of the Dragon War": return "Allows access to Ancestral Forge, Magma Starscape and Diamond Point.";
                    case "Justice in the Sky Paradise": return "Allows access to Sky Bridge and Lightning Tower.";
                    case "Bakunawa": return "Allows access to Gravity Bubble, Bakunawa Chase, Bakunawa Rush, Refinery Room, Clockwork Arboretum, Inversion Dynamo, Lunar Cannon, Merga and Weapon's Core.";
                    case "Progressive Chapter": return "Unlocks the next chapter's set of stages.";
                    case "Mirror Trap": return "Flips the stage horizontally.";
                    case "Moon Gravity Trap": return "Halves the current gravity for the player.";
                    case "Double Gravity Trap": return "Doubles the current gravity for the player.";
                    case "Ice Trap": return "Reduces the player's traction by a quarter.";
                    case "Reverse Trap": return "Reverses player controls for 30 seconds.";
                }
            }
            
            // If this isn't a Freedom Planet 2 item, then handle getting its description.
            else
            {
                // Check if a descriptions.txt file exists for this game.
                if (File.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\{game}\descriptions.txt"))
                {
                    // Read this description file.
                    string[] descriptions = File.ReadAllLines($@"{Paths.GameRootPath}\mod_overrides\Archipelago\{game}\descriptions.txt");

                    // Loop through each line in this description file. If it starts with this item's name, then split the first comma and return the rest.
                    foreach (string line in descriptions)
                        if (line.StartsWith(item))
                            return line.Substring(line.IndexOf(',') + 1);
                }
            }

            // Return the default description.
            return defaultDescription;
        }
    }
}
