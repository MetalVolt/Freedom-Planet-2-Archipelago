global using BepInEx;
global using HarmonyLib;
global using System;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using Freedom_Planet_2_Archipelago.Patchers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Freedom_Planet_2_Archipelago
{
    // TODO: Better Item Get Feedback, it at least scales now.
    // TODO: RingLink packet sending causes stutters, try to fix that.
    public class APSave()
    {
        /// <summary>
        /// The save slot this AP seed should use.
        /// </summary>
        public int FPSaveManagerSlot;

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

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
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
        public static float DoubleGravityTrapTimer = 0f;
        public static float MirrorTrapTimer = 0f;
        public static float MoonGravityTrapTimer = 0f;
        public static float NotifyMessageTimer = 0f;

        // Store our slot data.
        public static Dictionary<string, object> SlotData = [];

        // Set up the values for the connection menu.
        string serverAddress = "localhost:62746";
        string slotName = "Knux";
        string password = "";

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

        /// <summary>
        /// Initial code that runs when BepInEx loads our plugin.
        /// </summary>
        private void Awake()
        {
            // Load the Stage Debug Menu to act as a connector menu.
            SceneManager.LoadScene("StageDebugMenu");

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

            // Patch the FP Player class, used to send out DeathLinks.
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

            // Enable the Music Randomiser.
            if (Config.Bind("Music Randomiser", "Enable Music Randomiser", true, "Whether or not to use the music randomiser.").Value)
                harmony.PatchAll(typeof(MusicRandomiser));
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

            // Check if we're on the Stage Debug Menu scene.
            if (SceneManager.GetActiveScene().name == "StageDebugMenu")
            {
                // Make the cursor visible.
                Cursor.visible = true;

                // Create the login labels.
                // TODO: These look ugly.
                GUI.Label(new Rect(16, 70, 150, 20), "Host: ");
                GUI.Label(new Rect(16, 90, 150, 20), "Player Name: ");
                GUI.Label(new Rect(16, 110, 150, 20), "Password: ");

                // Create the login textboxes.
                // TODO: These look ugly.
                serverAddress = GUI.TextField(new Rect(150, 70, 150, 20), serverAddress);
                slotName = GUI.TextField(new Rect(150, 90, 150, 20), slotName);
                password = GUI.TextField(new Rect(150, 110, 150, 20), password);

                // Create and check the connect button.
                if (GUI.Button(new Rect(16, 130, 100, 20), "Connect"))
                {
                    // Print that we're attempting to connect.
                    Logger.LogInfo($"Attempting to connect to Archipelago server at {serverAddress}.");

                    // Create a session and try to login.
                    Session = ArchipelagoSessionFactory.CreateSession(serverAddress);
                    LoginResult = Session.TryConnectAndLogin("Manual_FreedomPlanet2_Knuxfan24", slotName, ItemsHandlingFlags.AllItems, null, null, null, password);

                    // Check if we've successfully logged in.
                    if (LoginResult.Successful)
                    {
                        // Get our slot data.
                        SlotData = Session.DataStorage.GetSlotData();

                        // DEBUG: Print all the key value pairs in the slotdata and their datatypes.
                        foreach (var key in SlotData)
                            Console.WriteLine($"{key.Key}: {key.Value} (Type: {key.Value.GetType()})");

                        // Reveal all the map tiles.
                        // Dragon Valley and Shenlin Park.
                        FPSaveManager.mapTileReveal[0] = true;
                        FPSaveManager.mapTileReveal[1] = true;

                        // Mystery of the Frozen North.
                        FPSaveManager.mapTileReveal[4] = true;
                        FPSaveManager.mapTileReveal[5] = true;
                        FPSaveManager.mapTileReveal[6] = true;
                        FPSaveManager.mapTileReveal[7] = true;

                        // Sky Pirate Panic.
                        FPSaveManager.mapTileReveal[2] = true;
                        FPSaveManager.mapTileReveal[3] = true;

                        // Enter the Battlesphere.
                        FPSaveManager.mapTileReveal[8] = true;
                        FPSaveManager.mapTileReveal[9] = true;
                        FPSaveManager.mapTileReveal[10] = true;

                        // Globe Opera.
                        FPSaveManager.mapTileReveal[11] = true;
                        FPSaveManager.mapTileReveal[12] = true;
                        FPSaveManager.mapTileReveal[13] = true;
                        FPSaveManager.mapTileReveal[14] = true;
                        FPSaveManager.mapTileReveal[15] = true;

                        // Justice in the Sky Paradise.
                        FPSaveManager.mapTileReveal[18] = true;
                        FPSaveManager.mapTileReveal[19] = true;

                        // Robot Wars! Snake VS Tarsier.
                        FPSaveManager.mapTileReveal[16] = true;
                        FPSaveManager.mapTileReveal[17] = true;

                        // Echoes of the Dragon War.
                        FPSaveManager.mapTileReveal[20] = true;
                        FPSaveManager.mapTileReveal[21] = true;
                        FPSaveManager.mapTileReveal[22] = true;

                        // Bakunawa.
                        FPSaveManager.mapTileReveal[23] = true;
                        FPSaveManager.mapTileReveal[32] = true;
                        FPSaveManager.mapTileReveal[24] = true;
                        FPSaveManager.mapTileReveal[25] = true;
                        FPSaveManager.mapTileReveal[26] = true;
                        FPSaveManager.mapTileReveal[27] = true;
                        FPSaveManager.mapTileReveal[28] = true;
                        FPSaveManager.mapTileReveal[29] = true;
                        FPSaveManager.mapTileReveal[30] = true;

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
                                Logger.LogInfo($"Getting information for location {Location.Name} with an index of {Location.Index}.");

                                // Scout the location for the player, item, flags and game.
                                Session.Locations.ScoutLocationsAsync(locationInfoPacket => { Location.Player = Session.Players.GetPlayerName(locationInfoPacket[Location.Index].Player); Location.Item = locationInfoPacket[Location.Index].ItemName; Location.Flags = locationInfoPacket[Location.Index].Flags; Location.Game = locationInfoPacket[Location.Index].ItemGame; }, new long[1] { Session.Locations.AllLocations[locationIndex] });

                                // Twiddle our thumbs waiting for the async operation to finish.
                                while (Location.Player == "" || Location.Item == "" || Location.Game == "")
                                    System.Threading.Thread.Sleep(1);

                                // DEBUG: Report the data on the item at this location.
                                // Logger.LogInfo($"Found {Location.Player}'s {Location.Item} for {Location.Game} with flags {Location.Flags} at {Session.Locations.GetLocationNameFromId(Session.Locations.AllLocations[locationIndex])} (location index {Location.Index})");

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
                            Logger.LogInfo($"Loaded {APSave.Locations.Length} location checks.");
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

                        // Always keep Dragon Valley and Shenlin Park unlocked.
                        FPSaveManager.mapTileReveal[0] = true;
                        FPSaveManager.mapTileReveal[1] = true;
                        
                        // Set up values to check our extra item slots and gold gems.
                        int serverDoubleGravityTrapCount = 0;
                        int serverExtraItemSlots = 0;
                        int serverGoldGemCount = 0;
                        int serverMirrorTrapCount = 0;
                        int serverMoonGravityTrapCount = 0;
                        int serverStarCardCount = 0;
                        int serverTimeCapsuleCount = 0;

                        // Loop through each item the server has sent and receive it.
                        foreach (ItemInfo item in Session.Items.AllItemsReceived)
                        {
                            // Split the item's name.
                            string itemName = item.ItemName;

                            // Check the item's name.
                            switch (itemName)
                            {
                                case "Double Gravity Trap": serverDoubleGravityTrapCount = StartItemCheck(serverDoubleGravityTrapCount, APSave.DoubleGravityTrapCount, itemName); break;
                                case "Extra Item Slot": serverExtraItemSlots = StartItemCheck(serverExtraItemSlots, APSave.ExtraItemSlots, itemName); break;
                                case "Gold Gem": serverGoldGemCount = StartItemCheck(serverGoldGemCount, APSave.GoldGemCount, itemName); break;
                                case "Mirror Trap": serverMirrorTrapCount = StartItemCheck(serverGoldGemCount, APSave.MirrorTrapCount, itemName); break;
                                case "Moon Gravity Trap": serverMoonGravityTrapCount = StartItemCheck(serverMoonGravityTrapCount, APSave.MoonGravityTrapCount, itemName); break;
                                case "Star Card": serverStarCardCount = StartItemCheck(serverStarCardCount, APSave.StarCardCount, itemName); break;
                                case "Time Capsule": serverTimeCapsuleCount = StartItemCheck(serverTimeCapsuleCount, APSave.TimeCapsuleCount, itemName); break;

                                default: ReceiveItem(itemName, true); break;
                            }

                            // Remove this item from the queue.
                            Session.Items.DequeueItem();
                        }

                        // Hide the cursor again.
                        Cursor.visible = false;

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
                    }
                }
            }

            // Set up the GUI stuff for the notify message text.
            // TODO: This is really meant to be temporary, it looks ugly.

            // Create the style for the notify message text.
            GUIStyle centeredStyle = new(GUI.skin.label) { alignment = TextAnchor.UpperCenter };

            // Set the style's colour to black for the drop shadow.
            centeredStyle.normal.textColor = UnityEngine.Color.black;

            // Write the notify message text at the bottom center of the screen, offset by one pixel.
            GUI.Label(new(0, 0, 608, 32) { center = new Vector2((640 / 2) + 1, (360 - 8) + 1) }, NotifyMessage, centeredStyle);

            // Set the style's colour to white.
            centeredStyle.normal.textColor = UnityEngine.Color.white;

            // Write the notify message text at the bottom center of the screen.
            GUI.Label(new(0, 0, 608, 32) { center = new Vector2(640 / 2, 360 - 8) }, NotifyMessage, centeredStyle);
        }

        private void Socket_ReceiveDeathLink(DeathLink deathLinkHelper)
        {
            // Present the cause and source of the DeathLink.
            // TODO: Test how this message appears for different games, I'm not sure if all of them give a cause. SA2 also puts the player name IN the cause.
            NotifyMessage = $@"{deathLinkHelper.Cause} [{deathLinkHelper.Source}]";
            NotifyMessageTimer = 300f;

            // Stop the player from being able to send a DeathLink out.
            FPPlayerPatcher.canSendDeathLink = false;

            // Set the flag to tell the player patcher that a DeathLink is awaiting.
            FPPlayerPatcher.hasBufferedDeathLink = true;
        }

        /// <summary>
        /// Receives an item from the multiworld.
        /// </summary>
        /// <param name="receivedItemsHelper">The helper handling the item receive.</param>
        private void Socket_ReceiveItem(Archipelago.MultiClient.Net.Helpers.ReceivedItemsHelper receivedItemsHelper)
        {
            // DEBUG: Print that this helper was fired.
            Console.WriteLine($"Item received helper fired for {receivedItemsHelper.PeekItem().ItemName} from {receivedItemsHelper.PeekItem().Player.Name}.");

            // Set the notify message timer to 300 (roughly five seconds?).
            NotifyMessageTimer = 300f;

            // Set the notify message depending on if we received this item from ourselves.
            if (receivedItemsHelper.PeekItem().Player.Name != Session.Players.GetPlayerName(Session.ConnectionInfo.Slot))
                NotifyMessage = $"Received {receivedItemsHelper.PeekItem().ItemName} from {receivedItemsHelper.PeekItem().Player.Name}";
            else
                NotifyMessage = $"Found your {receivedItemsHelper.PeekItem().ItemName}";

            ReceiveItem(receivedItemsHelper.PeekItem().ItemName);
            receivedItemsHelper.DequeueItem();
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
                                if (player.health >  1f)
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
        /// Code that runs every frame.
        /// </summary>
        private void Update()
        {
            // If the active scene is the Battlesphere Time Capsule cutscene, then boot the player out to the arena menu instead.
            if (SceneManager.GetActiveScene().name == "Cutscene_BattlesphereCapsule")
                SceneManager.LoadScene("ArenaMenu");

            // Check if the notify message timer is going and decrement it if so.
            if (NotifyMessageTimer > 0f)
                NotifyMessageTimer -= FPStage.deltaTime;

            // Check if the notify message timer has gone below 0.
            else if (NotifyMessageTimer < 0f)
            {
                // Set the timer to 0 so this check doesn't refire.
                NotifyMessageTimer = 0f;

                // Clear the notify message message.
                NotifyMessage = "";
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
                        { "source", Plugin.Session.ConnectionInfo.Slot },
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

        private int StartItemCheck(int serverValue, int saveValue, string itemName)
        {
            // Check if we have less of this item than the save reports.
            if (serverValue < saveValue)
            {
                // Increment the count.
                serverValue++;

                // Inform that we've skipped giving this item.
                Logger.LogInfo($"Skipped giving {itemName} as it was already received.");
            }

            // If we don't have less of this item than the save reports, then receive it.
            else
                ReceiveItem(itemName, true);

            // Return the value the server's given.
            return serverValue;
        }
    
        /// <summary>
        /// Handles receiving an item from the multiworld.
        /// </summary>
        /// <param name="ReceivedItem">The name of the item we're receiving.</param>
        private void ReceiveItem(string ReceivedItem, bool fromStart = false)
        {
            // Only play the sound if we're recieving this item in game rather than upon connect.
            if (!fromStart)
                FPAudio.PlaySfx(FPAudio.SFX_ITEMGET);

            // Check the item we're receiving.
            switch (ReceivedItem)
            {
                case "Crystals to Petals": APSave.UnlockedBraveStones[7] = true; break;

                case "Double Damage":
                    APSave.UnlockedBraveStones[20] = true;
                    SetTrapBraveStone(FPPowerup.DOUBLE_DAMAGE);
                    break;

                // Set the Double Gravity Trap Timer to 1750 (roughly thirty seconds?) and increment our save's double gravity trap count.
                case "Double Gravity Trap":
                    DoubleGravityTrapTimer = 1750f;
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

                // Increment the save manager's total gold gems count and our save's gold gem count.
                case "Gold Gem":
                    FPSaveManager.totalGoldGems++;
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

                // Set the Mirror Trap Timer to 3000 (roughly a minute?) and increment our save's mirror trap count.
                case "Mirror Trap":
                    MirrorTrapTimer = 3000f;
                    APSave.MirrorTrapCount++;
                    break;

                // Set the Moon Gravity Trap Timer to 1750 (roughly thirty seconds?) and increment our save's mirror trap count.
                case "Moon Gravity Trap":
                    MoonGravityTrapTimer = 1750f;
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

                // Unlock certain levels on the map.
                case "Progressive Chapter":
                    // Always keep Dragon Valley and Shenlin Park unlocked.
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

                // Warn that the given item is not yet handled on the client.
                default: Logger.LogWarning($"Item type '{ReceivedItem}' not yet handled!"); break;
            }

            // Force the game to save.
            FPSaveManager.SaveToFile(APSave.FPSaveManagerSlot);

            // Update our AP Save.
            SaveAPFile();
        }

        /// <summary>
        /// Applies a Barve Stone trap to the player.
        /// </summary>
        /// <param name="fromStart"></param>
        /// <param name="item"></param>
        private static void SetTrapBraveStone(FPPowerup item)
        {
            // Actually find the player.
            FPPlayer player = UnityEngine.Object.FindObjectOfType<FPPlayer>();

            // Check that the player exists and we haven't Received this item when loading the file.
            if (player != null)
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
        /// Returns a sprite icon for the given location.
        /// TODO: Can we find a way to just pull FP2's actual sprites rather than having them as seperate files? I feel like that's not the best approach in the long term.
        /// </summary>
        /// <param name="location">The location this sprite is for.</param>
        /// <returns>The sprite we've generated.</returns>
        public static Sprite GetItemSprite(Location location)
        {
            // Set the path to the mod overrides folder so we don't keep typing it out.
            string apPath = $@"{Paths.GameRootPath}\mod_overrides\Archipelago";

            // Set up a new texture.
            Texture2D texture = new(32, 32);

            // Load the Archipelago logo.
            texture.LoadImage(File.ReadAllBytes($@"{apPath}\ap_logo.png"));

            // If this is a progression item or trap, then use the approriate sprite.
            if (location.Flags == ItemFlags.Advancement)
                texture.LoadImage(File.ReadAllBytes($@"{apPath}\ap_logo_progression.png"));
            if (location.Flags == ItemFlags.Trap)
                texture.LoadImage(File.ReadAllBytes($@"{apPath}\ap_logo_trap.png"));

            // If this icon is not for Freedom Planet 2, then check if one exists for this game and icon combination, if so, load it.
            if (location.Game != "Manual_FreedomPlanet2_Knuxfan24")
            {
                if (File.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\{location.Game}\{location.Item}.png"))
                    texture.LoadImage(File.ReadAllBytes($@"{apPath}\{location.Game}\{location.Item}.png"));
            }

            // If this icon IS for Freedom Planet 2, then look through the sprites and find this item's.
            else
            {
                switch (location.Item)
                {
                    case "Gold Gem":                 texture.LoadImage(File.ReadAllBytes($@"{apPath}\gold_gem.png"));                                                break;
                    case "Star Card":                texture.LoadImage(File.ReadAllBytes($@"{apPath}\star_card.png"));                                               break;
                    case "Time Capsule":             texture.LoadImage(File.ReadAllBytes($@"{apPath}\time_capsule.png"));                                            break;
                    case "Potion - Extra Stock":     texture.LoadImage(File.ReadAllBytes($@"{apPath}\extra_stocks.png"));                                            break;
                    case "Potion - Strong Revivals": texture.LoadImage(File.ReadAllBytes($@"{apPath}\strong_revives.png"));                                          break;
                    case "Potion - Cheaper Stocks":  texture.LoadImage(File.ReadAllBytes($@"{apPath}\cheap_stocks.png"));                                            break;
                    case "Potion - Healing Strike":  texture.LoadImage(File.ReadAllBytes($@"{apPath}\healing_strike.png"));                                          break;
                    case "Potion - Attack Up":       texture.LoadImage(File.ReadAllBytes($@"{apPath}\attack_up.png"));                                               break;
                    case "Potion - Strong Shields":  texture.LoadImage(File.ReadAllBytes($@"{apPath}\strong_shields.png"));                                          break;
                    case "Potion - Accelerator":     texture.LoadImage(File.ReadAllBytes($@"{apPath}\accelerator.png"));                                             break;
                    case "Potion - Super Feather":   texture.LoadImage(File.ReadAllBytes($@"{apPath}\super_feather.png"));                                           break;
                    case "Element Burst":            texture.LoadImage(File.ReadAllBytes($@"{apPath}\element_burst.png"));                                           break;
                    case "Max Life Up":              texture.LoadImage(File.ReadAllBytes($@"{apPath}\max_life_up.png"));                                             break;
                    case "Crystals to Petals":       texture.LoadImage(File.ReadAllBytes($@"{apPath}\crystals_to_petals.png"));                                      break;
                    case "Powerup Start":            texture.LoadImage(File.ReadAllBytes($@"{apPath}\power_start_{FPPlayerPatcher.GetPlayer().ToLower()}.png"));     break;
                    case "Shadow Guard":             texture.LoadImage(File.ReadAllBytes($@"{apPath}\shadow_guard.png"));                                            break;
                    case "Payback Ring":             texture.LoadImage(File.ReadAllBytes($@"{apPath}\payback_ring.png"));                                            break;
                    case "Wood Charm":               texture.LoadImage(File.ReadAllBytes($@"{apPath}\wood_charm.png"));                                              break;
                    case "Earth Charm":              texture.LoadImage(File.ReadAllBytes($@"{apPath}\earth_charm.png"));                                             break;
                    case "Water Charm":              texture.LoadImage(File.ReadAllBytes($@"{apPath}\water_charm.png"));                                             break;
                    case "Fire Charm":               texture.LoadImage(File.ReadAllBytes($@"{apPath}\fire_charm.png"));                                              break;
                    case "Metal Charm":              texture.LoadImage(File.ReadAllBytes($@"{apPath}\metal_charm.png"));                                             break;
                    case "No Stocks":                texture.LoadImage(File.ReadAllBytes($@"{apPath}\no_stocks.png"));                                               break;
                    case "Expensive Stocks":         texture.LoadImage(File.ReadAllBytes($@"{apPath}\expensive_stocks.png"));                                        break;
                    case "Double Damage":            texture.LoadImage(File.ReadAllBytes($@"{apPath}\double_damage.png"));                                           break;
                    case "No Revivals":              texture.LoadImage(File.ReadAllBytes($@"{apPath}\no_revives.png"));                                              break;
                    case "No Guarding":              texture.LoadImage(File.ReadAllBytes($@"{apPath}\no_guarding.png"));                                             break;
                    case "No Petals":                texture.LoadImage(File.ReadAllBytes($@"{apPath}\no_petals.png"));                                               break;
                    case "Time Limit":               texture.LoadImage(File.ReadAllBytes($@"{apPath}\time_limit.png"));                                              break;
                    case "Items To Bombs":           texture.LoadImage(File.ReadAllBytes($@"{apPath}\items_to_bombs.png"));                                          break;
                    case "Life Oscillation":         texture.LoadImage(File.ReadAllBytes($@"{apPath}\life_oscillation.png"));                                        break;
                    case "One Hit KO":               texture.LoadImage(File.ReadAllBytes($@"{apPath}\one_hit_ko.png"));                                              break;
                    case "Petal Armor":              texture.LoadImage(File.ReadAllBytes($@"{apPath}\petal_armour.png"));                                            break;
                    case "Rainbow Charm":            texture.LoadImage(File.ReadAllBytes($@"{apPath}\rainbow_charm.png"));                                           break;
                    default:                         Console.WriteLine($"Item {location.Item} currently has no sprite definied.");                                   break;
                }
            }

            // Change the texture to use point filtering.
            texture.filterMode = FilterMode.Point;

            // Return a sprite from our texture.
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1);
        }

        /// <summary>
        /// Saves our AP file.
        /// </summary>
        public static void SaveAPFile() => File.WriteAllText($@"{Paths.GameRootPath}\Archipelago Saves\{Session.RoomState.Seed}_Save.json", JsonConvert.SerializeObject(APSave, Formatting.Indented));
    }
}
