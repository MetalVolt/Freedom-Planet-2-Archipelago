﻿global using BepInEx;
global using HarmonyLib;
global using System;
global using UnityEngine;
global using UnityEngine.SceneManagement;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using Freedom_Planet_2_Archipelago.Patchers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Freedom_Planet_2_Archipelago
{
    // TODO: RingLink packet sending causes skip hitching, try to fix that.
    // TODO: RingLink packet sending and release collecting can "crash" the game (it keeps running, but the item receiving seems to die). Track this issue down and sort it.
    // TODO: Actually write the last used host, slot and password to the config file.
    // TODO: Maybe change the Atlas references to an array now that I have more than one.
    // TODO: Change some chests to logically require the Super Feather potion?
    public class APSave()
    {
        /// <summary>
        /// The save slot this AP seed should use.
        /// </summary>
        public int FPSaveManagerSlot;

        /// <summary>
        /// The amount of Battlesphere Keys that have been given by the server.
        /// </summary>
        public int BattlesphereKeyCount;

        /// <summary>
        /// The amount of double gravity traps that have been given by the server.
        /// </summary>
        public int DoubleGravityTrapCount;

        /// <summary>
        /// The amount of extra item slots that have been given by the server.
        /// </summary>
        public int ExtraItemSlots;

        /// <summary>
        /// The amount of gold gems that have been given by the server.
        /// </summary>
        public int GoldGemCount;

        /// <summary>
        /// The amount of mirror traps that have been given by the server.
        /// </summary>
        public int MirrorTrapCount;

        /// <summary>
        /// The amount of moon gravity traps that have been given by the server.
        /// </summary>
        public int MoonGravityTrapCount;

        /// <summary>
        /// The amount of powerpoint traps that have been given by the server.
        /// </summary>
        public int PowerpointTrapCount;

        /// <summary>
        /// The amount of star cards that have been given by the server.
        /// </summary>
        public int StarCardCount;

        /// <summary>
        /// The amount of time capsules that have been given by the server.
        /// </summary>
        public int TimeCapsuleCount;

        /// <summary>
        /// The brave stones that have been received from the server.
        /// </summary>
        public bool[] UnlockedBraveStones { get; set; } = new bool[29];

        /// <summary>
        /// The potions that have been received from the server.
        /// </summary>
        public bool[] UnlockedPotions { get; set; } = new bool[9];

        /// <summary>
        /// The locations that exist in FP2 for this multiworld.
        /// </summary>
        public Location[] Locations;

        /// <summary>
        /// The individual chapter unlocks.
        /// </summary>
        public bool[] UnlockedChapters { get; set; } = new bool[8];

        /// <summary>
        /// The chest tracers we've received.
        /// </summary>
        public bool[] ChestTracers { get; set; } = new bool[24];
    }

    public class Location
    {
        /// <summary>
        /// The index for this location on the server.
        /// </summary>
        public long Index;

        /// <summary>
        /// The name of this location.
        /// </summary>
        public string Name = "";

        /// <summary>
        /// The item name at this location.
        /// </summary>
        public string Item = "";

        /// <summary>
        /// The player this location holds an item for.
        /// </summary>
        public string Player = "";

        /// <summary>
        /// The game this location holds an item for.
        /// </summary>
        public string Game = "";

        /// <summary>
        /// The flags for the item at this location.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemFlags Flags = ItemFlags.None;

        /// <summary>
        /// Whether or not this location has already been checked by the player.
        /// </summary>
        public bool Checked = false;

        /// <summary>
        /// Whether or not this location has had a hint sent out for it from the shop.
        /// </summary>
        public bool Hinted = false;
    }

    [BepInPlugin("FP2_AP", "Archipelago", "0.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        // Set up the Archipelago Session and LoginResult data.
        public static ArchipelagoSession Session;
        LoginResult LoginResult;

        // Set up the information for DeathLinks.
        public static DeathLinkService DeathLink;

        // Set up our APSave.
        public static APSave APSave;

        // Set up the timers for the various traps.
        public static int DoubleGravityTrapTimer = -1;
        public static int MirrorTrapTimer = -1;
        public static int MoonGravityTrapTimer = -1;
        public static int PowerpointTrapTimer = -1;

        // Store our slot data.
        public static Dictionary<string, object> SlotData = [];

        // Set up the values for the connection menu.
        public static string serverAddress = "";
        public static string slotName = "";
        public static string password = "";

        // Set up the value for the received item text.
        string NotifyMessage = "";

        // Variables for the Music Randomiser.
        public static System.Random Randomiser = new();
        public static List<AudioClip> Music;
        public static AudioClip[] CustomInvincibility = [];
        public static AudioClip[] CustomClear = [];
        public static AudioClip[] Custom1UP = [];

        // Set up the values for sending a RingLink packet out.
        public static float RingLinkTimer = 0f;
        public static int RingLinkValue = 0;

        // Set up the ItemLabel references.
        public static ItemLabel ItemLabelTemplate = null;
        public static ItemLabel ItemLabelSpawner = null;

        // Set up the sprites for the AP items.
        public static byte[] APLogo;
        static byte[] APLogo_Progression;
        static byte[] APLogo_Trap;
        public static Texture2D ItemSpriteAtlas;
        public static Texture2D OtherSpriteAtlas;

        // Set up our own time counter.
        private float TimeCounter = 0;

        /// <summary>
        /// Initial code that runs when BepInEx loads our plugin.
        /// </summary>
        private void Awake()
        {
            // Create the custom content directories if they doesn't already exist.
            if (!Directory.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Audio\music"))
                Directory.CreateDirectory($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Audio\music");
            if (!Directory.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Audio\jingles\1up"))
                Directory.CreateDirectory($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Audio\jingles\1up");
            if (!Directory.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Audio\jingles\clear"))
                Directory.CreateDirectory($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Audio\jingles\clear");
            if (!Directory.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Audio\jingles\invincibility"))
                Directory.CreateDirectory($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Audio\jingles\invincibility");
            if (!Directory.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sprites"))
                Directory.CreateDirectory($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sprites");
            if (!Directory.Exists($@"{Paths.GameRootPath}\Archipelago Saves"))
                Directory.CreateDirectory($@"{Paths.GameRootPath}\Archipelago Saves");

            // Get the sprites for the AP items.
            APLogo = GetAPSprite("Freedom_Planet_2_Archipelago.resources.ap_logo.png");
            APLogo_Progression = GetAPSprite("Freedom_Planet_2_Archipelago.resources.ap_logo_progression.png");
            APLogo_Trap = GetAPSprite("Freedom_Planet_2_Archipelago.resources.ap_logo_trap.png");

            // Load the Stage Debug Menu to act as a connector menu.
            SceneManager.LoadScene("StageDebugMenu");

            // Set the server values to the ones in the config.
            serverAddress = Config.Bind("Archipelago Server Settings", "Host", "", "The host to connect to.").Value;
            slotName = Config.Bind("Archipelago Server Settings", "Slot", "", "The name of the slot to connect to.").Value;
            password = Config.Bind("Archipelago Server Settings", "Password", "", "The server password (if it has one).").Value;

            // Set up Harmony.
            Harmony harmony = new(PluginInfo.PLUGIN_GUID);

            // Patch the Menu Spawner class, used to remove the Stage Debug Menu.
            harmony.PatchAll(typeof(MenuSpawnerPatcher));

            // Patch the FP Save Manager class, used for redirecting saves to a new folder and assigning our own save slot values.
            harmony.PatchAll(typeof(FPSaveManagerPatcher));

            // Patch the Item Chest class, used for sending items from chests.
            harmony.PatchAll(typeof(ItemChestPatcher));

            // Patch the FP Results Menu class, used for sending items from stage clears.
            harmony.PatchAll(typeof(FPResultsMenuPatcher));

            // Patch the Player Boss Merga class, used to stop the normal ending from playing.
            harmony.PatchAll(typeof(PlayerBossMergaPatcher));

            // Patch the FP Player class, used to handle a few things relating to the player.
            harmony.PatchAll(typeof(FPPlayerPatcher));

            // Patch the Menu Item Get class, used to send items purchased from the shop.
            harmony.PatchAll(typeof(MenuItemGetPatcher));

            // Patch the Menu Shop class, used to display items in the shop.
            harmony.PatchAll(typeof(MenuShopPatcher));

            // Patch the Menu Item Select class, used to reconstruct the item equip menu.
            harmony.PatchAll(typeof(MenuItemSelectPatcher));

            // Patch the Menu Classic class, used to stop map tiles from revealing at the wrong time.
            harmony.PatchAll(typeof(MenuClassicPatcher));

            // Patch the Menu World Map Confirm class, used to stop the player from selecting locked stages (still makes the sound and kills the UI for a frame).
            harmony.PatchAll(typeof(MenuWorldMapConfirmPatcher));

            // Patch the Item Label class, used to extend the Item Label's background.
            harmony.PatchAll(typeof(ItemLabelPatcher));

            // Patch the Menu Arena Challenge Select class, used to show the AP logo as the reward in the menu.
            harmony.PatchAll(typeof(MenuArenaChallengeSelectPatcher));

            // Patch the Player Spawn Point class, used to randomise the character every load if that value is set in our slot data.
            harmony.PatchAll(typeof(PlayerSpawnPointPatcher));

            // Patch the FP Camera class, used to handle the visual side of a Mirror Trap.
            harmony.PatchAll(typeof(FPCameraPatcher));

            // Patch the Menu Global Pause class, used to swap the core counter on the details tab with a Time Capsule counter.
            harmony.PatchAll(typeof(MenuGlobalPausePatcher));

            // Enable the Music Randomiser.
            if (Config.Bind("Music Randomiser", "Enable Music Randomiser", true, "Whether or not to use the music randomiser.").Value)
                harmony.PatchAll(typeof(MusicRandomiser));

            // Set up the silly Easter Egg message.
            harmony.PatchAll(typeof(EasterEggMessage));
        }

        /// <summary>
        /// Code that runs to draw Unity GUI elements.
        /// </summary>
        private void OnGUI()
        {
            // Handle setting the matrix for a resized window.
            float DesignWidth = 640.0f;
            float DesignHeight = 360.0f;
            float resX = (float)(Screen.width) / DesignWidth;
            float resY = (float)(Screen.height) / DesignHeight;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(resX, resY, 1));

            // If we're in release, then hide the textboxes.
            #if !DEBUG
            GUI.color = new(1, 1, 1, 0);
            #endif

            // Check if we're on the Stage Debug Menu scene.
            if (SceneManager.GetActiveScene().name == "StageDebugMenu")
            {
                // Make the cursor visible.
                Cursor.visible = true;

                // Create the hidden login textboxes.
                // TODO: Resizing the window to an arbitary resolution will cause the position of these to desynch with their visuals. Fix this.
                // TODO: Maybe put some sort of length cap on these so they don't stretch off the screen?
                serverAddress = GUI.TextField(new Rect(76, 27, 548, 24), serverAddress);
                slotName = GUI.TextField(new Rect(76, 51, 548, 24), slotName);
                password = GUI.TextField(new Rect(124, 75, 500, 24), password);

                // Update the text on our custom labels to match the contents of the hidden textboxes.
                UpdateHiddenTextbox("hostname", serverAddress);
                UpdateHiddenTextbox("slotname", slotName);
                UpdateHiddenTextbox("passwordname", password);

                // If we're in release, then restore the GUI colour so the connect button is visible.
                #if !DEBUG
                GUI.color = UnityEngine.Color.white;
                #endif

                // Create and check the connect button.
                // TODO: Make this look better somehow.
                if (GUI.Button(new Rect(16, 328, 608, 24), "Connect"))
                {
                    // Print that we're attempting to connect.
                    Console.WriteLine($"Attempting to connect to Archipelago server at {serverAddress} with slot {slotName} and password {password}.");

                    // Create a session and try to login.
                    Session = ArchipelagoSessionFactory.CreateSession(serverAddress);
                    LoginResult = Session.TryConnectAndLogin("Manual_FreedomPlanet2_Knuxfan24", slotName, ItemsHandlingFlags.AllItems, null, null, null, password);

                    // Check if we've successfully logged in.
                    if (LoginResult.Successful)
                    {
                        // Play the badge sound to indicate a successful connection.
                        FPAudio.PlaySfx(18);

                        // Get our slot data.
                        SlotData = Session.DataStorage.GetSlotData();

                        // DEBUG: Print all the key value pairs in the slotdata and their datatypes.
                        #if DEBUG
                        foreach (var key in SlotData)
                            Console.WriteLine($"{key.Key}: {key.Value} (Type: {key.Value.GetType()})");
                        #endif

                        // Reveal all the map tiles.
                        for (int i = 0; i < FPSaveManager.mapTileReveal.Length; i++)
                            FPSaveManager.mapTileReveal[i] = true;

                        // Set up the DeathLink service.
                        DeathLink = Session.CreateDeathLinkService();

                        // Enable DeathLink if its flagged in our slot data.
                        if ((long)SlotData["death_link"] != 0)
                            DeathLink.EnableDeathLink();

                        // Set up the RingLink tag and packet checker.
                        if ((long)SlotData["ring_link"] == 1)
                        {
                            Session.ConnectionInfo.UpdateConnectionOptions([.. Session.ConnectionInfo.Tags, .. new string[1] { "RingLink" }]);
                            Session.Socket.PacketReceived += Socket_RingLinkPacket_Received;
                        }

                        // Set up the listener for items getting received.
                        Session.Items.ItemReceived += Socket_ReceiveItem;

                        // Set up the listener for handling DeathLinks.
                        DeathLink.OnDeathLinkReceived += Socket_ReceiveDeathLink;

                        // Set up the listener for check completions.
                        Session.Locations.CheckedLocationsUpdated += Locations_CheckedLocationsUpdated;

                        // Check if the save file for this seed doesn't exist.
                        if (!File.Exists($@"{Paths.GameRootPath}\Archipelago Saves\{Session.RoomState.Seed}_Save.json"))
                        {
                            // Set up a random number generator.
                            System.Random rng = new();

                            // Roll a random number to use for our slot.
                            int saveSlot = rng.Next();

                            // If this slot number already exists, reroll until it doesn't.
                            while (File.Exists($@"{Paths.GameRootPath}\Archipelago Saves\{saveSlot}.json"))
                                saveSlot = rng.Next();

                            // Create our save, setting the slot and initialising the length of the locations array.
                            APSave = new()
                            {
                                FPSaveManagerSlot = saveSlot,
                                Locations = new Location[Session.Locations.AllLocations.Count]
                            };
                            
                            // If the chest tracer items are disabled, then unlock all of them.
                            if ((long)SlotData["chest_tracer_items"] == 0 && (long)SlotData["chest_tracer_global"] == 0)
                                for (int tracerIndex = 0; tracerIndex < APSave.ChestTracers.Length; tracerIndex++)
                                    APSave.ChestTracers[tracerIndex] = true;

                            // Loop through each location for our game on the server.
                            for (int locationIndex = 0; locationIndex < Session.Locations.AllLocations.Count; locationIndex++)
                            {
                                // Create a new location entry.
                                Location Location = new()
                                {
                                    Index = Session.Locations.AllLocations[locationIndex],
                                    Name = Session.Locations.GetLocationNameFromId(Session.Locations.AllLocations[locationIndex])
                                };

                                // Print that we're getting information for this location.
                                Console.WriteLine($"Getting information for location {Location.Name} with an index of {Location.Index}.");

                                // Scout the location for the player, item, flags and game.
                                Session.Locations.ScoutLocationsAsync(locationInfoPacket => { Location.Player = Session.Players.GetPlayerName(locationInfoPacket[Location.Index].Player); Location.Item = locationInfoPacket[Location.Index].ItemName; Location.Flags = locationInfoPacket[Location.Index].Flags; Location.Game = locationInfoPacket[Location.Index].ItemGame; }, [Session.Locations.AllLocations[locationIndex]]);

                                // Twiddle our thumbs waiting for the async operation to finish.
                                while (Location.Player == "" || Location.Item == "" || Location.Game == "")
                                    System.Threading.Thread.Sleep(1);

                                // DEBUG: Report the data on the item at this location.
                                #if DEBUG
                                Console.WriteLine($"Found {Location.Player}'s {Location.Item} for {Location.Game} with flags {Location.Flags} at {Session.Locations.GetLocationNameFromId(Session.Locations.AllLocations[locationIndex])} (location index {Location.Index})");
                                #endif

                                // Save this location check.
                                APSave.Locations[locationIndex] = Location;
                            }

                            // Write the save to a JSON for future loads.
                            SaveAPFile();
                        }

                        // Check if the save file exists.
                        else
                        {
                            // Read the save from the JSON.
                            APSave = JsonConvert.DeserializeObject<APSave>(File.ReadAllText($@"{Paths.GameRootPath}\Archipelago Saves\{Session.RoomState.Seed}_Save.json"));

                            // Report the length of the locations array in the save.
                            Console.WriteLine($"Loaded {APSave.Locations.Length} location checks.");
                        }

                        // Set the save manager's version to 1.
                        FPSaveManager.versionID = 1f;

                        // Force the game to load from our save.
                        FPSaveManager.LoadFromFile(APSave.FPSaveManagerSlot);

                        // Set the character based on our slot data.
                        switch ((long)SlotData["character"])
                        {
                            case 1: FPSaveManager.character = FPCharacterID.CAROL; break;
                            case 2: FPSaveManager.character = FPCharacterID.MILLA; break;
                            case 3: FPSaveManager.character = FPCharacterID.NEERA; break;
                        }

                        // Get our items from the server.
                        ReceiveStartItems();

                        // Hide the cursor again.
                        Cursor.visible = false;

                        // Find the menu's screen transition object.
                        FPScreenTransition transition = GameObject.Find("Screen Transition").GetComponent<FPScreenTransition>();

                        // Set the transition's type to wipe.
                        transition.transitionType = FPTransitionTypes.WIPE;

                        // Set the speed of the transition.
                        transition.transitionSpeed = 48f;

                        // Set the transition to load Dragon Valley so we can steal the ItemLabel and items texture atlas from a Chest there.
                        if (ItemLabelTemplate == null && ItemSpriteAtlas == null)
                            transition.sceneToLoad = "DragonValley";
                        else
                            transition.sceneToLoad = "ClassicMenu";

                        // Set the transition to pure black.
                        transition.SetTransitionColor(0f, 0f, 0f);

                        // Start the transition.
                        transition.BeginTransition();

                        // Stop the music.
                        FPAudio.StopMusic();

                        // Play the menu wipe sound.
                        FPAudio.PlayMenuSfx(3);
                    }

                    // Report that we failed to connect.
                    else
                    {
                        FPAudio.PlaySfx(FPAudio.SFX_INVALID);
                        Console.WriteLine($"Failed to connect to Archipelago server at {serverAddress} with slot {slotName} and password {password}!");
                    }
                }
            }
        }

        /// <summary>
        /// Gets the items from the server when we initially connect to it.
        /// </summary>
        private void ReceiveStartItems()
        {
            // Set up values to check any items that need multiple copies.
            int serverBattlesphereKeyCount = 0;
            int serverDoubleGravityTrapCount = 0;
            int serverExtraItemSlots = 0;
            int serverGoldGemCount = 0;
            int serverMirrorTrapCount = 0;
            int serverMoonGravityTrapCount = 0;
            int serverPowerpointTrapCount = 0;
            int serverStarCardCount = 0;
            int serverTimeCapsuleCount = 0;
            int serverProgressiveChapterCount = 0;

            // Loop through each item the server has sent and receive it.
            foreach (ItemInfo item in Session.Items.AllItemsReceived)
            {
                // Split the item's name.
                string itemName = item.ItemName;

                // Check the item's name to see if we need to receive a certain amount of them to avoid duplication.
                switch (itemName)
                {
                    case "Battlesphere Key": serverBattlesphereKeyCount++; break;
                    case "Double Gravity Trap": serverDoubleGravityTrapCount++; break;
                    case "Extra Item Slot": serverExtraItemSlots++; break;
                    case "Gold Gem": serverGoldGemCount++; break;
                    case "Mirror Trap": serverMirrorTrapCount++; break;
                    case "Moon Gravity Trap": serverMoonGravityTrapCount++; break;
                    case "Powerpoint Trap": serverPowerpointTrapCount++; break;
                    case "Progressive Chapter": serverProgressiveChapterCount++; break;
                    case "Star Card": serverStarCardCount++; break;
                    case "Time Capsule": serverTimeCapsuleCount++; break;

                    default: ReceiveItem(itemName); break;
                }

                // Remove this item from the queue.
                Session.Items.DequeueItem();
            }

            // Calculate how many progressive chapters we need.
            int progressiveChapterCount = 0;
            foreach (bool chapter in APSave.UnlockedChapters)
                if (chapter)
                    progressiveChapterCount++;

            // Send the multitude items.
            CalculateItems(serverBattlesphereKeyCount, APSave.BattlesphereKeyCount, "Battlesphere Key");
            CalculateItems(serverDoubleGravityTrapCount, APSave.DoubleGravityTrapCount, "Double Gravity Trap");
            CalculateItems(serverExtraItemSlots, APSave.ExtraItemSlots, "Extra Item Slot");
            CalculateItems(serverGoldGemCount, APSave.GoldGemCount, "Gold Gem");
            CalculateItems(serverMirrorTrapCount, APSave.MirrorTrapCount, "Mirror Trap");
            CalculateItems(serverMoonGravityTrapCount, APSave.MoonGravityTrapCount, "Moon Gravity Trap");
            CalculateItems(serverPowerpointTrapCount, APSave.PowerpointTrapCount, "Powerpoint Trap");
            CalculateItems(serverProgressiveChapterCount, progressiveChapterCount, "Progressive Chapter");
            CalculateItems(serverStarCardCount, APSave.StarCardCount, "Star Card");
            CalculateItems(serverTimeCapsuleCount, APSave.TimeCapsuleCount, "Time Capsule");

            // Local function that actually sends the missing multitude items.
            void CalculateItems(int serverValue, int saveValue, string itemName)
            {
                // If this is a debug build, report how many of this item we should receive, and what the initial and final values should be.
                #if DEBUG
                Console.WriteLine($"Server should send {serverValue - saveValue} {itemName}(s), giving a total of {serverValue}, up from {saveValue}.");
                #endif

                // Calculate how many of this item we need to get based on the gap between server and save and receive that many.
                for (int i = 0; i < serverValue - saveValue; i++)
                    ReceiveItem(itemName);
            }
        }

        /// <summary>
        /// Spawn a label if we do a check that isn't for ourself.
        /// </summary>
        private void Locations_CheckedLocationsUpdated(System.Collections.ObjectModel.ReadOnlyCollection<long> newCheckedLocations)
        {
            // Loop through each location in our check list.
            for (int i = 0; i < newCheckedLocations.Count; i++)
            {
                // Find this location in our array.
                Location location = Array.Find(APSave.Locations, location => location.Index == newCheckedLocations[i]);

                // Check that this location actually exists.
                if (location != null)
                {
                    // Check if this location is for another player.
                    if (Session.Players.GetPlayerName(Session.ConnectionInfo.Slot) != location.Player)
                    {
                        // Set the notify message to show the player name and item.
                        NotifyMessage = $"Found {location.Player}'s {location.Item}";

                        // Play the collection sound.
                        FPAudio.PlayCollectibleSfx(FPAudio.SFX_ITEMGET);

                        // Spawn the item label.
                        SpawnItemLabel();
                    }
                }
            }
        }

        /// <summary>
        /// Receives a DeathLink from the multiworld.
        /// </summary>
        /// <param name="deathLinkHelper">The helper handling the DeathLink receive.</param>
        private void Socket_ReceiveDeathLink(DeathLink deathLinkHelper)
        {
            // Present the cause and source of the DeathLink.
            if (deathLinkHelper.Cause != null)
                NotifyMessage = $"{deathLinkHelper.Cause}";
            else
                NotifyMessage = $"DeathLink received from {deathLinkHelper.Source}";

            // Stop the player from being able to send a DeathLink out.
            FPPlayerPatcher.canSendDeathLink = false;

            // Set the flag to tell the player patcher that a DeathLink is awaiting.
            FPPlayerPatcher.hasBufferedDeathLink = true;

            // Spawn the label to show the player.
            SpawnItemLabel();
        }

        /// <summary>
        /// Receives an item from the multiworld.
        /// </summary>
        /// <param name="receivedItemsHelper">The helper handling the item receive.</param>
        private void Socket_ReceiveItem(ReceivedItemsHelper receivedItemsHelper)
        {
            // DEBUG: Print that this helper was fired.
            #if DEBUG
            Console.WriteLine($"Item received helper fired for {receivedItemsHelper.PeekItem().ItemName} from {receivedItemsHelper.PeekItem().Player.Name}.");
            #endif

            // Set the notify message depending on if we received this item from ourselves.
            if (receivedItemsHelper.PeekItem().Player.Name != Session.Players.GetPlayerName(Session.ConnectionInfo.Slot))
                NotifyMessage = $"Received {receivedItemsHelper.PeekItem().ItemName} from {receivedItemsHelper.PeekItem().Player.Name}";
            else
                NotifyMessage = $"Found your {receivedItemsHelper.PeekItem().ItemName}";

            // Set up a value to check if we should show the ItemLabel.
            bool shouldShowLabel = true;

            // Check if this item came from ourself. If so, check the name of the location and flip the label flag to false if it's not from a stage clear.
            if (receivedItemsHelper.PeekItem().Player.Name == Session.Players.GetPlayerName(Session.ConnectionInfo.Slot))
                if (!receivedItemsHelper.PeekItem().LocationName.EndsWith(" - Clear") && !receivedItemsHelper.PeekItem().LocationName.StartsWith("The Battlesphere - Challenge"))
                    shouldShowLabel = false;

            // Handle actually receiving the item.
            ReceiveItem(receivedItemsHelper.PeekItem().ItemName);
            receivedItemsHelper.DequeueItem();

            // Show the item label if we should.
            if (shouldShowLabel)
            {
                // Play the collection sound.
                FPAudio.PlayCollectibleSfx(FPAudio.SFX_ITEMGET);

                // Spawn the label to show the player.
                SpawnItemLabel();
            }
        }

        /// <summary>
        /// Process a packet. This is only used for the RingLink integration.
        /// </summary>
        /// <param name="packet">The packet being received from the Multiworld.</param>
        private void Socket_RingLinkPacket_Received(ArchipelagoPacketBase packet)
        {
            switch (packet)
            {
                case BouncedPacket bouncedPacket when bouncedPacket.Tags.Contains("RingLink"):
                    // Ignore the packet if we're the one who sent it.
                    if (bouncedPacket.Data["source"].ToObject<int>() == Session.ConnectionInfo.Slot)
                        return;

                    // Get the value from the RingLink.
                    int ringLinkValue = bouncedPacket.Data["amount"].ToObject<int>();

                    // Look for the player object.
                    FPPlayer player = UnityEngine.Object.FindObjectOfType<FPPlayer>();

                    // Edit the player's crystal counts if they exist.
                    if (player != null)
                    {
                        player.totalCrystals += ringLinkValue;
                        if (player.totalCrystals < 0)
                            player.totalCrystals = 0;

                        player.crystals -= ringLinkValue;
                        if (player.crystals > player.extraLifeCost)
                            player.crystals = player.extraLifeCost;

                        // Give a 1UP if the player has enough crystals (copy and pasted from the original source).
                        if (player.crystals < 1)
                        {
                            player.crystals = player.extraLifeCost;

                            if (player.lives < 9)
                                player.lives++;

                            CrystalBonus crystalBonus = (CrystalBonus)FPStage.CreateStageObject(CrystalBonus.classID, 292f, -64f);
                            crystalBonus.animator.Play("HUD_Add");
                            crystalBonus.duration = 40f;

                            InvincibilityStar invincibilityStar = (InvincibilityStar)FPStage.CreateStageObject(InvincibilityStar.classID, -100f, -100f);
                            invincibilityStar.parentObject = player;
                            invincibilityStar.distance = 320f;
                            invincibilityStar.descend = true;
                            InvincibilityStar invincibilityStar2 = (InvincibilityStar)FPStage.CreateStageObject(InvincibilityStar.classID, -100f, -100f);
                            invincibilityStar2.parentObject = player;
                            invincibilityStar2.rotation = 180f;
                            invincibilityStar2.distance = 320f;
                            invincibilityStar2.descend = true;

                            FPAudio.PlayJingle(3);
                        }

                        // Check if this RingLink value is a negative number.
                        if (ringLinkValue < 0)
                        {
                            // Check if the player has a shield.
                            if (player.shieldHealth > 0)
                            {
                                // Play the approriate sound effect for the shield.
                                if (player.shieldHealth > 1)
                                    FPAudio.PlaySfx(player.sfxShieldHit);
                                else
                                    FPAudio.PlaySfx(player.sfxShieldBreak);

                                // Reduce the player's shield health.
                                player.shieldHealth -= 1;

                                // Create the shield hit flash.
                                ShieldHit shieldHit2 = (ShieldHit)FPStage.CreateStageObject(ShieldHit.classID, player.position.x, player.position.y);
                                shieldHit2.SetParentObject(player);
                                shieldHit2.remainingDuration = 60f - Mathf.Min((float)player.shieldHealth * 3f, 30f);
                            }

                            // Check if the player has health to lose (a RingLink should never kill).
                            else if (player.health > 0)
                            {
                                // Play the damage sound effect.
                                FPAudio.PlaySfx(player.sfxHurt);

                                // Either remove a health petal, or floor the health down to 0.
                                if (player.health > 1f)
                                    player.health -= 1f;
                                else
                                    player.health = 0;
                            }
                        }
                    }

                    // If the player doesn't exist (because a stage isn't active for instance), then just apply the RingLink straight to the save.
                    else
                    {
                        FPSaveManager.totalCrystals += ringLinkValue;
                        if (FPSaveManager.totalCrystals < 0)
                            FPSaveManager.totalCrystals = 0;
                    }

                    break;
            }
        }

        /// <summary>
        /// Creates the ItemLabel to tell the user than an item/DeathLink has come in.
        /// TODO: This appears behind some stuff (like the shop), can I fix that?
        /// </summary>
        private void SpawnItemLabel()
        {
            // Check that our label template actually exists before trying to instantiate it.
            if (ItemLabelTemplate == null)
                return;

            // If the ItemLabelSpawner already exists, destroy it.
            ItemLabelSpawner?.RequestDestroy();

            // Instantiate the ItemLabelSpawner by copying our template to it.
            ItemLabelSpawner = UnityEngine.Object.Instantiate(ItemLabelTemplate);

            // Set the message on the ItemLabel to our NotifyMessage.
            ItemLabelSpawner.itemName = NotifyMessage;

            // Clear NotifyMessage.
            NotifyMessage = string.Empty;

            // Set up the listener for the ItemLabel expiring.
            ItemLabelSpawner.ItemLabelExpired += ItemLabelExpired;
        }

        /// <summary>
        /// Listener for our ItemLabel expiring.
        /// </summary>
        private void ItemLabelExpired(ItemLabel itemLabel)
        {
            ItemLabelSpawner.ItemLabelExpired -= ItemLabelExpired;
            if (ItemLabelTemplate == ItemLabelSpawner)
                ItemLabelSpawner = null;
        }

        /// <summary>
        /// Code that runs every frame.
        /// </summary>
        private void Update()
        {
            // Increment our time counter by the game's delta time.
            TimeCounter += Time.deltaTime;

            // Check if time counter has reached one.
            while (TimeCounter >= 1f)
            {
                // Subtract 1 from it.
                TimeCounter -= 1f;

                // If a player exists, then decrement the trap timers.
                if (UnityEngine.Object.FindObjectOfType<FPPlayer>() != null)
                {
                    if (DoubleGravityTrapTimer > 0) DoubleGravityTrapTimer--;
                    if (MirrorTrapTimer > 0) MirrorTrapTimer--;
                    if (MoonGravityTrapTimer > 0) MoonGravityTrapTimer--;

                    // If this is a debug build, then print the remaining time on the trap timers.
                    #if DEBUG
                    if (DoubleGravityTrapTimer > 0) Console.WriteLine($"Double Gravity Trap Timer is now: {DoubleGravityTrapTimer}");
                    if (MirrorTrapTimer > 0) Console.WriteLine($"Mirror Trap Timer is now: {MirrorTrapTimer}");
                    if (MoonGravityTrapTimer > 0) Console.WriteLine($"Moon Gravity Trap Timer is now: {MoonGravityTrapTimer}");
                    if (PowerpointTrapTimer > 0) Console.WriteLine($"Powerpoint Trap Timer is now: {PowerpointTrapTimer}");
                    #endif
                }

                if (PowerpointTrapTimer > 0)
                {
                    PowerpointTrapTimer--;

                    #if DEBUG
                    Console.WriteLine($"Powerpoint Trap Timer is now: {PowerpointTrapTimer}");
                    #endif

                    if (Application.targetFrameRate > 15)
                        Application.targetFrameRate = 15;
                }
                else
                    FPSaveManager.SetTargetFPS();
            }

            // If the active scene is the Battlesphere Time Capsule cutscene or initial challenge list, then boot the player out to the arena menu instead.
            if (SceneManager.GetActiveScene().name == "Cutscene_BattlesphereCapsule" || SceneManager.GetActiveScene().name == "ArenaChallengeMenu")
                SceneManager.LoadScene("ArenaMenu");

            // Check if we haven't yet read the second sprite atlas and that we're on the classic menu.
            if (OtherSpriteAtlas == null && SceneManager.GetActiveScene().name == "ClassicMenu")
            {
                // Find the menu GameObject.
                GameObject menu = UnityEngine.GameObject.Find("ClassicMenu");

                // Check that the menu actually exists and grab the atlas from the Dragon Valley background if it does.
                if (menu != null)
                    OtherSpriteAtlas = menu.transform.GetChild(13).transform.GetChild(6).GetComponent<SpriteRenderer>().sprite.texture;
            }

            // Check if the RingLink timer is going and decrement it if so.
            if (RingLinkTimer > 0f)
                RingLinkTimer -= FPStage.deltaTime;

            // Check if the RingLink timer has gone below 0.
            else if (RingLinkTimer < 0f)
            {
                // Set the timer to 0 so this check doesn't refire.
                RingLinkTimer = 0f;

                // Calculate our epoch time.
                TimeSpan epochTimeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1);

                // Create a RingLink packet.
                BouncePacket RingLinkPacket = new()
                {
                    Tags = ["RingLink"],
                    Data = new Dictionary<string, Newtonsoft.Json.Linq.JToken>
                    {
                        { "source", Session.ConnectionInfo.Slot },
                        { "time", (long)epochTimeSpan.TotalSeconds },
                        { "amount", RingLinkValue }
                    }
                };

                // Send the packet to the server.
                Session.Socket.SendPacketAsync(RingLinkPacket);

                // Reset the RingLink value.
                RingLinkValue = 0;
            }
        }
    
        /// <summary>
        /// Handles receiving an item from the multiworld.
        /// </summary>
        /// <param name="ReceivedItem">The name of the item we're receiving.</param>
        private void ReceiveItem(string ReceivedItem)
        {
            // Check the item we're receiving.
            switch (ReceivedItem)
            {
                case "Battlesphere Key": if (APSave.BattlesphereKeyCount < 18) APSave.BattlesphereKeyCount++; break;

                case "Crystals to Petals": APSave.UnlockedBraveStones[7] = true; break;

                case "Double Damage":
                    APSave.UnlockedBraveStones[20] = true;
                    SetTrapBraveStone(FPPowerup.DOUBLE_DAMAGE);
                    break;

                // Set the Double Gravity Trap Timer to 30 seconds and increment our save's double gravity trap count.
                case "Double Gravity Trap":
                    DoubleGravityTrapTimer = 30;
                    APSave.DoubleGravityTrapCount++;
                    break;

                case "Earth Charm": APSave.UnlockedBraveStones[13] = true; break;

                case "Element Burst": APSave.UnlockedBraveStones[4] = true; break;

                case "Expensive Stocks": 
                    APSave.UnlockedBraveStones[19] = true;
                    SetTrapBraveStone(FPPowerup.EXPENSIVE_STOCKS);
                    break;

                // Increment the save manager's item slot expansion level and our save's extra item slots.
                case "Extra Item Slot":
                    FPSaveManager.itemSlotExpansionLevel++;
                    APSave.ExtraItemSlots++;
                    break;

                // Set the save manager's potion slot expansion level to 1 (as only one instance of it actually works).
                case "Extra Potion Slots": FPSaveManager.potionSlotExpansionLevel = 1; break;

                case "Fire Charm": APSave.UnlockedBraveStones[15] = true; break;

                // Handle the Gold Gem differently depending on the slot data
                case "Gold Gem":
                    GoldGem();
                    APSave.GoldGemCount++;
                    break;

                case "Items To Bombs":
                    APSave.UnlockedBraveStones[26] = true;
                    SetTrapBraveStone(FPPowerup.ITEMS_TO_BOMBS);
                    break;

                case "Life Oscillation":
                    APSave.UnlockedBraveStones[27] = true;
                    SetTrapBraveStone(FPPowerup.BIPOLAR_LIFE);
                    break;

                case "Max Life Up": APSave.UnlockedBraveStones[6] = true; break;

                case "Metal Charm": APSave.UnlockedBraveStones[16] = true; break;

                // Set the Mirror Trap Timer to a minute and increment our save's mirror trap count.
                case "Mirror Trap":
                    MirrorTrapTimer = 60;
                    APSave.MirrorTrapCount++;
                    break;

                // Set the Moon Gravity Trap Timer to 30 seconds and increment our save's mirror trap count.
                case "Moon Gravity Trap":
                    MoonGravityTrapTimer = 30;
                    APSave.MoonGravityTrapCount++;
                    break;

                case "No Guarding":
                    APSave.UnlockedBraveStones[22] = true;
                    SetTrapBraveStone(FPPowerup.NO_GUARDING);
                    break;

                case "No Petals":
                    APSave.UnlockedBraveStones[23] = true;
                    SetTrapBraveStone(FPPowerup.NO_PETALS);
                    break;

                case "No Revivals":
                    APSave.UnlockedBraveStones[21] = true;
                    SetTrapBraveStone(FPPowerup.NO_REVIVALS);
                    break;

                case "No Stocks": APSave.UnlockedBraveStones[18] = true;
                    SetTrapBraveStone(FPPowerup.STOCK_DRAIN);
                    break;

                case "One Hit KO":
                    APSave.UnlockedBraveStones[28] = true;
                    SetTrapBraveStone(FPPowerup.ONE_HIT_KO);
                    break;

                case "Payback Ring": APSave.UnlockedBraveStones[10] = true; break;

                case "Petal Armor": APSave.UnlockedBraveStones[5] = true; break;

                case "Potion - Accelerator": APSave.UnlockedPotions[7] = true; break;

                case "Potion - Attack Up": APSave.UnlockedPotions[5] = true; break;

                case "Potion - Cheaper Stocks": APSave.UnlockedPotions[3] = true; break;

                case "Potion - Extra Stock": APSave.UnlockedPotions[1] = true; break;

                case "Potion - Healing Strike": APSave.UnlockedPotions[4] = true; break;

                case "Potion - Strong Revivals": APSave.UnlockedPotions[2] = true; break;

                case "Potion - Strong Shields": APSave.UnlockedPotions[6] = true; break;

                case "Potion - Super Feather": APSave.UnlockedPotions[8] = true; break;

                // Set the Powerpoint Trap Timer to 30 seconds and increment our save's powerpoint trap count.
                case "Powerpoint Trap":
                    PowerpointTrapTimer = 30;
                    APSave.PowerpointTrapCount++;
                    break;

                // Unlock certain levels on the map.
                case "Progressive Chapter":
                    for (int chapterIndex = 0; chapterIndex < APSave.UnlockedChapters.Length; chapterIndex++)
                    {
                        if (!APSave.UnlockedChapters[chapterIndex])
                        {
                            APSave.UnlockedChapters[chapterIndex] = true;
                            break;
                        }
                    }
                    break;

                case "Powerup Start": APSave.UnlockedBraveStones[8] = true; break;

                case "Rainbow Charm": APSave.UnlockedBraveStones[17] = true; break;

                case "Shadow Guard": APSave.UnlockedBraveStones[9] = true; break;

                // Increment our save's star card count.
                case "Star Card": APSave.StarCardCount++; break;

                // Increment our save's time capsule count.
                case "Time Capsule": APSave.TimeCapsuleCount++; break;

                case "Time Limit":
                    APSave.UnlockedBraveStones[24] = true;
                    SetTrapBraveStone(FPPowerup.TIME_LIMIT);
                    break;

                case "Water Charm": APSave.UnlockedBraveStones[14] = true; break;

                case "Wood Charm": APSave.UnlockedBraveStones[12] = true; break;

                case "Mystery of the Frozen North": APSave.UnlockedChapters[0] = true; break;
                case "Sky Pirate Panic": APSave.UnlockedChapters[1] = true; break;
                case "Enter the Battlesphere": APSave.UnlockedChapters[2] = true; break;
                case "Globe Opera": APSave.UnlockedChapters[3] = true; break;
                case "Justice in the Sky Paradise": APSave.UnlockedChapters[4] = true; break;
                case "Robot Wars! Snake VS Tarsier": APSave.UnlockedChapters[5] = true; break;
                case "Echoes of the Dragon War": APSave.UnlockedChapters[6] = true; break;
                case "Bakunawa": APSave.UnlockedChapters[7] = true; break;

                case "Chest Tracer - Dragon Valley": APSave.ChestTracers[0] = true; break;
                case "Chest Tracer - Shenlin Park": APSave.ChestTracers[1] = true; break;
                case "Chest Tracer - Avian Museum": APSave.ChestTracers[2] = true; break;
                case "Chest Tracer - Airship Sigwada": APSave.ChestTracers[3] = true; break;
                case "Chest Tracer - Tiger Falls": APSave.ChestTracers[4] = true; break;
                case "Chest Tracer - Robot Graveyard": APSave.ChestTracers[5] = true; break;
                case "Chest Tracer - Shade Armory": APSave.ChestTracers[6] = true; break;
                case "Chest Tracer - Phoenix Highway": APSave.ChestTracers[7] = true; break;
                case "Chest Tracer - Zao Land": APSave.ChestTracers[8] = true; break;
                case "Chest Tracer - Globe Opera 1": APSave.ChestTracers[9] = true; break;
                case "Chest Tracer - Globe Opera 2": APSave.ChestTracers[10] = true; break;
                case "Chest Tracer - Palace Courtyard": APSave.ChestTracers[11] = true; break;
                case "Chest Tracer - Tidal Gate": APSave.ChestTracers[12] = true; break;
                case "Chest Tracer - Zulon Jungle": APSave.ChestTracers[13] = true; break;
                case "Chest Tracer - Nalao Lake": APSave.ChestTracers[14] = true; break;
                case "Chest Tracer - Sky Bridge": APSave.ChestTracers[15] = true; break;
                case "Chest Tracer - Lightning Tower": APSave.ChestTracers[16] = true; break;
                case "Chest Tracer - Ancestral Forge": APSave.ChestTracers[17] = true; break;
                case "Chest Tracer - Magma Starscape": APSave.ChestTracers[18] = true; break;
                case "Chest Tracer - Gravity Bubble": APSave.ChestTracers[19] = true; break;
                case "Chest Tracer - Bakunawa Rush": APSave.ChestTracers[20] = true; break;
                case "Chest Tracer - Clockwork Arboretum": APSave.ChestTracers[21] = true; break;
                case "Chest Tracer - Inversion Dynamo": APSave.ChestTracers[22] = true; break;
                case "Chest Tracer - Lunar Cannon": APSave.ChestTracers[23] = true; break;
                case "Chest Tracer":
                    for (int tracerIndex = 0; tracerIndex < APSave.ChestTracers.Length; tracerIndex++)
                        APSave.ChestTracers[tracerIndex] = true;
                    break;

                // DEBUG: Warn that the given item is not yet handled on the client.
                #if DEBUG
                default: Console.WriteLine($"Item type '{ReceivedItem}' not yet handled!"); break;
                #endif
            }

            // Handle enabling the tracer if we're in a stage already.
            if (ReceivedItem.StartsWith("Chest Tracer"))
            {
                // Look for the player object.
                FPPlayer player = UnityEngine.Object.FindObjectOfType<FPPlayer>();

                // If the player exists, then recreate the chest tracers.
                if (player != null)
                    FPPlayerPatcher.CreateChestTracers();
            }

            // Force the game to save.
            FPSaveManager.SaveToFile(APSave.FPSaveManagerSlot);

            // Update our AP Save.
            SaveAPFile();

            static void GoldGem()
            {
                // Check the slot data to see which shops are enabled.
                bool millaShop = (long)SlotData["milla_shop"] == 1;
                bool vinylShop = (long)SlotData["vinyl_shop"] == 1;

                // Check if Milla's shop is enabled.
                if (millaShop)
                {
                    // Set up a counter to track how many locations for this shop have been checked.
                    int checkedCount = 0;

                    // Loop through each item for this shop and increment the counter if they've been checked.
                    foreach (var location in APSave.Locations.Where(location => location.Name.StartsWith("Shop - ") && !location.Name.Contains("Vinyl") && location.Checked))
                        checkedCount++;

                    // If we've gotten all 24 locations for Milla's shop, then disable it.
                    if (checkedCount >= 24)
                        millaShop = false;
                }

                // Check if the Vinyl shop is enabled.
                if (vinylShop)
                {
                    // Set up a counter to track how many locations for this shop have been checked.
                    int checkedCount = 0;

                    // Loop through each item for this shop and increment the counter if they've been checked.
                    foreach (var location in APSave.Locations.Where(location => location.Name.StartsWith("Shop - Vinyl") && location.Checked))
                        checkedCount++;

                    // If we've gotten all 66 locations for the Vinyl shop, then disable it.
                    if (checkedCount >= 66)
                        vinylShop = false;
                }

                // If Milla's shop is enabled, then give a Gold Gem.
                if (millaShop)
                    FPSaveManager.totalGoldGems++;

                // If not, but the vinyl shop is enabled, then give 100 crystals.
                else if (vinylShop)
                {
                    // Look for the player object.
                    FPPlayer player = UnityEngine.Object.FindObjectOfType<FPPlayer>();

                    // Edit the player's crystal counts if they exist.
                    if (player != null)
                    {
                        player.totalCrystals += 100;

                        player.crystals -= 100;
                        if (player.crystals > player.extraLifeCost)
                            player.crystals = player.extraLifeCost;

                        // Give a 1UP if the player has enough crystals (copy and pasted from the original source).
                        if (player.crystals < 1)
                        {
                            player.crystals = player.extraLifeCost;

                            if (player.lives < 9)
                                player.lives++;

                            CrystalBonus crystalBonus = (CrystalBonus)FPStage.CreateStageObject(CrystalBonus.classID, 292f, -64f);
                            crystalBonus.animator.Play("HUD_Add");
                            crystalBonus.duration = 40f;

                            InvincibilityStar invincibilityStar = (InvincibilityStar)FPStage.CreateStageObject(InvincibilityStar.classID, -100f, -100f);
                            invincibilityStar.parentObject = player;
                            invincibilityStar.distance = 320f;
                            invincibilityStar.descend = true;
                            InvincibilityStar invincibilityStar2 = (InvincibilityStar)FPStage.CreateStageObject(InvincibilityStar.classID, -100f, -100f);
                            invincibilityStar2.parentObject = player;
                            invincibilityStar2.rotation = 180f;
                            invincibilityStar2.distance = 320f;
                            invincibilityStar2.descend = true;

                            FPAudio.PlayJingle(3);
                        }
                    }

                    // If the player doesn't exist (because a stage isn't active for instance), then just add the crystals straight to the save.
                    else
                        FPSaveManager.totalCrystals += 100;
                }

                // If neither shop is active, then give the player a 1UP instead.
                else
                    FPPlayerPatcher.hasBufferedExtraLife = true;
            }
        }

        /// <summary>
        /// Applies a Barve Stone trap to the player.
        /// </summary>
        /// <param name="item">The Brave Stone to equip.</param>
        private static void SetTrapBraveStone(FPPowerup item)
        {
            // Actually find the player.
            FPPlayer player = UnityEngine.Object.FindObjectOfType<FPPlayer>();

            // Check that the player exists and we haven't Received this item when loading the file.
            if (player != null)
            {
                // If this Brave Stone is already equipped on the player, then don't add a second copy of it.
                if (player.powerups.Contains(item))
                    return;

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
        /// Returns a sprite icon for the given location.
        /// TODO: Can I shrink the Star Cards? They feel a bit too big in comparison to everything else.
        /// </summary>
        /// <param name="location">The location this sprite is for.</param>
        /// <param name="shouldUseHideFlags">Whether this sprite should abide by the shop_information slot data.</param>
        /// <returns>The sprite we've generated.</returns>
        public static Sprite GetItemSprite(Location location, bool shouldUseHideFlags = false)
        {
            // Set up a new texture.
            Texture2D texture = new(32, 32);

            // Change the texture to use point filtering.
            texture.filterMode = FilterMode.Point;

            // Load the Archipelago logo.
            texture.LoadImage(APLogo);

            // If the Show Item Names in Shops setting is set to either Hidden or Nothing, then return the base sprite no matter what.
            if (shouldUseHideFlags)
                if ((long)SlotData["shop_information"] >= 2)
                    return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1);

            // If this is a progression item or trap, then use the approriate sprite.
            if (location.Flags == ItemFlags.Advancement)
                texture.LoadImage(APLogo_Progression);
            if (location.Flags == ItemFlags.Trap)
                texture.LoadImage(APLogo_Trap);

            // If the Show Item Names in Shops setting is set to Flags, then return whichever AP Logo we have loaded.
            if (shouldUseHideFlags)
                if ((long)SlotData["shop_information"] == 1)
                    return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1);

            // If this icon is not for Freedom Planet 2, then check if one exists for this game and icon combination, if so, load it.
            if (location.Game != "Manual_FreedomPlanet2_Knuxfan24")
            {
                if (File.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sprites\{location.Game}\{location.Item}.png"))
                    texture.LoadImage(File.ReadAllBytes($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sprites\{location.Game}\{location.Item}.png"));
            }

            // Special case for certain games that use invalid characters.
            if (location.Game == "Balatro") // Uses forward slashes in some item names.
            {
                if (File.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sprites\{location.Game}\{location.Item.Replace('/', '_')}.png"))
                    texture.LoadImage(File.ReadAllBytes($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sprites\{location.Game}\{location.Item.Replace('/', '_')}.png"));
            }

            if (location.Game == "Terraria") // Uses colons in some item names.
            {
                if (File.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sprites\{location.Game}\{location.Item.Replace(':', '_')}.png"))
                    texture.LoadImage(File.ReadAllBytes($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sprites\{location.Game}\{location.Item.Replace(':', '_')}.png"));
            }

            // Don't try to read sprites from our Atlas' if they don't exist yet.
            if (ItemSpriteAtlas == null || OtherSpriteAtlas == null)
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1);

            // If this is a Freedom Planet 2 item, then get it straight from the texture atlas.
            // Power Up Start is handled seperately, as it appears differently depending on the player character.
            if (location.Game == "Manual_FreedomPlanet2_Knuxfan24")
            {
                if (location.Item != "Powerup Start")
                {
                    switch (location.Item)
                    {
                        case "Chest Tracer":
                        case "Chest Tracer - Dragon Valley":
                        case "Chest Tracer - Shenlin Park":
                        case "Chest Tracer - Tiger Falls":
                        case "Chest Tracer - Robot Graveyard":
                        case "Chest Tracer - Shade Armory":
                        case "Chest Tracer - Avian Museum":
                        case "Chest Tracer - Airship Sigwada":
                        case "Chest Tracer - Phoenix Highway":
                        case "Chest Tracer - Zao Land":
                        case "Chest Tracer - Globe Opera 1":
                        case "Chest Tracer - Globe Opera 2":
                        case "Chest Tracer - Palace Courtyard":
                        case "Chest Tracer - Tidal Gate":
                        case "Chest Tracer - Sky Bridge":
                        case "Chest Tracer - Lightning Tower":
                        case "Chest Tracer - Zulon Jungle":
                        case "Chest Tracer - Nalao Lake":
                        case "Chest Tracer - Ancestral Forge":
                        case "Chest Tracer - Magma Starscape":
                        case "Chest Tracer - Gravity Bubble":
                        case "Chest Tracer - Bakunawa Rush":
                        case "Chest Tracer - Clockwork Arboretum":
                        case "Chest Tracer - Inversion Dynamo":
                        case "Chest Tracer - Lunar Cannon": return GetSpriteFromAtlas(1002, 1380, 26, 26, true);
                        case "Crystals to Petals": return GetSpriteFromAtlas(495, 709, 24, 28);
                        case "Double Damage": return GetSpriteFromAtlas(646, 994, 23, 28);
                        case "Earth Charm": return GetSpriteFromAtlas(603, 795, 26, 27);
                        case "Element Burst": return GetSpriteFromAtlas(638, 924, 32, 32);
                        case "Expensive Stocks": return GetSpriteFromAtlas(606, 947, 29, 25);
                        case "Extra Item Slot": case "Extra Potion Slots": return GetSpriteFromAtlas(494, 606, 25, 33);
                        case "Fire Charm": return GetSpriteFromAtlas(532, 832, 25, 29);
                        case "Gold Gem": return GetSpriteFromAtlas(610, 640, 22, 22);
                        case "Items To Bombs": return GetSpriteFromAtlas(674, 897, 28, 29);
                        case "Life Oscillation": return GetSpriteFromAtlas(310, 715, 28, 27);
                        case "Max Life Up": return GetSpriteFromAtlas(809, 995, 18, 27);
                        case "Metal Charm": return GetSpriteFromAtlas(521, 675, 26, 30);
                        case "No Guarding": return GetSpriteFromAtlas(667, 959, 32, 30);
                        case "No Petals": return GetSpriteFromAtlas(528, 748, 18, 29);
                        case "No Revivals": return GetSpriteFromAtlas(709, 994, 28, 28);
                        case "No Stocks": return GetSpriteFromAtlas(703, 965, 24, 23);
                        case "One Hit KO": return GetSpriteFromAtlas(550, 931, 12, 15);
                        case "Payback Ring": return GetSpriteFromAtlas(574, 920, 26, 26);
                        case "Petal Armor": return GetSpriteFromAtlas(616, 862, 32, 27);
                        case "Potion - Accelerator": return GetSpriteFromAtlas(503, 574, 25, 30);
                        case "Potion - Attack Up": return GetSpriteFromAtlas(484, 247, 23, 29);
                        case "Potion - Cheaper Stocks": return GetSpriteFromAtlas(524, 640, 31, 30);
                        case "Potion - Extra Stock": return GetSpriteFromAtlas(675, 994, 28, 28);
                        case "Potion - Healing Strike": return GetSpriteFromAtlas(734, 959, 15, 31);
                        case "Potion - Strong Revivals": return GetSpriteFromAtlas(617, 895, 25, 25);
                        case "Potion - Strong Shields": return GetSpriteFromAtlas(494, 643, 20, 27);
                        case "Potion - Super Feather": return GetSpriteFromAtlas(552, 731, 26, 32);
                        case "Progressive Chapter":
                        case "Mystery of the Frozen North":
                        case "Sky Pirate Panic":
                        case "Enter the Battlesphere":
                        case "Globe Opera":
                        case "Justice in the Sky Paradise":
                        case "Robot Wars! Snake VS Tarsier":
                        case "Echoes of the Dragon War":
                        case "Bakunawa": return GetSpriteFromAtlas(777, 991, 26, 32);
                        case "Rainbow Charm": return GetSpriteFromAtlas(617, 826, 31, 32);
                        case "Shadow Guard": return GetSpriteFromAtlas(635, 794, 27, 28);
                        case "Star Card": return GetSpriteFromAtlas(985, 178, 26, 26, true);
                        case "Time Capsule": return GetSpriteFromAtlas(1050, 785, 20, 25, true);
                        case "Time Limit": return GetSpriteFromAtlas(495, 674, 20, 30);
                        case "Water Charm": return GetSpriteFromAtlas(584, 887, 27, 29);
                        case "Wood Charm": return GetSpriteFromAtlas(550, 865, 28, 27);

                        // DEBUG: Warn that the given FP2 item doesn't yet have a sprite.
                        #if DEBUG
                        default: Console.WriteLine($"Item {location.Item} currently has no sprite definied."); break;
                        #endif
                    }
                }
                else
                {
                    // TODO: In a test, the shop showed Carol's sprite for Neera's, is this my fault or some weirdness with the random character?
                    switch (FPSaveManager.character)
                    {
                        case FPCharacterID.LILAC: return GetSpriteFromAtlas(538, 798, 29, 28);
                        case FPCharacterID.CAROL: case FPCharacterID.BIKECAROL: return GetSpriteFromAtlas(492, 743, 31, 28);
                        case FPCharacterID.MILLA: return GetSpriteFromAtlas(552, 767, 26, 25);
                        case FPCharacterID.NEERA: return GetSpriteFromAtlas(702, 929, 26, 32);
                    }
                    
                }

                // Allow a custom sprite to overwrite the predefinied ones.
                if (File.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sprites\{location.Item}.png"))
                    texture.LoadImage(File.ReadAllBytes($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sprites\{location.Item}.png"));
            }

            // Return a sprite from our texture.
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1);
        }

        /// <summary>
        /// Gets a sprite out of a texture atlas.
        /// </summary>
        /// <param name="x">The x coordinate on the atlas for this sprite.</param>
        /// <param name="y">The y coordinate on the atlas for this sprite.</param>
        /// <param name="width">The width of this sprite.</param>
        /// <param name="height">The height of this sprite.</param>
        private static Sprite GetSpriteFromAtlas(float x, float y, float width, float height, bool otherAtlas = false)
        {
            if (!otherAtlas)
                return Sprite.Create(ItemSpriteAtlas, new Rect(x, ItemSpriteAtlas.height - (y + height), width, height), new Vector2(0.5f, 0.5f), 1);
            else
                return Sprite.Create(OtherSpriteAtlas, new Rect(x, OtherSpriteAtlas.height - (y + height), width, height), new Vector2(0.5f, 0.5f), 1);
        }

        /// <summary>
        /// Saves our AP file.
        /// </summary>
        public static void SaveAPFile() => File.WriteAllText($@"{Paths.GameRootPath}\Archipelago Saves\{Session.RoomState.Seed}_Save.json", JsonConvert.SerializeObject(APSave, Formatting.Indented));
    
        /// <summary>
        /// Gets the sprites for the Archipelago logo from the DLL.
        /// </summary>
        /// <param name="spriteName">The name of the resource to load.</param>
        /// <returns>The bytes that make up the resource.</returns>
        public static byte[] GetAPSprite(string spriteName)
        {
            // Load the resource into a stream.
            Stream sprite = Assembly.GetExecutingAssembly().GetManifestResourceStream(spriteName);

            // Create a binary reader from this stream.
            BinaryReader reader = new(sprite);

            // Read the length of the stream into a byte array and return it.
            return reader.ReadBytes((int)sprite.Length);
        }

        /// <summary>
        /// Changes the value of a text mesh to match what was typed into an invisible textbox.
        /// </summary>
        /// <param name="objectName">The name of the text mesh to update.</param>
        /// <param name="displayedText">The string to display.</param>
        private static void UpdateHiddenTextbox(string objectName, string displayedText)
        {
            // Find the TextMesh object.
            GameObject textmeshGameObject = UnityEngine.GameObject.Find(objectName);

            // Check that we've actually found the TextMesh object.
            if (textmeshGameObject != null)
            {
                // Find the actual TextMesh component of the object.
                TextMesh textmeshComponent = textmeshGameObject.GetComponent<TextMesh>();

                // Check that we've actually found the TextMesh component.
                if (textmeshComponent != null)
                {
                    // Check if our TextMesh's value doesn't match what we're inputting. (Including the square brackets)
                    if (textmeshComponent.text != $"[{displayedText}]")
                    {
                        // Update the text in the TextMesh.
                        textmeshComponent.text = $"[{displayedText}]";

                        // Play the menu move sound.
                        FPAudio.PlaySfx(FPAudio.SFX_MOVE);
                    }
                }
            }
        }
    }
}
