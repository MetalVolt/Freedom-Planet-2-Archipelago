using System.Linq;

namespace FP2Archipelago.Patchers
{
    internal class Chest
    {
        /// <summary>
        /// Sets the sprite and message for the item displayed when opening the chest.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemChest), "Start")]
        static void SetSpriteAndMessage(ref FPItemChestContent ___contents, ref FPPowerup ___powerupType, ref FPMusicTrack ___musicID, ref Sprite ___itemSprite,
                                        ref string ___labelMessage)
        {
            // Set up some values to track the player, the item, the location and the game.
            string player = "";
            string item = "";
            string location = "";
            string game = "";

            // Check if this chest is a power up and get the location from it.
            if (___contents == FPItemChestContent.POWERUP)
            {
                switch (___powerupType)
                {
                    case FPPowerup.TIME_LIMIT:       location = "Dragon Valley - Brave Stone";                    break;
                    case FPPowerup.STOCK_DRAIN:      location = "Shenlin Park - Brave Stone";                     break;
                    case FPPowerup.NO_GUARDING:      location = "Tiger Falls - Brave Stone";                      break;
                    case FPPowerup.NO_PETALS:        location = "Avian Museum - Brave Stone";                     break;
                    case FPPowerup.DOUBLE_DAMAGE:    location = "Airship Sigwada - Brave Stone";                  break;
                    case FPPowerup.NO_REVIVALS:      location = "Phoenix Highway - Brave Stone";                  break;
                    case FPPowerup.EXPENSIVE_STOCKS: location = "Zao Land - Brave Stone";                         break;
                    default:                         Console.WriteLine($"Item ID {___powerupType} not handled!"); break;
                }
            }

            // Check if this chest is a vinyl and get the location from it.
            if (___contents == FPItemChestContent.MUSIC)
            {
                switch (___musicID)
                {
                    case FPMusicTrack.STAGE_VALLEY:    location = "Dragon Valley - Vinyl";                       break;
                    case FPMusicTrack.STAGE_PARK:      location = "Shenlin Park - Vinyl";                        break;
                    case FPMusicTrack.STAGE_FALLS:     location = "Tiger Falls - Vinyl";                         break;
                    case FPMusicTrack.STAGE_GRAVEYARD: location = "Robot Graveyard - Vinyl";                     break;
                    case FPMusicTrack.STAGE_ARMORY:    location = "Shade Armory - Vinyl";                        break;
                    case FPMusicTrack.STAGE_MUSEUM:    location = "Avian Museum - Vinyl";                        break;
                    case FPMusicTrack.STAGE_SIGWADA:   location = "Airship Sigwada - Vinyl";                     break;
                    case FPMusicTrack.STAGE_HIGHWAY:   location = "Phoenix Highway - Vinyl";                     break;
                    case FPMusicTrack.STAGE_ZAOLAND:   location = "Zao Land - Vinyl";                            break;
                    case FPMusicTrack.STAGE_OPERA1:    location = "Globe Opera 1 - Vinyl";                       break;
                    case FPMusicTrack.STAGE_OPERA2B:   location = "Globe Opera 2 - Vinyl";                       break;
                    case FPMusicTrack.STAGE_COURTYARD: location = "Palace Courtyard - Vinyl";                    break;
                    case FPMusicTrack.STAGE_GATE:      location = "Tidal Gate - Vinyl";                          break;
                    case FPMusicTrack.STAGE_BRIDGE:    location = "Sky Bridge - Vinyl";                          break;
                    case FPMusicTrack.STAGE_TOWER:     location = "Lightning Tower - Vinyl";                     break;
                    case FPMusicTrack.STAGE_JUNGLE:    location = "Zulon Jungle - Vinyl";                        break;
                    case FPMusicTrack.STAGE_LAKE:      location = "Nalao Lake - Vinyl";                          break;
                    case FPMusicTrack.STAGE_FORGE:     location = "Ancestral Forge - Vinyl";                     break;
                    case FPMusicTrack.STAGE_STARSCAPE: location = "Magma Starscape - Vinyl";                     break;
                    case FPMusicTrack.STAGE_BUBBLE:    location = "Gravity Bubble - Vinyl";                      break;
                    case FPMusicTrack.STAGE_FINALE1:   location = "Bakunawa Rush - Vinyl";                       break;
                    case FPMusicTrack.STAGE_FINALE2:   location = "Clockwork Arboretum - Vinyl";                 break;
                    case FPMusicTrack.STAGE_FINALE3:   location = "Inversion Dynamo - Vinyl";                    break;
                    case FPMusicTrack.STAGE_FINALE4:   location = "Lunar Cannon - Vinyl";                        break;
                    default:                           Console.WriteLine($"Vinyl ID {___musicID} not handled!"); break;
                }
            }

            // If we have a location, then scout it to set player and item, if not, then return.
            if (location != "")
                GlobalValues.Session.Locations.ScoutLocationsAsync(locationInfoPacket => { player = GlobalValues.Session.Players.GetPlayerName(locationInfoPacket.Locations[0].Player); item = GlobalValues.Session.Items.GetItemName(locationInfoPacket.Locations[0].Item); }, new long[1] { GlobalValues.Locations[location] });
            else
                return;

            // Twiddle our thumbs waiting for the async operation to finish.
            while (player == "" || item == "")
                Console.WriteLine("Outpaced the async operation, waiting.");

            // Get the game of the player this item is for.
            game = GlobalValues.Session.Players.AllPlayers.FirstOrDefault(i => i.Name == player).Game;

            // Print things for testing.
            // Console.WriteLine($"Location {location} holds a(n) {item} for {player} playing {game}");

            // Start the label message.
            ___labelMessage = "Found ";

            // Determine if the label message should say your or say another player's name.
            if (GlobalValues.Session.Players.GetPlayerName(GlobalValues.Session.ConnectionInfo.Slot) == player)
                ___labelMessage += "your ";
            else
                ___labelMessage += $"{player}'s ";

            // End the label message.
            ___labelMessage += $"{item}.";

            // Set the item's sprite.
            ___itemSprite = Plugin.GetItemSprite(game, item);
        }

        /// <summary>
        /// Sends a location out from opening a Vinyl/Brave Stone chest.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemChest), "BoxHit")]
        static void SendChestLocationCheck(ref FPItemChestContent ___contents, ref FPPowerup ___powerupType, ref FPMusicTrack ___musicID)
        {
            // Set up a value to store the location ID for this chest.
            long? locationID = null;

            // Check if this chest is a power up and get the location from it.
            if (___contents == FPItemChestContent.POWERUP)
            {
                switch (___powerupType)
                {
                    case FPPowerup.TIME_LIMIT:       locationID = GlobalValues.Locations["Dragon Valley - Brave Stone"];   break;
                    case FPPowerup.STOCK_DRAIN:      locationID = GlobalValues.Locations["Shenlin Park - Brave Stone"];    break;
                    case FPPowerup.NO_GUARDING:      locationID = GlobalValues.Locations["Tiger Falls - Brave Stone"];     break;
                    case FPPowerup.NO_PETALS:        locationID = GlobalValues.Locations["Avian Museum - Brave Stone"];    break;
                    case FPPowerup.DOUBLE_DAMAGE:    locationID = GlobalValues.Locations["Airship Sigwada - Brave Stone"]; break;
                    case FPPowerup.NO_REVIVALS:      locationID = GlobalValues.Locations["Phoenix Highway - Brave Stone"]; break;
                    case FPPowerup.EXPENSIVE_STOCKS: locationID = GlobalValues.Locations["Zao Land - Brave Stone"];        break;
                    default:                         Console.WriteLine($"Item ID {___powerupType} not handled!");          break;
                }
            }

            // Check if this chest is a vinyl and get the location from it.
            if (___contents == FPItemChestContent.MUSIC)
            {
                switch (___musicID)
                {
                    case FPMusicTrack.STAGE_VALLEY:    locationID = GlobalValues.Locations["Dragon Valley - Vinyl"];       break;
                    case FPMusicTrack.STAGE_PARK:      locationID = GlobalValues.Locations["Shenlin Park - Vinyl"];        break;
                    case FPMusicTrack.STAGE_FALLS:     locationID = GlobalValues.Locations["Tiger Falls - Vinyl"];         break;
                    case FPMusicTrack.STAGE_GRAVEYARD: locationID = GlobalValues.Locations["Robot Graveyard - Vinyl"];     break;
                    case FPMusicTrack.STAGE_ARMORY:    locationID = GlobalValues.Locations["Shade Armory - Vinyl"];        break;
                    case FPMusicTrack.STAGE_MUSEUM:    locationID = GlobalValues.Locations["Avian Museum - Vinyl"];        break;
                    case FPMusicTrack.STAGE_SIGWADA:   locationID = GlobalValues.Locations["Airship Sigwada - Vinyl"];     break;
                    case FPMusicTrack.STAGE_HIGHWAY:   locationID = GlobalValues.Locations["Phoenix Highway - Vinyl"];     break;
                    case FPMusicTrack.STAGE_ZAOLAND:   locationID = GlobalValues.Locations["Zao Land - Vinyl"];            break;
                    case FPMusicTrack.STAGE_OPERA1:    locationID = GlobalValues.Locations["Globe Opera 1 - Vinyl"];       break;
                    case FPMusicTrack.STAGE_OPERA2B:   locationID = GlobalValues.Locations["Globe Opera 2 - Vinyl"];       break;
                    case FPMusicTrack.STAGE_COURTYARD: locationID = GlobalValues.Locations["Palace Courtyard - Vinyl"];    break;
                    case FPMusicTrack.STAGE_GATE:      locationID = GlobalValues.Locations["Tidal Gate - Vinyl"];          break;
                    case FPMusicTrack.STAGE_BRIDGE:    locationID = GlobalValues.Locations["Sky Bridge - Vinyl"];          break;
                    case FPMusicTrack.STAGE_TOWER:     locationID = GlobalValues.Locations["Lightning Tower - Vinyl"];     break;
                    case FPMusicTrack.STAGE_JUNGLE:    locationID = GlobalValues.Locations["Zulon Jungle - Vinyl"];        break;
                    case FPMusicTrack.STAGE_LAKE:      locationID = GlobalValues.Locations["Nalao Lake - Vinyl"];          break;
                    case FPMusicTrack.STAGE_FORGE:     locationID = GlobalValues.Locations["Ancestral Forge - Vinyl"];     break;
                    case FPMusicTrack.STAGE_STARSCAPE: locationID = GlobalValues.Locations["Magma Starscape - Vinyl"];     break;
                    case FPMusicTrack.STAGE_BUBBLE:    locationID = GlobalValues.Locations["Gravity Bubble - Vinyl"];      break;
                    case FPMusicTrack.STAGE_FINALE1:   locationID = GlobalValues.Locations["Bakunawa Rush - Vinyl"];       break;
                    case FPMusicTrack.STAGE_FINALE2:   locationID = GlobalValues.Locations["Clockwork Arboretum - Vinyl"]; break;
                    case FPMusicTrack.STAGE_FINALE3:   locationID = GlobalValues.Locations["Inversion Dynamo - Vinyl"];    break;
                    case FPMusicTrack.STAGE_FINALE4:   locationID = GlobalValues.Locations["Lunar Cannon - Vinyl"];        break;
                    default:                           Console.WriteLine($"Vinyl ID {___musicID} not handled!");           break;
                }
            }

            // If we've read a location ID, then send the item at it.
            if (locationID != null)
                GlobalValues.Session.Locations.CompleteLocationChecks((long)locationID);
        }
    }
}
