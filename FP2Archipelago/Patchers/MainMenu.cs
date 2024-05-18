using System.Linq;

namespace FP2Archipelago.Patchers
{
    internal class MainMenu
    {
        /// <summary>
        /// Gets the items that should be in Milla's shop.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuMain), "Start")]
        static void GetMillaShopItems()
        {
            // Check if the shop is used (this should be a setting in a proper APWorld) and that we haven't already filled in the list.
            if (GlobalValues.Locations.ContainsKey("Shop - Element Burst") && GlobalValues.MillaShopItems == null)
            {
                // Initialise the list of items.
                GlobalValues.MillaShopItems = new();

                // Loop through all 24 items.
                for (int i = 0; i < 24; i++)
                {
                    // Set up a new location.
                    LocationContents location = new();

                    // Get this location's name.
                    switch (i)
                    {
                        case 0:  location.Location = "Shop - Element Burst"; break;
                        case 1:  location.Location = "Shop - Crystals To Petals"; break;
                        case 2:  location.Location = "Shop - Petal Armor"; break;
                        case 3:  location.Location = "Shop - Extra Stock"; break;
                        case 4:  location.Location = "Shop - Strong Revivals"; break;
                        case 5:  location.Location = "Shop - Cheaper Stocks"; break;
                        case 6:  location.Location = "Shop - Healing Strike"; break;
                        case 7:  location.Location = "Shop - Attack Up"; break;
                        case 8:  location.Location = "Shop - Strong Shields"; break;
                        case 9:  location.Location = "Shop - Accelerator"; break;
                        case 10: location.Location = "Shop - Super Feather"; break;
                        case 11: location.Location = "Shop - Max Life Up"; break;
                        case 12: location.Location = "Shop - One Hit KO"; break;
                        case 13: location.Location = "Shop - Life Oscillation"; break;
                        case 14: location.Location = "Shop - Items To Bombs"; break;
                        case 15: location.Location = "Shop - Powerup Start"; break;
                        case 16: location.Location = "Shop - Shadow Guard"; break;
                        case 17: location.Location = "Shop - Payback Ring"; break;
                        case 18: location.Location = "Shop - Wood Charm"; break;
                        case 19: location.Location = "Shop - Earth Charm"; break;
                        case 20: location.Location = "Shop - Water Charm"; break;
                        case 21: location.Location = "Shop - Fire Charm"; break;
                        case 22: location.Location = "Shop - Metal Charm"; break;
                        case 23: location.Location = "Shop - Rainbow Charm"; break;
                    }

                    // Scout the location.
                    GlobalValues.Session.Locations.ScoutLocationsAsync(locationInfoPacket => { location.Player = GlobalValues.Session.Players.GetPlayerName(locationInfoPacket.Locations[0].Player); location.Item = GlobalValues.Session.Items.GetItemName(locationInfoPacket.Locations[0].Item); }, new long[1] { GlobalValues.Locations[location.Location] });

                    // Twiddle our thumbs waiting for the async operation to finish.
                    while (location.Player == "" || location.Item == "")
                        System.Threading.Thread.Sleep(1);

                    // Get the game of the player this item is for.
                    location.Game = GlobalValues.Session.Players.AllPlayers.FirstOrDefault(i => i.Name == location.Player).Game;

                    // Print things for testing.
                    //Console.WriteLine($"Location {location.Location} holds a(n) {location.Item} for {location.Player} playing {location.Game}");

                    // Add this item to our list.
                    GlobalValues.MillaShopItems.Add(location);
                }
            }
        }

        /// <summary>
        /// Gets the items that should be in the Vinyl shop.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuMain), "Start")]
        static void GetVinylShopItems()
        {
            // Check if the vinyl shop is used (this should be a setting in a proper APWorld) and that we haven't already filled in the list.
            if (GlobalValues.Locations.ContainsKey("Shop - Vinyl - Title Screen") && GlobalValues.VinylShopItems == null)
            {
                // Initialise the list of items.
                GlobalValues.VinylShopItems = new();

                // Loop through all 66 items.
                for (int i = 0; i < 66; i++)
                {
                    // Set up a new location.
                    LocationContents location = new();

                    // Get this location's name.
                    switch (i)
                    {
                        case 0: location.Location = "Shop - Vinyl - Title Screen"; break;
                        case 1: location.Location = "Shop - Vinyl - Main Menu"; break;
                        case 2: location.Location = "Shop - Vinyl - Basic Tutorial"; break;
                        case 3: location.Location = "Shop - Vinyl - Globe Opera 2A"; break;
                        case 4: location.Location = "Shop - Vinyl - Weapon's Core"; break;
                        case 5: location.Location = "Shop - Vinyl - Boss - Robot A"; break;
                        case 6: location.Location = "Shop - Vinyl - Boss - Robot B"; break;
                        case 7: location.Location = "Shop - Vinyl - Boss - Aaa"; break;
                        case 8: location.Location = "Shop - Vinyl - Boss - Phoenix Highway"; break;
                        case 9: location.Location = "Shop - Vinyl - Boss - Zao Land"; break;
                        case 10: location.Location = "Shop - Vinyl - Boss - Arena"; break;
                        case 11: location.Location = "Shop - Vinyl - Boss - Captain Kalaw"; break;
                        case 12: location.Location = "Shop - Vinyl - Boss - Serpentine A"; break;
                        case 13: location.Location = "Shop - Vinyl - Boss - Serpentine B"; break;
                        case 14: location.Location = "Shop - Vinyl - Boss - Beast One/Two"; break;
                        case 15: location.Location = "Shop - Vinyl - Boss - Beast Three"; break;
                        case 16: location.Location = "Shop - Vinyl - Boss - BFF2000"; break;
                        case 17: location.Location = "Shop - Vinyl - Boss - Diamond Point"; break;
                        case 18: location.Location = "Shop - Vinyl - Boss - Arboretum"; break;
                        case 19: location.Location = "Shop - Vinyl - Boss - Merga"; break;
                        case 20: location.Location = "Shop - Vinyl - Boss - Merga (Pinch)"; break;
                        case 21: location.Location = "Shop - Vinyl - Boss - Weapon's Core"; break;
                        case 22: location.Location = "Shop - Vinyl - Stage Clear"; break;
                        case 23: location.Location = "Shop - Vinyl - Results - Lilac"; break;
                        case 24: location.Location = "Shop - Vinyl - Results - Carol"; break;
                        case 25: location.Location = "Shop - Vinyl - Results - Milla"; break;
                        case 26: location.Location = "Shop - Vinyl - Results - Neera"; break;
                        case 27: location.Location = "Shop - Vinyl - Bonus Stage"; break;
                        case 28: location.Location = "Shop - Vinyl - Speed Gate"; break;
                        case 29: location.Location = "Shop - Vinyl - Shopping"; break;
                        case 30: location.Location = "Shop - Vinyl - Map - Shang Tu"; break;
                        case 31: location.Location = "Shop - Vinyl - Map - Shang Mu"; break;
                        case 32: location.Location = "Shop - Vinyl - Map - Shuigang"; break;
                        case 33: location.Location = "Shop - Vinyl - Map - Opera"; break;
                        case 34: location.Location = "Shop - Vinyl - Map - Parusa"; break;
                        case 35: location.Location = "Shop - Vinyl - Map - Floating Island"; break;
                        case 36: location.Location = "Shop - Vinyl - Map - Volcano"; break;
                        case 37: location.Location = "Shop - Vinyl - Map - Bakunawa"; break;
                        case 38: location.Location = "Shop - Vinyl - Singing Water Temple"; break;
                        case 39: location.Location = "Shop - Vinyl - Royal Palace"; break;
                        case 40: location.Location = "Shop - Vinyl - Battlesphere Commercial"; break;
                        case 41: location.Location = "Shop - Vinyl - Battlesphere Lobby"; break;
                        case 42: location.Location = "Shop - Vinyl - Battlesphere Course"; break;
                        case 43: location.Location = "Shop - Vinyl - Captain Kalaw's Theme"; break;
                        case 44: location.Location = "Shop - Vinyl - Gallery"; break;
                        case 45: location.Location = "Shop - Vinyl - Shuigang"; break;
                        case 46: location.Location = "Shop - Vinyl - City Hall"; break;
                        case 47: location.Location = "Shop - Vinyl - Adventure Square"; break;
                        case 48: location.Location = "Shop - Vinyl - Paradise Prime"; break;
                        case 49: location.Location = "Shop - Vinyl - Cutscene - Generic"; break;
                        case 50: location.Location = "Shop - Vinyl - Cutscene - Call to Arms"; break;
                        case 51: location.Location = "Shop - Vinyl - Cutscene - Big Mood A"; break;
                        case 52: location.Location = "Shop - Vinyl - Cutscene - Big Mood B"; break;
                        case 53: location.Location = "Shop - Vinyl - Cutscene - Heroic"; break;
                        case 54: location.Location = "Shop - Vinyl - Cutscene - Preparation"; break;
                        case 55: location.Location = "Shop - Vinyl - Cutscene - Bakunawa"; break;
                        case 56: location.Location = "Shop - Vinyl - Lilac's Theme"; break;
                        case 57: location.Location = "Shop - Vinyl - Carol's Theme"; break;
                        case 58: location.Location = "Shop - Vinyl - Milla's Theme"; break;
                        case 59: location.Location = "Shop - Vinyl - Neera's Theme"; break;
                        case 60: location.Location = "Shop - Vinyl - Corazon's Theme"; break;
                        case 61: location.Location = "Shop - Vinyl - Serpentine's Theme"; break;
                        case 62: location.Location = "Shop - Vinyl - Merga's Theme"; break;
                        case 63: location.Location = "Shop - Vinyl - Audio Log A"; break;
                        case 64: location.Location = "Shop - Vinyl - Audio Log B"; break;
                        case 65: location.Location = "Shop - Vinyl - Audio Log C"; break;
                    }

                    // Scout the location.
                    GlobalValues.Session.Locations.ScoutLocationsAsync(locationInfoPacket => { location.Player = GlobalValues.Session.Players.GetPlayerName(locationInfoPacket.Locations[0].Player); location.Item = GlobalValues.Session.Items.GetItemName(locationInfoPacket.Locations[0].Item); }, new long[1] { GlobalValues.Locations[location.Location] });

                    // Twiddle our thumbs waiting for the async operation to finish.
                    while (location.Player == "" || location.Item == "")
                        System.Threading.Thread.Sleep(1);

                    // Get the game of the player this item is for.
                    location.Game = GlobalValues.Session.Players.AllPlayers.FirstOrDefault(i => i.Name == location.Player).Game;

                    // Print things for testing.
                    //Console.WriteLine($"Location {location.Location} holds a(n) {location.Item} for {location.Player} playing {location.Game}");

                    // Add this item to our list.
                    GlobalValues.VinylShopItems.Add(location);
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuGameMode), "Start")]
        static void SkipGameModeMenu() => StupidGameModeMenuBypassHack(false);

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuTutorialPrompt), "Start")]
        static void SkipTutorialPrompt() => StupidGameModeMenuBypassHack(true);

        /// <summary>
        /// Skips over a menu and goes straight to the Classic Mode menu.
        /// TODO: This feels stupid.
        /// </summary>
        /// <param name="fromTutorialPrompt">Whether or not this has been called from the Tutorial prompt or not.</param>
        static void StupidGameModeMenuBypassHack(bool fromTutorialPrompt)
        {
            // Kill the menu so no input can be given to it?
            if (!fromTutorialPrompt)
                UnityEngine.Object.FindObjectOfType<MenuGameMode>().enabled = false;
            else
                UnityEngine.Object.FindObjectOfType<MenuTutorialPrompt>().enabled = false;

            // Find the menu's screen transition object.
            FPScreenTransition transition = GameObject.Find("Screen Transition").GetComponent<FPScreenTransition>();

            // Set the transition's type to wipe.
            transition.transitionType = FPTransitionTypes.WIPE;

            // Set the speed of the transition.
            transition.transitionSpeed = 48f;

            // Set the transition to load the Classic Mode Menu.
            transition.sceneToLoad = "ClassicMenu";

            // Set the transition to pure black.
            transition.SetTransitionColor(0f, 0f, 0f);

            // Start the transition.
            transition.BeginTransition();

            // Stop the music.
            FPAudio.StopMusic();

            // Play the menu wipe sound.
            FPAudio.PlayMenuSfx(3);

            // Unlock the entire map.
            for (int i = 0; i < FPSaveManager.mapTileReveal.Length; i++)
                FPSaveManager.mapTileReveal[i] = true;
        }

    }
}
