global using HarmonyLib;
global using UnityEngine;
global using System;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using BepInEx;
using FP2Archipelago.Patchers;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace FP2Archipelago
{
    // TODO: See Currently Missing Functionality.txt
    [BepInPlugin("FP2Archipelago", "Freedom Planet 2 Archipelago", "0.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        /// <summary>
        /// Initial code that runs when BepInEx loads our plugin.
        /// </summary>
        private void Awake()
        {
            // Enable the NONE items for the potions and Brave Stones.
            GlobalValues.UnlockedPotions[0] = true;
            GlobalValues.UnlockedBraveStones[0] = true;

            // Set up Harmony.
            Harmony harmony = new("K24_FP2Archipelago");

            // Handle patching the save manager's Star Card count and item sanitisation.
            harmony.PatchAll(typeof(SaveManager));

            // Handle patching the save handling to do redirecting.
            harmony.PatchAll(typeof(SavePatches));

            // Handle skipping the menu stuff.
            harmony.PatchAll(typeof(MainMenu));

            // Handle patching data on the classic map.
            harmony.PatchAll(typeof(ClassicMap));

            // Handle patching data on the player.
            harmony.PatchAll(typeof(Player));
            harmony.PatchAll(typeof(PatchControlls));
            harmony.PatchAll(typeof(PatchControllsRewired));

            // Handle patching data on the results menu.
            harmony.PatchAll(typeof(ResultsMenu));

            // Handle patching data on the item get menu.
            harmony.PatchAll(typeof(ItemGetMenu));

            // Handle patching data on chests.
            harmony.PatchAll(typeof(Chest));

            // Handle patching data on the shop.
            harmony.PatchAll(typeof(ShopMenu));

            // Handle patching the badge message updater.
            harmony.PatchAll(typeof(Badge));

            // Handle patching the item menu so we can equip items the game's save says we don't have.
            harmony.PatchAll(typeof(ItemMenu));

            // Handle patching Merga's boss to remove the ending from it.
            harmony.PatchAll(typeof(MergaBoss));

            // Connect to the Archipelago game.
            // TODO: Unhardcode.
            // TODO: Have a way to do this from in game rather than on start up in case server shenanigans happen.
            //GlobalValues.Session = ArchipelagoSessionFactory.CreateSession("archipelago.gg", 51268);
            GlobalValues.Session = ArchipelagoSessionFactory.CreateSession("localhost", 38281);
            //GlobalValues.Session = ArchipelagoSessionFactory.CreateSession("skincostco.com", 62746);
            GlobalValues.DeathLink = GlobalValues.Session.CreateDeathLinkService();
            GlobalValues.Login = GlobalValues.Session.TryConnectAndLogin("Manual_FreedomPlanet2_Knuxfan24", "knux_fp2", ItemsHandlingFlags.AllItems);
            GlobalValues.DeathLink.EnableDeathLink();

            // Check if the JSON for this seed exists and load it, if not, create it.
            // TODO: Fetch all the locations and store them?
            if (File.Exists($@"{Paths.GameRootPath}\Archipelago Saves\{GlobalValues.Session.RoomState.Seed}.json"))
                GlobalValues.Items = JsonConvert.DeserializeObject<ItemSet>(File.ReadAllText($@"{Paths.GameRootPath}\Archipelago Saves\{GlobalValues.Session.RoomState.Seed}.json"));
            else
                File.WriteAllText($@"{Paths.GameRootPath}\Archipelago Saves\{GlobalValues.Session.RoomState.Seed}.json", JsonConvert.SerializeObject(GlobalValues.Items, Formatting.Indented));

            // Set up the listener for items getting recieved.
            GlobalValues.Session.Items.ItemReceived += (receivedItemsHelper) =>
            {
                // Check if we've already recieved items from loading the save for the first time.
                if (GlobalValues.RecievedFirstLoadItems)
                {
                    // Get the name of the recieved item.
                    string item = receivedItemsHelper.PeekItemName();

                    // Check if we need to do anything to the JSON for the recieved item.
                    switch (item)
                    {
                        case "Gold Gem": GlobalValues.Items.GoldGems++; break;
                        case "Extra Item Slot": GlobalValues.Items.ExtraItemSlots++; break;
                    }

                    // Recieve the item with the specified name.
                    RecieveItem(item);

                    // Remove this item from the queue.
                    receivedItemsHelper.DequeueItem();

                    // Update the JSON and force the game to save.
                    File.WriteAllText($@"{Paths.GameRootPath}\Archipelago Saves\{GlobalValues.Session.RoomState.Seed}.json", JsonConvert.SerializeObject(GlobalValues.Items, Formatting.Indented));
                    FPSaveManager.SaveToFile(FPSaveManager.fileSlot);
                }
            };

            // Read the location names and IDs from the multiworld.
            for (int i = 0; i < GlobalValues.Session.Locations.AllLocations.Count; i++)
                GlobalValues.Locations.Add(GlobalValues.Session.Locations.GetLocationNameFromId(GlobalValues.Session.Locations.AllLocations[i]), GlobalValues.Session.Locations.AllLocations[i]);

            // Set up the listener for DeathLinks getting recieved.
            GlobalValues.DeathLink.OnDeathLinkReceived += (deathLinkObject) =>
            {
                // Save the cause of this DeathLink.
                GlobalValues.LastDLCause = deathLinkObject.Cause;

                // Save who sent this DeathLink.
                GlobalValues.LastDLResponsible = deathLinkObject.Source;

                // Set the player's flag so they can't send a DeathLink alongside the one they recieve.
                Player.canSendDL = false;

                // Find the actual player object.
                FPPlayer player = UnityEngine.Object.FindObjectOfType<FPPlayer>();

                // Check that the player has been found.
                if (player != null)
                {
                    // Remove the player's invincibility, guard and health.
                    player.invincibilityTime = 0;
                    player.guardTime = 0;
                    player.health = 0;

                    // Damage the player.
                    player.Action_Hurt();

                    // Create a badge message to give feedback.
                    BadgeMessage badgeMessage = UnityEngine.Object.Instantiate(FPStage.currentStage.badgeMessage);

                    // Set the badge message ID to 59 (the Pangu hologram one).
                    badgeMessage.id = 59;

                    // Calculate how long the badge message should be shown for.
                    badgeMessage.timer = 0f - FPStage.badgeDisplayOffset;

                    // Calculate and set the local position of the badge message.
                    badgeMessage.transform.localPosition += new Vector3(0f, Mathf.Ceil(FPStage.badgeDisplayOffset / 100f) % 3f * 64f, 0f);

                    // Increment the badge display offset for if multiple badge messages have to be shown at once for whatever reason.
                    FPStage.badgeDisplayOffset += 100f;
                }

                // If we haven't found the player's object, then set the flag for a buffered DeathLink.
                else
                {
                    Player.bufferedDL = true;
                }
            };
        }

        /// <summary>
        /// Recieve an item from the multiworld.
        /// </summary>
        /// <param name="recievedItem">The name of the recieved item.</param>
        /// <param name="fromStart">Whether we're recieving this from loading the file.</param>
        public static void RecieveItem(string recievedItem, bool fromStart = false)
        {
            // Print a message to the console showing what was recieved.
            Console.WriteLine($"Recieved {recievedItem}");

            // Handle each item.
            switch (recievedItem)
            {
                case "Gold Gem": FPSaveManager.totalGoldGems++; break;
                case "Star Card": GlobalValues.StarCards++; break;

                case "Time Capsule":
                    GlobalValues.TimeCapsules++;
                    for (int i = 0; i < GlobalValues.TimeCapsules; i++)
                        if (i < 13)
                            FPSaveManager.timeCapsules[i] = 1;
                    break;

                // Extra Slots
                case "Extra Item Slot": FPSaveManager.itemSlotExpansionLevel = GlobalValues.Items.ExtraItemSlots; break;
                case "Extra Potion Slots": FPSaveManager.potionSlotExpansionLevel = 1; break;

                // Brave Stones
                case "Element Burst": GlobalValues.UnlockedBraveStones[4] = true; break;
                case "Max Life Up": GlobalValues.UnlockedBraveStones[6] = true; break;
                case "Crystals to Petals": GlobalValues.UnlockedBraveStones[7] = true; break;
                case "Powerup Start": GlobalValues.UnlockedBraveStones[8] = true; break;
                case "Shadow Guard": GlobalValues.UnlockedBraveStones[9] = true; break;
                case "Payback Ring": GlobalValues.UnlockedBraveStones[10] = true; break;
                case "Wood Charm": GlobalValues.UnlockedBraveStones[12] = true; break;
                case "Earth Charm": GlobalValues.UnlockedBraveStones[13] = true; break;
                case "Water Charm": GlobalValues.UnlockedBraveStones[14] = true; break;
                case "Fire Charm": GlobalValues.UnlockedBraveStones[15] = true; break;
                case "Metal Charm": GlobalValues.UnlockedBraveStones[16] = true; break;
                case "No Stocks": GlobalValues.UnlockedBraveStones[18] = true; SetTrapBraveStone(fromStart, FPPowerup.STOCK_DRAIN); break;
                case "Expensive Stocks": GlobalValues.UnlockedBraveStones[19] = true; SetTrapBraveStone(fromStart, FPPowerup.EXPENSIVE_STOCKS); break;
                case "Double Damage": GlobalValues.UnlockedBraveStones[20] = true; SetTrapBraveStone(fromStart, FPPowerup.DOUBLE_DAMAGE); break;
                case "No Revivals": GlobalValues.UnlockedBraveStones[21] = true; SetTrapBraveStone(fromStart, FPPowerup.NO_REVIVALS); break;
                case "No Guarding": GlobalValues.UnlockedBraveStones[22] = true; SetTrapBraveStone(fromStart, FPPowerup.NO_GUARDING); break;
                case "No Petals": GlobalValues.UnlockedBraveStones[23] = true; SetTrapBraveStone(fromStart, FPPowerup.NO_PETALS); break;
                case "Time Limit": GlobalValues.UnlockedBraveStones[24] = true; SetTrapBraveStone(fromStart, FPPowerup.TIME_LIMIT); break;
                case "Items To Bombs": GlobalValues.UnlockedBraveStones[26] = true; SetTrapBraveStone(fromStart, FPPowerup.ITEMS_TO_BOMBS); break;
                case "Life Oscillation": GlobalValues.UnlockedBraveStones[27] = true; SetTrapBraveStone(fromStart, FPPowerup.BIPOLAR_LIFE); break;
                case "One Hit KO": GlobalValues.UnlockedBraveStones[28] = true; SetTrapBraveStone(fromStart, FPPowerup.ONE_HIT_KO); break;
                case "Petal Armor": GlobalValues.UnlockedBraveStones[5] = true; break;
                case "Rainbow Charm": GlobalValues.UnlockedBraveStones[17] = true; break;

                // Potions
                case "Potion - Extra Stock": GlobalValues.UnlockedPotions[1] = true; break;
                case "Potion - Strong Revivals": GlobalValues.UnlockedPotions[2] = true; break;
                case "Potion - Cheaper Stocks": GlobalValues.UnlockedPotions[3] = true; break;
                case "Potion - Healing Strike": GlobalValues.UnlockedPotions[4] = true; break;
                case "Potion - Attack Up": GlobalValues.UnlockedPotions[5] = true; break;
                case "Potion - Strong Shields": GlobalValues.UnlockedPotions[6] = true; break;
                case "Potion - Accelerator": GlobalValues.UnlockedPotions[7] = true; break;
                case "Potion - Super Feather": GlobalValues.UnlockedPotions[8] = true; break;

                // Chapter Unlocks
                // TODO: Do anything.

                // Traps
                case "Mirror Trap": if (!fromStart) GlobalValues.IsMirrored = true; break;
                case "Moon Gravity Trap":
                    if (!fromStart)
                    {
                        // Actually find the player.
                        FPPlayer player = UnityEngine.Object.FindObjectOfType<FPPlayer>();

                        // Check that the player exists, if so halve the gravity. If not, set the buffered flag.
                        if (player != null && UnityEngine.Object.FindObjectOfType<FPResultsMenu>() == null)
                            player.gravityStrength /= 2;
                        else
                            Player.bufferedMoonGravity = true;
                    }
                    break;
                case "Double Gravity Trap":
                    if (!fromStart)
                    {
                        // Actually find the player.
                        FPPlayer player = UnityEngine.Object.FindObjectOfType<FPPlayer>();

                        // Check that the player exists, if so double the gravity. If not, set the buffered flag.
                        if (player != null && UnityEngine.Object.FindObjectOfType<FPResultsMenu>() == null)
                            player.gravityStrength *= 2;
                        else
                            Player.bufferedDoubleGravity = true;
                    }
                    break;
                case "Ice Trap":
                    if (!fromStart)
                    {
                        // Actually find the player.
                        FPPlayer player = UnityEngine.Object.FindObjectOfType<FPPlayer>();

                        // Check that the player exists, if so halve the deceleration values. If not, set the buffered flag.
                        if (player != null && UnityEngine.Object.FindObjectOfType<FPResultsMenu>() == null)
                        {
                            player.deceleration /= 4;
                            player.skidDeceleration /= 4;
                        }
                        else
                            Player.bufferedIceTrap = true;
                    }
                    break;
                case "Reverse Trap":
                    if (!fromStart)
                    {
                        // Actually find the player.
                        FPPlayer player = UnityEngine.Object.FindObjectOfType<FPPlayer>();

                        // Check that the player exists, if so then calculate when the reverse trap should expire. If not, just set it to 30.
                        if (player != null && UnityEngine.Object.FindObjectOfType<FPResultsMenu>() == null)
                        {
                            GlobalValues.ReverseTrapExpireTime = FPStage.currentStage.seconds + 30;
                            if (GlobalValues.ReverseTrapExpireTime >= 60)
                                GlobalValues.ReverseTrapExpireTime -= 60;
                        }
                        else
                            GlobalValues.ReverseTrapExpireTime = 30;
                    }
                    break;

                default: Console.WriteLine($"Item not yet handled"); break;
            }

            // If this isn't from loading the file, then create a badge message to act as a notification.
            if (!fromStart)
            {
                // Create a badge message to give feedback.
                BadgeMessage badgeMessage = UnityEngine.Object.Instantiate(FPStage.currentStage.badgeMessage);

                // Set the badge message ID to 19 (the museum restoration one).
                badgeMessage.id = 19;

                // Calculate how long the badge message should be shown for.
                badgeMessage.timer = 0f - FPStage.badgeDisplayOffset;

                // Calculate and set the local position of the badge message.
                badgeMessage.transform.localPosition += new Vector3(0f, Mathf.Ceil(FPStage.badgeDisplayOffset / 100f) % 3f * 64f, 0f);

                // Increment the badge display offset for if multiple badge messages have to be shown at once for whatever reason.
                FPStage.badgeDisplayOffset += 100f;
            }
        }

        /// <summary>
        /// Applies a Barve Stone trap to the player.
        /// </summary>
        /// <param name="fromStart"></param>
        /// <param name="item"></param>
        private static void SetTrapBraveStone(bool fromStart, FPPowerup item)
        {
            // Actually find the player.
            FPPlayer player = UnityEngine.Object.FindObjectOfType<FPPlayer>();

            // Check that the player exists and we haven't recieved this item when loading the file.
            if (player != null && !fromStart)
            {
                // Set up a new array that is 1 element longer than the existing powers array.
                FPPowerup[] powers = new FPPowerup[player.powerups.Length + 1];

                // Loop through each item in the existing array and copy it to our new one.
                for (int i = 0; i < player.powerups.Length; i++)
                    powers[i] = player.powerups[i];

                // Add our trap item to the end of the array.
                powers[powers.Length - 1] = item;

                // Replace the existing array with our new one.
                player.powerups = powers;

                // Apply extra edits for items that need them.
                switch (item)
                {
                    case FPPowerup.STOCK_DRAIN: player.lives = 0; break;
                    case FPPowerup.ONE_HIT_KO: player.health = 0; break;
                }
            }
        }

        /// <summary>
        /// Returns a sprite icon for the given item from the given game.
        /// TODO: Character specific Powerup Start sprites.
        /// </summary>
        /// <param name="game">The game this icon is for.</param>
        /// <param name="item">The item this icon is for.</param>
        /// <returns>The srite we've generated.</returns>
        public static Sprite GetItemSprite(string game, string item)
        {
            // Set the path to the mod overrides folder so we don't keep typing it out.
            string apPath = $@"{Paths.GameRootPath}\mod_overrides\Archipelago";

            // Set up a new texture.
            Texture2D texture = new(32, 32);

            // Load the Archipelago logo.
            texture.LoadImage(File.ReadAllBytes($@"{apPath}\ap_logo.png"));

            // If this icon is not for Freedom Planet 2, then check if one exists for this game and icon combination, if so, load it.
            if (game != "Manual_FreedomPlanet2_Knuxfan24")
            {
                if (File.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\{game}\{item}.png"))
                    texture.LoadImage(File.ReadAllBytes($@"{apPath}\{game}\{item}.png"));
            }

            // If this icon IS for Freedom Planet 2, then look through the sprites and find this item's.
            else
            {
                switch (item)
                {
                    case "Gold Gem": texture.LoadImage(File.ReadAllBytes($@"{apPath}\gold_gem.png")); break;
                    case "Star Card": texture.LoadImage(File.ReadAllBytes($@"{apPath}\star_card.png")); break;
                    case "Time Capsule": texture.LoadImage(File.ReadAllBytes($@"{apPath}\time_capsule.png")); break;
                    case "Potion - Extra Stock": texture.LoadImage(File.ReadAllBytes($@"{apPath}\extra_stocks.png")); break;
                    case "Potion - Strong Revivals": texture.LoadImage(File.ReadAllBytes($@"{apPath}\strong_revives.png")); break;
                    case "Potion - Cheaper Stocks": texture.LoadImage(File.ReadAllBytes($@"{apPath}\cheap_stocks.png")); break;
                    case "Potion - Healing Strike": texture.LoadImage(File.ReadAllBytes($@"{apPath}\healing_strike.png")); break;
                    case "Potion - Attack Up": texture.LoadImage(File.ReadAllBytes($@"{apPath}\attack_up.png")); break;
                    case "Potion - Strong Shields": texture.LoadImage(File.ReadAllBytes($@"{apPath}\strong_shields.png")); break;
                    case "Potion - Accelerator": texture.LoadImage(File.ReadAllBytes($@"{apPath}\accelerator.png")); break;
                    case "Potion - Super Feather": texture.LoadImage(File.ReadAllBytes($@"{apPath}\super_feather.png")); break;
                    case "Element Burst": texture.LoadImage(File.ReadAllBytes($@"{apPath}\element_burst.png")); break;
                    case "Max Life Up": texture.LoadImage(File.ReadAllBytes($@"{apPath}\max_life_up.png")); break;
                    case "Crystals to Petals": texture.LoadImage(File.ReadAllBytes($@"{apPath}\crystals_to_petals.png")); break;
                    case "Powerup Start": texture.LoadImage(File.ReadAllBytes($@"{apPath}\power_start_lilac.png")); break;
                    case "Shadow Guard": texture.LoadImage(File.ReadAllBytes($@"{apPath}\shadow_guard.png")); break;
                    case "Payback Ring": texture.LoadImage(File.ReadAllBytes($@"{apPath}\payback_ring.png")); break;
                    case "Wood Charm": texture.LoadImage(File.ReadAllBytes($@"{apPath}\wood_charm.png")); break;
                    case "Earth Charm": texture.LoadImage(File.ReadAllBytes($@"{apPath}\earth_charm.png")); break;
                    case "Water Charm": texture.LoadImage(File.ReadAllBytes($@"{apPath}\water_charm.png")); break;
                    case "Fire Charm": texture.LoadImage(File.ReadAllBytes($@"{apPath}\fire_charm.png")); break;
                    case "Metal Charm": texture.LoadImage(File.ReadAllBytes($@"{apPath}\metal_charm.png")); break;
                    case "No Stocks": texture.LoadImage(File.ReadAllBytes($@"{apPath}\no_stocks.png")); break;
                    case "Expensive Stocks": texture.LoadImage(File.ReadAllBytes($@"{apPath}\expensive_stocks.png")); break;
                    case "Double Damage": texture.LoadImage(File.ReadAllBytes($@"{apPath}\double_damage.png")); break;
                    case "No Revivals": texture.LoadImage(File.ReadAllBytes($@"{apPath}\no_revives.png")); break;
                    case "No Guarding": texture.LoadImage(File.ReadAllBytes($@"{apPath}\no_guarding.png")); break;
                    case "No Petals": texture.LoadImage(File.ReadAllBytes($@"{apPath}\no_petals.png")); break;
                    case "Time Limit": texture.LoadImage(File.ReadAllBytes($@"{apPath}\time_limit.png")); break;
                    case "Items To Bombs": texture.LoadImage(File.ReadAllBytes($@"{apPath}\items_to_bombs.png")); break;
                    case "Life Oscillation": texture.LoadImage(File.ReadAllBytes($@"{apPath}\life_oscillation.png")); break;
                    case "One Hit KO": texture.LoadImage(File.ReadAllBytes($@"{apPath}\one_hit_ko.png")); break;
                    case "Petal Armor": texture.LoadImage(File.ReadAllBytes($@"{apPath}\petal_armour.png")); break;
                    case "Rainbow Charm": texture.LoadImage(File.ReadAllBytes($@"{apPath}\rainbow_charm.png")); break;
                    default: Console.WriteLine($"Item {item} currently has no sprite definied."); break;
                }
            }

            // Change the texture to use point filtering.
            texture.filterMode = FilterMode.Point;

            // Return a sprite from our texture.
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1);
        }
    }

    /// <summary>
    /// Values that are used by more than one source.
    /// </summary>
    public static class GlobalValues
    {
        /// <summary>
        /// The amount of Star Cards the player has recieved.
        /// </summary>
        public static int StarCards { get; set; } = 0;

        /// <summary>
        /// The amount of Time Capsules the player has recieved.
        /// </summary>
        public static int TimeCapsules { get; set; } = 0;
        
        /// <summary>
        /// The Archipelago session.
        /// </summary>
        public static ArchipelagoSession Session { get; set; }

        /// <summary>
        /// The DeathLink service.
        /// </summary>
        public static DeathLinkService DeathLink { get; set; }

        /// <summary>
        /// Our login result.
        /// </summary>
        public static LoginResult Login { get; set; }

        /// <summary>
        /// Whether or not we've recieved our items from loading the first time.
        /// </summary>
        public static bool RecievedFirstLoadItems { get; set; }

        /// <summary>
        /// The locations that exist for our multiworld.
        /// </summary>
        public static Dictionary<string, long> Locations { get; set; } = new();

        /// <summary>
        /// The locations for Milla's shop.
        /// </summary>
        public static List<LocationContents> MillaShopItems { get; set; }

        /// <summary>
        /// The locations for the Vinyl shop.
        /// </summary>
        public static List<LocationContents> VinylShopItems { get; set; }

        /// <summary>
        /// The Item Set object used to read/write from/to the JSON.
        /// </summary>
        public static ItemSet Items { get; set; } = new();

        /// <summary>
        /// The Brave Stones that have been recieved from the server.
        /// </summary>
        public static bool[] UnlockedBraveStones { get; set; } = new bool[29];

        /// <summary>
        /// The potions that have been recieved from the server.
        /// </summary>
        public static bool[] UnlockedPotions { get; set; } = new bool[9];

        /// <summary>
        /// Whether or not a Mirror Trap is active.
        /// </summary>
        public static bool IsMirrored { get; set; }

        /// <summary>
        /// What was the cause of the last recieved DeathLink.
        /// </summary>
        public static string LastDLCause { get; set; }

        /// <summary>
        /// Who caused the last recieved DeathLink.
        /// </summary>
        public static string LastDLResponsible { get; set; }

        /// <summary>
        /// When a Reverse Trap should expire.
        /// </summary>
        public static int ReverseTrapExpireTime { get; set; } = 60;
    }

    public class LocationContents
    {
        /// <summary>
        /// The player the item at this location belongs to.
        /// </summary>
        public string Player { get; set; } = "";

        /// <summary>
        /// The item at this location.
        /// </summary>
        public string Item { get; set; } = "";

        /// <summary>
        /// The name of this location.
        /// </summary>
        public string Location { get; set; } = "";

        /// <summary>
        /// The game the item at this location belongs to.
        /// </summary>
        public string Game { get; set; } = "";

        /// <summary>
        /// Whether this location has been checked.
        /// </summary>
        public bool Checked { get; set; } = false;
    }

    public class ItemSet
    {
        /// <summary>
        /// The amount of Gold Gems we've recieved from the server.
        /// </summary>
        public int GoldGems { get; set; }

        /// <summary>
        /// The amount of Extra Item Slots we have access to.
        /// </summary>
        public byte ExtraItemSlots { get; set; }
    }
}
