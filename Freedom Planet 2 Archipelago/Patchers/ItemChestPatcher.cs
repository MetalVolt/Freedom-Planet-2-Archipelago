namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class ItemChestPatcher
    {
        public static int ItemIndex = 0;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPStage), "Start")]
        static void ResetItemIndex() => ItemIndex = 0;

        /// <summary>
        /// Stupid hack used to find and steal both an initialised ItemLabel and the items texture atlas from a Dragon Valley chest.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemChest), "Start")]
        static void FetchItemLabel(ref Sprite ___itemSprite, ref ItemLabel ___label)
        {
            // Check that this chest has both a label and item sprite.
            // Also check that we haven't already read them.
            if (___label != null && Plugin.ItemLabelTemplate == null && ___itemSprite != null && Plugin.ItemSpriteAtlas == null)
            {
                // Copy this chest's item sprite texture onto the plugin's atlas.
                Plugin.ItemSpriteAtlas = ___itemSprite.texture;

                // Copy this chest's label onto the plugin one.
                Plugin.ItemLabelTemplate = ___label;

                // Boot the player out to the classic menu.
                SceneManager.LoadScene("ClassicMenu");
            }
        }

        /// <summary>
        /// Sets up the chests.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemChest), "Start")]
        static void SetupChests(ref FPItemChestContent ___contents, ref FPPowerup ___powerupType, ref int ___collectableID)
        {
            // Don't do any of this if we haven't got the label and sprite atlas yet.
            if (Plugin.ItemLabelTemplate == null && Plugin.ItemSpriteAtlas == null)
                return;

            // Set this chest's contents to random.
            ___contents = FPItemChestContent.RANDOM;

            // Set this chest's collectable ID.
            ___collectableID = ItemIndex;

            // Increment our index.
            ItemIndex++;

            // Set this chest's power up to the unused Story Mode item.
            ___powerupType = FPPowerup.STORY_MODE;

            // Get this chest's location.
            Location location = GetLocationAtChest(___collectableID);

            // If this location doesn't exist on the server, then return.
            if (location == null)
            {
                // Report it if we're in debug, just because we failed doesn't mean an error, as it may not exist because of settings.
                #if DEBUG
                Console.WriteLine($"Failed to get the location for Chest {___collectableID} in Stage {FPStage.currentStage.stageID}!");
                #endif

                return;
            }

            // If this location hasn't been checked yet, then set the contents to powerup so we show it when opening the chest.
            if (!location.Checked)
                ___contents = FPItemChestContent.POWERUP;
        }

        /// <summary>
        /// Gets the location at the specified ID for the specified stage.
        /// </summary>
        private static Location GetLocationAtChest(int collectableID)
        {
            switch (FPStage.currentStage.stageID)
            {
                // Dragon Valley.
                case 1:
                    switch (collectableID)
                    {
                        case 0: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Dragon Valley - Extra Chest 1");
                        case 1: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Dragon Valley - Brave Stone");
                        case 2: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Dragon Valley - Vinyl");
                        case 3: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Dragon Valley - Extra Chest 2");
                        case 4: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Dragon Valley - Extra Chest 3");
                        case 5: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Dragon Valley - Extra Chest 4");
                        default: Console.WriteLine($"No location found for chest {collectableID} in Dragon Valley!"); return null;
                    }

                // Shenlin Park.
                case 2:
                    switch (collectableID)
                    {
                        case 0: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Shenlin Park - Vinyl");
                        case 1: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Shenlin Park - Brave Stone");
                        case 2: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Shenlin Park - Extra Chest 1");
                        default: Console.WriteLine($"No location found for chest {collectableID} in Shenlin Park!"); return null;
                    }

                // Avian Museum.
                case 3:
                    switch (collectableID)
                    {
                        case 0: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Avian Museum - Brave Stone");
                        case 1: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Avian Museum - Extra Chest 1");
                        case 2: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Avian Museum - Vinyl");
                        default: Console.WriteLine($"No location found for chest {collectableID} in Avian Museum!"); return null;
                    }

                // Airship Sigwada.
                case 4:
                    switch (collectableID)
                    {
                        case 0: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Airship Sigwada - Extra Chest 1");
                        case 1: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Airship Sigwada - Brave Stone");
                        case 2: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Airship Sigwada - Vinyl");
                        default: Console.WriteLine($"No location found for chest {collectableID} in Airship Sigwada!"); return null;
                    }

                // Tiger Falls.
                case 5:
                    switch (collectableID)
                    {
                        case 0: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Tiger Falls - Extra Chest 1");
                        case 1: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Tiger Falls - Vinyl");
                        case 2: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Tiger Falls - Extra Chest 2");
                        case 3: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Tiger Falls - Extra Chest 3");
                        case 4: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Tiger Falls - Brave Stone");
                        case 5: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Tiger Falls - Extra Chest 4");
                        default: Console.WriteLine($"No location found for chest {collectableID} in Tiger Falls!"); return null;
                    }

                // Robot Graveyard.
                case 6:
                    switch (collectableID)
                    {
                        case 0: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Robot Graveyard - Extra Chest 1");
                        case 1: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Robot Graveyard - Vinyl");
                        default: Console.WriteLine($"No location found for chest {collectableID} in Robot Graveyard!"); return null;
                    }

                // Shade Armory.
                case 7:
                    switch (collectableID)
                    {
                        case 0: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Shade Armory - Vinyl");
                        case 1: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Shade Armory - Extra Chest 1");
                        case 2: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Shade Armory - Extra Chest 2");
                        default: Console.WriteLine($"No location found for chest {collectableID} in Shade Armory!"); return null;
                    }

                // Phoenix Highway.
                case 9:
                    switch (collectableID)
                    {
                        case 0: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Phoenix Highway - Extra Chest 1");
                        case 1: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Phoenix Highway - Brave Stone");
                        case 2: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Phoenix Highway - Extra Chest 2");
                        case 3: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Phoenix Highway - Vinyl");
                        default: Console.WriteLine($"No location found for chest {collectableID} in Phoenix Highway!"); return null;
                    }

                // Zao Land.
                case 10:
                    switch (collectableID)
                    {
                        case 0: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Zao Land - Extra Chest 1");
                        case 1: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Zao Land - Brave Stone");
                        case 2: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Zao Land - Vinyl");
                        case 3: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Zao Land - Extra Chest 2");
                        case 4: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Zao Land - Extra Chest 3");
                        default: Console.WriteLine($"No location found for chest {collectableID} in Zao Land!"); return null;
                    }

                // Globe Opera 1.
                case 11:
                    switch (collectableID)
                    {
                        case 0: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Globe Opera 1 - Extra Chest 1");
                        case 1: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Globe Opera 1 - Vinyl");
                        case 2: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Globe Opera 1 - Extra Chest 2");
                        case 3: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Globe Opera 1 - Extra Chest 3");
                        case 4: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Globe Opera 1 - Extra Chest 4");
                        default: Console.WriteLine($"No location found for chest {collectableID} in Globe Opera 1!"); return null;
                    }

                // Globe Opera 2.
                case 12:
                    switch (collectableID)
                    {
                        case 0: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Globe Opera 2 - Vinyl");
                        case 1: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Globe Opera 2 - Extra Chest 1");
                        case 2: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Globe Opera 2 - Extra Chest 2");
                        default: Console.WriteLine($"No location found for chest {collectableID} in Globe Opera 2!"); return null;
                    }

                // Palace Courtyard.
                case 14:
                    switch (collectableID)
                    {
                        case 0: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Palace Courtyard - Vinyl");
                        default: Console.WriteLine($"No location found for chest {collectableID} in Palace Courtyard!"); return null;
                    }

                // Tidal Gate.
                case 15:
                    switch (collectableID)
                    {
                        case 0: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Tidal Gate - Vinyl");
                        case 1: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Tidal Gate - Extra Chest 1");
                        case 2: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Tidal Gate - Extra Chest 2");
                        case 3: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Tidal Gate - Extra Chest 3");
                        case 4: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Tidal Gate - Extra Chest 4");
                        case 5: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Tidal Gate - Extra Chest 5");
                        case 6: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Tidal Gate - Extra Chest 6");
                        case 7: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Tidal Gate - Extra Chest 7");
                        default: Console.WriteLine($"No location found for chest {collectableID} in Tidal Gate!"); return null;
                    }

                // Zulon Jungle.
                case 16:
                    switch (collectableID)
                    {
                        case 0: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Zulon Jungle - Extra Chest 1");
                        case 1: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Zulon Jungle - Extra Chest 2");
                        case 2: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Zulon Jungle - Vinyl");
                        default: Console.WriteLine($"No location found for chest {collectableID} in Zulon Jungle!"); return null;
                    }

                // Nalao Lake.
                case 17:
                    switch (collectableID)
                    {
                        case 0: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Nalao Lake - Extra Chest 1");
                        case 1: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Nalao Lake - Extra Chest 2");
                        case 2: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Nalao Lake - Vinyl");
                        default: Console.WriteLine($"No location found for chest {collectableID} in Nalao Lake!"); return null;
                    }

                // Sky Bridge.
                case 18:
                    switch (collectableID)
                    {
                        case 0: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Sky Bridge - Extra Chest 1");
                        case 1: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Sky Bridge - Extra Chest 2");
                        case 2: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Sky Bridge - Vinyl");
                        default: Console.WriteLine($"No location found for chest {collectableID} in Sky Bridge!"); return null;
                    }

                // Lightning Tower.
                case 19:
                    switch (collectableID)
                    {
                        case 0: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Sky Bridge - Vinyl");
                        case 1: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Sky Bridge - Extra Chest 1");
                        default: Console.WriteLine($"No location found for chest {collectableID} in Sky Bridge!"); return null;
                    }

                // Ancestral Forge.
                case 20:
                    switch (collectableID)
                    {
                        case 0: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Ancestral Forge - Extra Chest 1");
                        case 1: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Ancestral Forge - Extra Chest 2");
                        case 2: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Ancestral Forge - Extra Chest 3");
                        case 3: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Ancestral Forge - Extra Chest 4");
                        case 4: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Ancestral Forge - Vinyl");
                        case 5: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Ancestral Forge - Extra Chest 5");
                        case 6: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Ancestral Forge - Extra Chest 6");
                        default: Console.WriteLine($"No location found for chest {collectableID} in Ancestral Forge!"); return null;
                    }

                // Magma Starscape.
                case 21:
                    switch (collectableID)
                    {
                        case 0: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Magma Starscape - Vinyl");
                        case 1: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Magma Starscape - Extra Chest 1");
                        case 2: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Magma Starscape - Extra Chest 2");
                        default: Console.WriteLine($"No location found for chest {collectableID} in Magma Starscape!"); return null;
                    }

                // Gravity Bubble.
                case 23:
                    switch (collectableID)
                    {
                        case 0: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Gravity Bubble - Vinyl");
                        case 1: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Gravity Bubble - Extra Chest 1");
                        default: Console.WriteLine($"No location found for chest {collectableID} in Gravity Bubble!"); return null;
                    }

                // Bakunawa Rush.
                case 24:
                    switch (collectableID)
                    {
                        case 0: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Bakunawa Rush - Vinyl");
                        case 1: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Bakunawa Rush - Extra Chest 1");
                        default: Console.WriteLine($"No location found for chest {collectableID} in Bakunawa Rush!"); return null;
                    }

                // Clockwork Arboretum.
                case 26:
                    switch (collectableID)
                    {
                        case 0: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Clockwork Arboretum - Vinyl");
                        default: Console.WriteLine($"No location found for chest {collectableID} in Clockwork Arboretum!"); return null;
                    }

                // Magma Starscape.
                case 27:
                    switch (collectableID)
                    {
                        case 0: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Inversion Dynamo - Extra Chest 1");
                        case 1: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Inversion Dynamo - Extra Chest 2");
                        case 2: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Inversion Dynamo - Vinyl");
                        default: Console.WriteLine($"No location found for chest {collectableID} in Inversion Dynamo!"); return null;
                    }

                // Lunar Cannon.
                case 28:
                    switch (collectableID)
                    {
                        case 0: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Lunar Cannon - Vinyl");
                        default: Console.WriteLine($"No location found for chest {collectableID} in Lunar Cannon!"); return null;
                    }

                default: Console.WriteLine($"Stage chest handler missing for stage {FPStage.currentStage.stageID}!"); return null;
            }
        }

        /// <summary>
        /// Sets the sprite and message for the item displayed when opening the chest.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemChest), "Start")]
        static void SetSpriteAndMessage(ref Sprite ___itemSprite, ref string ___labelMessage, ref int ___collectableID)
        {
            // Read the location at this chest (if there is one).
            Location location = GetLocationAtChest(___collectableID);

            // Check if we've read a location.
            if (location != null)
            {
                // Start the label message.
                ___labelMessage = "Found ";

                if (Plugin.Session.Players.GetPlayerName(Plugin.Session.ConnectionInfo.Slot) == location.Player)
                    ___labelMessage += "your ";
                else
                    ___labelMessage += $"{location.Player}'s ";

                // End the label message.
                ___labelMessage += $"{location.Item}";

                // Set the item's sprite.
                ___itemSprite = Plugin.GetItemSprite(location);
            }
        }

        /// <summary>
        /// Sends a location out from opening a chest.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemChest), "BoxHit")]
        static void SendChestLocationCheck(ref int ___collectableID)
        {
            // Read the location at this chest (if there is one).
            Location location = GetLocationAtChest(___collectableID);

            // Check if we've read a location.
            if (location != null)
            {
                // Send the check at this location's index.
                Plugin.Session.Locations.CompleteLocationChecks(location.Index);

                // Mark this location as checked.
                location.Checked = true;
            }
        }
    }
}
