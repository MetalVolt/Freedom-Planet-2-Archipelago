using System.IO;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuShopPatcher
    {
        /// <summary>
        /// Stops the shop menu from sorting purchased items to the end of the list.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuShop), "SortItems")]
        static bool StopShopSorting() => false;

        /// <summary>
        /// Send out the hints for the avaliable shop items.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuShop), "Start")]
        static void SendHints(ref FPPowerup[] ___itemsForSale)
        {
            // Determine if this is Milla's Item Shop or the Vinyl Shop.
            bool isItemShop = true;
            if (___itemsForSale[0] == FPPowerup.NONE)
                isItemShop = false;

            // Send out the hints for the items that are unlocked.
            if (isItemShop && (long)Plugin.SlotData["milla_shop"] == 1)
            {
                SendHint("Shop - Element Burst");
                SendHint("Shop - Crystals To Petals");
                SendHint("Shop - Petal Armor");
                SendHint("Shop - Extra Stock");
                SendHint("Shop - Strong Revivals");
                SendHint("Shop - Cheaper Stocks");
                SendHint("Shop - Healing Strike");
                SendHint("Shop - Attack Up");

                if (FPSaveManager.TotalStarCards() >= 2)
                {
                    SendHint("Shop - Strong Shields");
                    SendHint("Shop - Accelerator");
                    SendHint("Shop - Super Feather");
                }

                if (FPSaveManager.TotalStarCards() >= 11)
                {
                    SendHint("Shop - Max Life Up");
                    SendHint("Shop - One Hit KO");
                    SendHint("Shop - Life Oscillation");
                    SendHint("Shop - Items To Bombs");
                    SendHint("Shop - Powerup Start");
                    SendHint("Shop - Shadow Guard");
                    SendHint("Shop - Payback Ring");
                }

                if (FPSaveManager.TotalStarCards() >= 15)
                {
                    SendHint("Shop - Wood Charm");
                    SendHint("Shop - Earth Charm");
                    SendHint("Shop - Water Charm");
                    SendHint("Shop - Fire Charm");
                    SendHint("Shop - Metal Charm");
                    SendHint("Shop - Rainbow Charm");
                }
            }
            
            if (!isItemShop && (long)Plugin.SlotData["vinyl_shop"] == 1)
            {
                SendHint("Shop - Vinyl - Title Screen");
                SendHint("Shop - Vinyl - Main Menu");
                SendHint("Shop - Vinyl - Basic Tutorial");
                SendHint("Shop - Vinyl - Bonus Stage");
                SendHint("Shop - Vinyl - Speed Gate");
                SendHint("Shop - Vinyl - Shopping");
                SendHint("Shop - Vinyl - Map - Shang Tu");

                if (FPSaveManager.TotalStarCards() >= 1)
                {
                    SendHint("Shop - Vinyl - Stage Clear");
                    SendHint("Shop - Vinyl - Results - Lilac");
                    SendHint("Shop - Vinyl - Results - Carol");
                    SendHint("Shop - Vinyl - Results - Milla");
                    SendHint("Shop - Vinyl - Results - Neera");
                    SendHint("Shop - Vinyl - Singing Water Temple");
                    SendHint("Shop - Vinyl - Cutscene - Generic");
                }

                if (FPSaveManager.TotalStarCards() >= 2)
                {
                    SendHint("Shop - Vinyl - Map - Shang Mu");
                    SendHint("Shop - Vinyl - Map - Shuigang");
                    SendHint("Shop - Vinyl - Royal Palace");
                    SendHint("Shop - Vinyl - Audio Log B");
                }

                if (FPSaveManager.TotalStarCards() >= 11)
                {
                    SendHint("Shop - Vinyl - Boss - Robot A");
                    SendHint("Shop - Vinyl - Boss - Robot B");
                    SendHint("Shop - Vinyl - Boss - Aaa");
                    SendHint("Shop - Vinyl - Boss - Phoenix Highway");
                    SendHint("Shop - Vinyl - Boss - Zao Land");
                    SendHint("Shop - Vinyl - Boss - Arena");
                    SendHint("Shop - Vinyl - Boss - Serpentine A");
                    SendHint("Shop - Vinyl - Boss - Serpentine B");
                    SendHint("Shop - Vinyl - Map - Opera");
                    SendHint("Shop - Vinyl - Battlesphere Commercial");
                    SendHint("Shop - Vinyl - Battlesphere Lobby");
                    SendHint("Shop - Vinyl - Battlesphere Course");
                    SendHint("Shop - Vinyl - Captain Kalaw's Theme");
                    SendHint("Shop - Vinyl - Gallery");
                    SendHint("Shop - Vinyl - Shuigang");
                    SendHint("Shop - Vinyl - Corazon's Theme");
                    SendHint("Shop - Vinyl - Serpentine's Theme");
                    SendHint("Shop - Vinyl - Audio Log A");
                }

                if (FPSaveManager.TotalStarCards() >= 14)
                {
                    SendHint("Shop - Vinyl - Globe Opera 2A");
                    SendHint("Shop - Vinyl - Boss - Beast One/Two");
                    SendHint("Shop - Vinyl - Cutscene - Call to Arms");
                }

                if (FPSaveManager.TotalStarCards() >= 15)
                {
                    SendHint("Shop - Vinyl - Map - Parusa");
                    SendHint("Shop - Vinyl - Cutscene - Big Mood A");
                    SendHint("Shop - Vinyl - Cutscene - Big Mood B");
                    SendHint("Shop - Vinyl - Merga's Theme");
                    SendHint("Shop - Vinyl - Audio Log C");
                }

                if (FPSaveManager.TotalStarCards() >= 16)
                {
                    SendHint("Shop - Vinyl - Map - Floating Island");
                    SendHint("Shop - Vinyl - Map - Volcano");
                    SendHint("Shop - Vinyl - City Hall");
                    SendHint("Shop - Vinyl - Adventure Square");
                }

                if (FPSaveManager.TotalStarCards() >= 17)
                {
                    SendHint("Shop - Vinyl - Paradise Prime");
                }

                if (FPSaveManager.TotalStarCards() >= 22)
                {
                    SendHint("Shop - Vinyl - Boss - Beast Three");
                    SendHint("Shop - Vinyl - Boss - BFF2000");
                }

                if (FPSaveManager.TotalStarCards() >= 23)
                {
                    SendHint("Shop - Vinyl - Boss - Captain Kalaw");
                    SendHint("Shop - Vinyl - Boss - Diamond Point");
                }

                if (FPSaveManager.TotalStarCards() >= 24)
                {
                    SendHint("Shop - Vinyl - Cutscene - Heroic");
                    SendHint("Shop - Vinyl - Cutscene - Preparation");
                    SendHint("Shop - Vinyl - Cutscene - Bakunawa");
                }

                if (FPSaveManager.TotalStarCards() >= 25)
                {
                    SendHint("Shop - Vinyl - Map - Bakunawa");
                }

                if (FPSaveManager.TotalStarCards() >= 28)
                {
                    SendHint("Shop - Vinyl - Boss - Arboretum");
                }

                if (FPSaveManager.TotalStarCards() >= 31)
                {
                    SendHint("Shop - Vinyl - Boss - Merga");
                    SendHint("Shop - Vinyl - Boss - Merga (Pinch)");
                    SendHint("Shop - Vinyl - Lilac's Theme");
                    SendHint("Shop - Vinyl - Carol's Theme");
                    SendHint("Shop - Vinyl - Milla's Theme");
                    SendHint("Shop - Vinyl - Neera's Theme");
                }

                if (FPSaveManager.TotalStarCards() >= 32)
                {
                    SendHint("Shop - Vinyl - Weapon's Core");
                    SendHint("Shop - Vinyl - Boss - Weapon's Core");
                }
            }
        }

        /// <summary>
        /// Sends out a hint for an item if it hasn't already been hinted for.
        /// </summary>
        /// <param name="item">The name of the item to hint for.</param>
        private static void SendHint(string item)
        {
            // Only hint this shop location if we can actually see the item names.
            if ((long)Plugin.SlotData["shop_information"] != 0)
                return;

            // Get the item we're looking for from the locations array.
            Location itemLocation = Array.Find(Plugin.APSave.Locations, location => location.Name == item);

            // Check that this location hasn't already been hinted.
            if (!itemLocation.Hinted)
            {
                // Scout the location.
                Plugin.Session.Locations.ScoutLocationsAsync(locationInfoPacket => { }, Archipelago.MultiClient.Net.Enums.HintCreationPolicy.CreateAndAnnounce, itemLocation.Index);

                // Mark the location as hinted.
                itemLocation.Hinted = true;

                // Force the plugin to save our AP file.
                Plugin.SaveAPFile();
            }
        }

        /// <summary>
        /// Reduces the prices of items in the shop to either 1 Gold Gem or 100 Crystals, depending on the shop type.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuShop), "UpdateItemList", new Type[] { typeof(bool) })]
        static void ReduceAllItemPrices(ref FPPowerup[] ___itemsForSale, ref int[] ___itemCosts)
        {
            // If this is the Vinyl Shop, then loop through each item cost and set them to 100.
            if (___itemsForSale[0] == FPPowerup.NONE && (long)Plugin.SlotData["vinyl_shop"] == 1)
                for (int costIndex = 0; costIndex < ___itemCosts.Length; costIndex++)
                    ___itemCosts[costIndex] = 100;

            // If this is Milla's Shop, then loop through each item cost and set them to 1.
            if (___itemsForSale[1] != FPPowerup.NONE && (long)Plugin.SlotData["milla_shop"] == 1)
                for (int costIndex = 0; costIndex < ___itemCosts.Length; costIndex++)
                    ___itemCosts[costIndex] = 1;
        }

        /// <summary>
        /// Shows the items that the multiworld has placed in the shops.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuShop), "UpdateItemList", new Type[] { typeof(bool) })]
        static void ShowAPItemInShop(ref int ___currentDetail, ref int ___detailListOffset, ref FPHudDigit[] ___powerups, ref FPHudDigit[] ___vinyls,
                                     ref FPPowerup[] ___itemsForSale, ref FPMusicTrack[] ___musicID, ref MenuText[] ___detailName, ref MenuText ___itemDescription)
        {
            // Calculated the selected item.
            int selectedItem = ___currentDetail + ___detailListOffset;

            // Determine if this is Milla's Item Shop or the Vinyl Shop.
            bool isItemShop = true;
            if (___itemsForSale[0] == FPPowerup.NONE)
                isItemShop = false;

            // Set up an array of location checks that are currently visible in the shop.
            Location[] locations = new Location[8];

            // If this is Milla's Shop, then handle the Brave Stones and Potions.
            if (isItemShop)
            {
                if ((long)Plugin.SlotData["milla_shop"] != 1)
                    return;

                // Loop through each item that is currently visible.
                for (int itemIndex = 0; itemIndex < 8; itemIndex++)
                {
                    // Set up the name for this item's location check.
                    string locationName = null;

                    // Determine what the name for this location should be.
                    switch (___itemsForSale[itemIndex + ___detailListOffset])
                    {                                   
                        case FPPowerup.ELEMENT_BURST:   locationName = "Shop - Element Burst";      break;
                        case FPPowerup.MORE_PETALS:     locationName = "Shop - Crystals To Petals"; break;
                        case FPPowerup.PETAL_ARMOR:     locationName = "Shop - Petal Armor";        break;
                        case FPPowerup.EXTRA_STOCK:     locationName = "Shop - Extra Stock";        break;
                        case FPPowerup.STRONG_REVIVALS: locationName = "Shop - Strong Revivals";    break;
                        case FPPowerup.CHEAPER_STOCKS:  locationName = "Shop - Cheaper Stocks";     break;
                        case FPPowerup.REGENERATION:    locationName = "Shop - Healing Strike";     break;
                        case FPPowerup.ATTACK_UP:       locationName = "Shop - Attack Up";          break;
                        case FPPowerup.STRONG_SHIELDS:  locationName = "Shop - Strong Shields";     break;
                        case FPPowerup.SPEED_UP:        locationName = "Shop - Accelerator";        break;
                        case FPPowerup.JUMP_UP:         locationName = "Shop - Super Feather";      break;
                        case FPPowerup.MAX_LIFE_UP:     locationName = "Shop - Max Life Up";        break;
                        case FPPowerup.ONE_HIT_KO:      locationName = "Shop - One Hit KO";         break;
                        case FPPowerup.BIPOLAR_LIFE:    locationName = "Shop - Life Oscillation";   break;
                        case FPPowerup.ITEMS_TO_BOMBS:  locationName = "Shop - Items To Bombs";     break;
                        case FPPowerup.POWERUP_START:   locationName = "Shop - Powerup Start";      break;
                        case FPPowerup.SHADOW_GUARD:    locationName = "Shop - Shadow Guard";       break;
                        case FPPowerup.PAYBACK_RING:    locationName = "Shop - Payback Ring";       break;
                        case FPPowerup.WOOD_CHARM:      locationName = "Shop - Wood Charm";         break;
                        case FPPowerup.EARTH_CHARM:     locationName = "Shop - Earth Charm";        break;
                        case FPPowerup.WATER_CHARM:     locationName = "Shop - Water Charm";        break;
                        case FPPowerup.FIRE_CHARM:      locationName = "Shop - Fire Charm";         break;
                        case FPPowerup.METAL_CHARM:     locationName = "Shop - Metal Charm";        break;
                        case FPPowerup.RAINBOW_CHARM:   locationName = "Shop - Rainbow Charm";      break;

                        // DEBUG: Alert that this FPPowerup type isn't handled.
                        #if DEBUG
                        default: Console.WriteLine($"FPPowerup type {___itemsForSale[itemIndex + ___detailListOffset]} not handled!"); break;
                        #endif
                    }

                    // Find the location with this name.
                    locations[itemIndex] = Array.Find(Plugin.APSave.Locations, location => location.Name == locationName);

                    // If we haven't found a location, stop running this function.
                    if (locations[itemIndex] == null)
                        return;
                }
            }

            // If this is the Vinyl Shop, then handle the Vinyls.
            else
            {
                if ((long)Plugin.SlotData["vinyl_shop"] != 1)
                    return;

                // Loop through each vinyl that is currently visible.
                for (int vinylIndex = 0; vinylIndex < 8; vinylIndex++)
                {
                    // Set up the name for this vinyl's location check.
                    string locationName = null;

                    // Determine what the name for this location should be.
                    switch (___musicID[vinylIndex + ___detailListOffset])
                    {
                        case FPMusicTrack.TITLE: locationName = "Shop - Vinyl - Title Screen"; break;
                        case FPMusicTrack.MENU: locationName = "Shop - Vinyl - Main Menu"; break;
                        case FPMusicTrack.TUTORIAL_BASIC: locationName = "Shop - Vinyl - Basic Tutorial"; break;
                        case FPMusicTrack.STAGE_OPERA2A: locationName = "Shop - Vinyl - Globe Opera 2A"; break;
                        case FPMusicTrack.STAGE_LASTBOSS: locationName = "Shop - Vinyl - Weapon's Core"; break;
                        case FPMusicTrack.BOSS_MAJOR: locationName = "Shop - Vinyl - Boss - Robot A"; break;
                        case FPMusicTrack.BOSS_MINOR: locationName = "Shop - Vinyl - Boss - Robot B"; break;
                        case FPMusicTrack.BOSS_AAA: locationName = "Shop - Vinyl - Boss - Aaa"; break;
                        case FPMusicTrack.BOSS_HIGHWAY: locationName = "Shop - Vinyl - Boss - Phoenix Highway"; break;
                        case FPMusicTrack.BOSS_ZAOLAND: locationName = "Shop - Vinyl - Boss - Zao Land"; break;
                        case FPMusicTrack.BOSS_ARENA: locationName = "Shop - Vinyl - Boss - Arena"; break;
                        case FPMusicTrack.BOSS_KALAW: locationName = "Shop - Vinyl - Boss - Captain Kalaw"; break;
                        case FPMusicTrack.BOSS_ALIEN1: locationName = "Shop - Vinyl - Boss - Serpentine A"; break;
                        case FPMusicTrack.BOSS_ALIEN2: locationName = "Shop - Vinyl - Boss - Serpentine B"; break;
                        case FPMusicTrack.BOSS_OPERA1: locationName = "Shop - Vinyl - Boss - Beast One/Two"; break;
                        case FPMusicTrack.BOSS_OPERA2: locationName = "Shop - Vinyl - Boss - Beast Three"; break;
                        case FPMusicTrack.BOSS_OPERA3: locationName = "Shop - Vinyl - Boss - BFF2000"; break;
                        case FPMusicTrack.BOSS_STARSCAPE: locationName = "Shop - Vinyl - Boss - Diamond Point"; break;
                        case FPMusicTrack.BOSS_CORAZON: locationName = "Shop - Vinyl - Boss - Arboretum"; break;
                        case FPMusicTrack.BOSS_MERGA: locationName = "Shop - Vinyl - Boss - Merga"; break;
                        case FPMusicTrack.BOSS_MERGA_PINCH: locationName = "Shop - Vinyl - Boss - Merga (Pinch)"; break;
                        case FPMusicTrack.BOSS_LAST: locationName = "Shop - Vinyl - Boss - Weapon's Core"; break;
                        case FPMusicTrack.CLEAR: locationName = "Shop - Vinyl - Stage Clear"; break;
                        case FPMusicTrack.CLEAR_LILAC: locationName = "Shop - Vinyl - Results - Lilac"; break;
                        case FPMusicTrack.CLEAR_CAROL: locationName = "Shop - Vinyl - Results - Carol"; break;
                        case FPMusicTrack.CLEAR_MILLA: locationName = "Shop - Vinyl - Results - Milla"; break;
                        case FPMusicTrack.CLEAR_NEERA: locationName = "Shop - Vinyl - Results - Neera"; break;
                        case FPMusicTrack.BONUS: locationName = "Shop - Vinyl - Bonus Stage"; break;
                        case FPMusicTrack.SPEEDGATE: locationName = "Shop - Vinyl - Speed Gate"; break;
                        case FPMusicTrack.SHOP: locationName = "Shop - Vinyl - Shopping"; break;
                        case FPMusicTrack.MAP_VALLEY: locationName = "Shop - Vinyl - Map - Shang Tu"; break;
                        case FPMusicTrack.MAP_SHANGMU: locationName = "Shop - Vinyl - Map - Shang Mu"; break;
                        case FPMusicTrack.MAP_SHUIGANG: locationName = "Shop - Vinyl - Map - Shuigang"; break;
                        case FPMusicTrack.MAP_OPERA: locationName = "Shop - Vinyl - Map - Opera"; break;
                        case FPMusicTrack.MAP_PARUSA: locationName = "Shop - Vinyl - Map - Parusa"; break;
                        case FPMusicTrack.MAP_FLOATING: locationName = "Shop - Vinyl - Map - Floating Island"; break;
                        case FPMusicTrack.MAP_VOLCANO: locationName = "Shop - Vinyl - Map - Volcano"; break;
                        case FPMusicTrack.MAP_BAKU: locationName = "Shop - Vinyl - Map - Bakunawa"; break;
                        case FPMusicTrack.HUB_TEMPLE: locationName = "Shop - Vinyl - Singing Water Temple"; break;
                        case FPMusicTrack.HUB_SHANGTU: locationName = "Shop - Vinyl - Royal Palace"; break;
                        case FPMusicTrack.ARENA_COMMERCIAL: locationName = "Shop - Vinyl - Battlesphere Commercial"; break;
                        case FPMusicTrack.ARENA_LOBBY: locationName = "Shop - Vinyl - Battlesphere Lobby"; break;
                        case FPMusicTrack.ARENA_COURSE: locationName = "Shop - Vinyl - Battlesphere Course"; break;
                        case FPMusicTrack.SCENE_KALAW: locationName = "Shop - Vinyl - Captain Kalaw's Theme"; break;
                        case FPMusicTrack.HUB_GALLERY: locationName = "Shop - Vinyl - Gallery"; break;
                        case FPMusicTrack.HUB_SHUIGANG: locationName = "Shop - Vinyl - Shuigang"; break;
                        case FPMusicTrack.HUB_CITYHALL: locationName = "Shop - Vinyl - City Hall"; break;
                        case FPMusicTrack.HUB_PARUSA: locationName = "Shop - Vinyl - Adventure Square"; break;
                        case FPMusicTrack.HUB_PARADISE: locationName = "Shop - Vinyl - Paradise Prime"; break;
                        case FPMusicTrack.SCENE_GENERIC: locationName = "Shop - Vinyl - Cutscene - Generic"; break;
                        case FPMusicTrack.SCENE_MARCH: locationName = "Shop - Vinyl - Cutscene - Call to Arms"; break;
                        case FPMusicTrack.SCENE_BIGMOOD1: locationName = "Shop - Vinyl - Cutscene - Big Mood A"; break;
                        case FPMusicTrack.SCENE_BIGMOOD2: locationName = "Shop - Vinyl - Cutscene - Big Mood B"; break;
                        case FPMusicTrack.SCENE_HEROIC: locationName = "Shop - Vinyl - Cutscene - Heroic"; break;
                        case FPMusicTrack.SCENE_PREPARATION: locationName = "Shop - Vinyl - Cutscene - Preparation"; break;
                        case FPMusicTrack.SCENE_BAKUNAWA: locationName = "Shop - Vinyl - Cutscene - Bakunawa"; break;
                        case FPMusicTrack.THEME_LILAC: locationName = "Shop - Vinyl - Lilac's Theme"; break;
                        case FPMusicTrack.THEME_CAROL: locationName = "Shop - Vinyl - Carol's Theme"; break;
                        case FPMusicTrack.THEME_MILLA: locationName = "Shop - Vinyl - Milla's Theme"; break;
                        case FPMusicTrack.THEME_NEERA: locationName = "Shop - Vinyl - Neera's Theme"; break;
                        case FPMusicTrack.THEME_CORY: locationName = "Shop - Vinyl - Corazon's Theme"; break;
                        case FPMusicTrack.THEME_SERPENTINE: locationName = "Shop - Vinyl - Serpentine's Theme"; break;
                        case FPMusicTrack.THEME_MERGA: locationName = "Shop - Vinyl - Merga's Theme"; break;
                        case FPMusicTrack.SCENE_AUDIOLOG1: locationName = "Shop - Vinyl - Audio Log A"; break;
                        case FPMusicTrack.SCENE_AUDIOLOG2: locationName = "Shop - Vinyl - Audio Log B"; break;
                        case FPMusicTrack.SCENE_AUDIOLOG3: locationName = "Shop - Vinyl - Audio Log C"; break;
                            
                        // DEBUG: Alert that this FPMusicTrack index isn't handled.
                        #if DEBUG
                        default: Console.WriteLine($"FPMusicTrack index {___musicID[vinylIndex + ___detailListOffset]} not handled!"); break;
                        #endif
                    }

                    // Find the location with this name.
                    locations[vinylIndex] = Array.Find(Plugin.APSave.Locations, location => location.Name == locationName);

                    // If we haven't found a location, stop running this function.
                    if (locations[vinylIndex] == null)
                        return;
                }
            }

            // If this is Milla's Shop, then set the sprites in ___powerups.
            if (isItemShop)
            {
                // Loop through each powerup.
                for (int powerupIndex = 0; powerupIndex < ___powerups.Length; powerupIndex++)
                {
                    // If this location is null, then stop running this function.
                    if (locations[powerupIndex] == null)
                        return;

                    // Check if this power up's icon is not the ? icon.
                    if (___powerups[powerupIndex].digitValue != 1)
                    {
                        // Read and insert a custom sprite at position 37.
                        ___powerups[powerupIndex].digitFrames[37] = Plugin.GetItemSprite(locations[powerupIndex], true);

                        // Set this icon to position 37.
                        ___powerups[powerupIndex].SetDigitValue(37);
                    }
                }
            }

            // If this is the Vinyl Shop, then set the sprites in ___vinyls.
            else
            {
                // Loop through each vinyl.
                for (int vinylIndex = 0; vinylIndex < ___vinyls.Length; vinylIndex++)
                {
                    // If this location is null, then stop running this function.
                    if (locations[vinylIndex] == null)
                        return;

                    // Check if this vinyl's icon is not the ? icon.
                    if (___vinyls[vinylIndex].digitValue != 0)
                    {
                        // Read and insert a custom sprite at position 10.
                        ___vinyls[vinylIndex].digitFrames[10] = Plugin.GetItemSprite(locations[vinylIndex], true);

                        // Set this icon to position 10.
                        ___vinyls[vinylIndex].SetDigitValue(10);
                    }
                }
            }

            // Check if the displayed item name is not the ? ? ? ? ? string.
            if (___detailName[0].GetComponent<TextMesh>().text != "? ? ? ? ?")
            {
                // Get the name of this item.
                ___detailName[0].GetComponent<TextMesh>().text = GetItemName(locations[selectedItem - ___detailListOffset]);

                // Set the description of our item based on if its for ourself or not.
                if (locations[selectedItem - ___detailListOffset].Player != Plugin.Session.Players.GetPlayerName(Plugin.Session.ConnectionInfo.Slot))
                    ___itemDescription.GetComponent<TextMesh>().text = FPStage.WrapText(GetItemDescription(locations[selectedItem - ___detailListOffset], $"An item for {locations[selectedItem - ___detailListOffset].Player}'s {locations[selectedItem - ___detailListOffset].Game}."), 40) + $"\r\nLocation: {locations[selectedItem - ___detailListOffset].Name}";
                else
                    ___itemDescription.GetComponent<TextMesh>().text = FPStage.WrapText(GetItemDescription(locations[selectedItem - ___detailListOffset], "An item for you."), 40) + $"\r\nLocation: {locations[selectedItem - ___detailListOffset].Name}";
            }
        }

        /// <summary>
        /// Gets the name for the item to display in the shop.
        /// </summary>
        /// <param name="location">The location this name is for.
        /// <returns>The name we want.</returns>
        private static string GetItemName(Location location)
        {
            // Determine who this item is for.
            string itemTarget = $"{location.Player}'s ";
            if (location.Player == Plugin.Session.Players.GetPlayerName(Plugin.Session.ConnectionInfo.Slot))
                itemTarget = "";

            // Determine the progression level of this item.
            string itemType = "";
            if (location.Flags == Archipelago.MultiClient.Net.Enums.ItemFlags.Advancement) itemType = "Progression ";
            if (location.Flags == Archipelago.MultiClient.Net.Enums.ItemFlags.Trap) itemType = "Trap ";

            // Return the right string for the Show Item Names in Shops setting.
            switch ((long)Plugin.SlotData["shop_information"])
            {
                default: return $"{itemTarget}{location.Item}";
                case 1: return $"{itemTarget}{itemType}Item";
                case 2: return $"{itemTarget}Item";
                case 3: return "Item";
            }
        }

        /// <summary>
        /// Get a description for the given item.
        /// </summary>
        /// <param name="location">The location this description is for.
        /// <param name="defaultDescription">The description to fall back on if we don't have one in this chunk.</param>
        /// <returns>The description we want.</returns>
        private static string GetItemDescription(Location location, string defaultDescription)
        {
            // Determine who this item is for.
            string itemTarget = location.Player;
            if (location.Player == Plugin.Session.Players.GetPlayerName(Plugin.Session.ConnectionInfo.Slot))
                itemTarget = "you";

            switch ((long)Plugin.SlotData["shop_information"])
            {
                case 1:
                    if (location.Flags == Archipelago.MultiClient.Net.Enums.ItemFlags.Advancement)
                        return $"A progression item for {itemTarget}.";
                    else if (location.Flags == Archipelago.MultiClient.Net.Enums.ItemFlags.Trap)
                        return $"A trap item for {itemTarget}.";
                    else
                        return $"An item for {itemTarget}.";

                case 2:
                    return $"An item for {itemTarget}.";

                case 3:
                    return $"An item for somebody.";
            }

            // Check if this item is for Freedom Planet 2.
            // If so, then either read the game's own descriptions, or read my own.
            if (location.Game == "Manual_FreedomPlanet2_Knuxfan24")
            {
                switch (location.Item)
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
                }
            }

            // If this isn't a Freedom Planet 2 item, then handle getting its description.
            else
            {
                // Check if a descriptions.txt file exists for this game.
                if (File.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\{location.Game}\descriptions.txt"))
                {
                    // Read this description file.
                    string[] descriptions = File.ReadAllLines($@"{Paths.GameRootPath}\mod_overrides\Archipelago\{location.Game}\descriptions.txt");

                    // Loop through each line in this description file. If the line before the first comma perfectly matches the item name, then use everything after said comma for the description.
                    foreach (string line in descriptions)
                        if (line.Remove(line.IndexOf(',')) == location.Item)
                            return line.Substring(line.IndexOf(',') + 1);
                }
            }

            // Return the default description.
            return defaultDescription;
        }
    }
}
