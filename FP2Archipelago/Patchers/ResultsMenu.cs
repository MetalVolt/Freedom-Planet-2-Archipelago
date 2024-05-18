using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Packets;

namespace FP2Archipelago.Patchers
{
    internal class ResultsMenu
    {
        /// <summary>
        /// Sends out a stage clear check.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPResultsMenu), "Start")]
        static void SendStageClearLocationCheck(ref int ___challengeID)
        {
            // Check if we need to disable a mirror trap.
            if (GlobalValues.IsMirrored)
            {
                // Find the pixel art camera.
                GameObject gameObject2 = GameObject.Find("Pixel Art Target");

                // If we've found it, then check its scale, if it's a negative number, then invert it.
                if (gameObject2 != null)
                    if (gameObject2.transform.localScale.x < 0)
                        gameObject2.transform.localScale = new Vector3(gameObject2.transform.localScale.x * -1f, gameObject2.transform.localScale.y, gameObject2.transform.localScale.z);

                // Disable the mirror value.
                GlobalValues.IsMirrored = false;
            }

            // Check if the stage we've just cleared is Weapon's Core.
            if (FPStage.currentStage.stageID == 30)
            {
                // Set up a status update with the goal flag.
                StatusUpdatePacket statusUpdatePacket = new() { Status = ArchipelagoClientState.ClientGoal };

                // Send the goal flag.
                GlobalValues.Session.Socket.SendPacket(statusUpdatePacket);

                // Don't bother running the rest of this function.
                return;
            }

            // Set up a value to store the location ID for the cleared stage.
            long? locationID = null;

            // Determine the location ID from the completed stage by looking up a value in the Locations dictonary.
            switch (FPStage.currentStage.stageID)
            {
                case 1:  locationID = GlobalValues.Locations["Dragon Valley - Clear"];       break;
                case 2:  locationID = GlobalValues.Locations["Shenlin Park - Clear"];        break;
                case 3:  locationID = GlobalValues.Locations["Avian Museum - Clear"];        break;
                case 4:  locationID = GlobalValues.Locations["Airship Sigwada - Clear"];     break;
                case 5:  locationID = GlobalValues.Locations["Tiger Falls - Clear"];         break;
                case 6:  locationID = GlobalValues.Locations["Robot Graveyard - Clear"];     break;
                case 7:  locationID = GlobalValues.Locations["Shade Armory - Clear"];        break;
                case 8:  locationID = GlobalValues.Locations["Snowfields - Clear"];          break;
                case 9:  locationID = GlobalValues.Locations["Phoenix Highway - Clear"];     break;
                case 10: locationID = GlobalValues.Locations["Zao Land - Clear"];            break;
                case 11: locationID = GlobalValues.Locations["Globe Opera 1 - Clear"];       break;
                case 12: locationID = GlobalValues.Locations["Globe Opera 2 - Clear"];       break;
                case 13: locationID = GlobalValues.Locations["Auditorium - Clear"];          break;
                case 14: locationID = GlobalValues.Locations["Palace Courtyard - Clear"];    break;
                case 15: locationID = GlobalValues.Locations["Tidal Gate - Clear"];          break;
                case 16: locationID = GlobalValues.Locations["Zulon Jungle - Clear"];        break;
                case 17: locationID = GlobalValues.Locations["Nalao Lake - Clear"];          break;
                case 18: locationID = GlobalValues.Locations["Sky Bridge - Clear"];          break;
                case 19: locationID = GlobalValues.Locations["Lightning Tower - Clear"];     break;
                case 20: locationID = GlobalValues.Locations["Ancestral Forge - Clear"];     break;
                case 21: locationID = GlobalValues.Locations["Magma Starscape - Clear"];     break;
                case 22: locationID = GlobalValues.Locations["Diamond Point - Clear"];       break;
                case 23: locationID = GlobalValues.Locations["Gravity Bubble - Clear"];      break;
                case 24: locationID = GlobalValues.Locations["Bakunawa Rush - Clear"];       break;
                case 25: locationID = GlobalValues.Locations["Refinery Room - Clear"];       break;
                case 26: locationID = GlobalValues.Locations["Clockwork Arboretum - Clear"]; break;
                case 27: locationID = GlobalValues.Locations["Inversion Dynamo - Clear"];    break;
                case 28: locationID = GlobalValues.Locations["Lunar Cannon - Clear"];        break;
                case 29: locationID = GlobalValues.Locations["Merga - Clear"];               break;
                case 32: locationID = GlobalValues.Locations["Bakunawa Chase - Clear"];      break;

                // Handle The Battlesphere differently due to its challenge ID system.
                case 31:
                    switch (___challengeID)
                    {
                        case 4:  locationID = GlobalValues.Locations["The Battlesphere - Clear"];           break;
                        case 6:  locationID = GlobalValues.Locations["The Battlesphere - Time Capsule 1"];  break;
                        case 7:  locationID = GlobalValues.Locations["The Battlesphere - Time Capsule 2"];  break;
                        case 8:  locationID = GlobalValues.Locations["The Battlesphere - Time Capsule 3"];  break;
                        case 9:  locationID = GlobalValues.Locations["The Battlesphere - Time Capsule 4"];  break;
                        case 10: locationID = GlobalValues.Locations["The Battlesphere - Time Capsule 5"];  break;
                        case 11: locationID = GlobalValues.Locations["The Battlesphere - Time Capsule 6"];  break;
                        case 12: locationID = GlobalValues.Locations["The Battlesphere - Time Capsule 7"];  break;
                        case 13: locationID = GlobalValues.Locations["The Battlesphere - Time Capsule 8"];  break;
                        case 14: locationID = GlobalValues.Locations["The Battlesphere - Time Capsule 9"];  break;
                        case 15: locationID = GlobalValues.Locations["The Battlesphere - Time Capsule 10"]; break;
                        case 16: locationID = GlobalValues.Locations["The Battlesphere - Time Capsule 11"]; break;
                        case 17: locationID = GlobalValues.Locations["The Battlesphere - Time Capsule 12"]; break;
                    }
                    break;

                // Panic if we haven't handled this stage ID.
                default: Console.WriteLine($"Stage ID {FPStage.currentStage.stageID} not handled!"); break;
            }

            // If we've read a location ID, then send the item at it.
            if (locationID != null)
                GlobalValues.Session.Locations.CompleteLocationChecks((long)locationID);
        }
    }
}
