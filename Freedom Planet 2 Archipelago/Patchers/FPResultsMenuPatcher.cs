using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Packets;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class FPResultsMenuPatcher
    {
        /// <summary>
        /// Sends out a stage clear check.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPResultsMenu), "Start")]
        static void SendStageClearLocationCheck(ref int ___challengeID)
        {
            // Check if the stage we've just cleared is Weapon's Core.
            if (FPStage.currentStage.stageID == 30)
            {
                // Set up a status update with the goal flag.
                StatusUpdatePacket statusUpdatePacket = new() { Status = ArchipelagoClientState.ClientGoal };

                // Send the goal flag.
                Plugin.Session.Socket.SendPacket(statusUpdatePacket);

                // Don't bother running the rest of this function.
                return;
            }

            // Set up a location.
            Location location = null;

            // Determine the location from the completed stage by looking up a value in the Locations array.
            switch (FPStage.currentStage.stageID)
            {
                case 1:  location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Dragon Valley - Clear");       break;
                case 2:  location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shenlin Park - Clear");        break;
                case 3:  location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Avian Museum - Clear");        break;
                case 4:  location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Airship Sigwada - Clear");     break;
                case 5:  location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Tiger Falls - Clear");         break;
                case 6:  location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Robot Graveyard - Clear");     break;
                case 7:  location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shade Armory - Clear");        break;
                case 8:  location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Snowfields - Clear");          break;
                case 9:  location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Phoenix Highway - Clear");     break;
                case 10: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Zao Land - Clear");            break;
                case 11: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Globe Opera 1 - Clear");       break;
                case 12: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Globe Opera 2 - Clear");       break;
                case 13: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Auditorium - Clear");          break;
                case 14: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Palace Courtyard - Clear");    break;
                case 15: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Tidal Gate - Clear");          break;
                case 16: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Zulon Jungle - Clear");        break;
                case 17: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Nalao Lake - Clear");          break;
                case 18: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Sky Bridge - Clear");          break;
                case 19: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Lightning Tower - Clear");     break;
                case 20: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Ancestral Forge - Clear");     break;
                case 21: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Magma Starscape - Clear");     break;
                case 22: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Diamond Point - Clear");       break;
                case 23: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Gravity Bubble - Clear");      break;
                case 24: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Bakunawa Rush - Clear");       break;
                case 25: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Refinery Room - Clear");       break;
                case 26: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Clockwork Arboretum - Clear"); break;
                case 27: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Inversion Dynamo - Clear");    break;
                case 28: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Lunar Cannon - Clear");        break;
                case 29: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Merga - Clear");               break;
                case 32: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Bakunawa Chase - Clear");      break;

                // Handle The Battlesphere differently due to its challenge ID system.
                case 31:
                    switch (___challengeID)
                    {
                        case 4:  location = Array.Find(Plugin.APSave.Locations, location => location.Name == "The Battlesphere - Clear");           break;
                        case 6:  location = Array.Find(Plugin.APSave.Locations, location => location.Name == "The Battlesphere - Time Capsule 1");  break;
                        case 7:  location = Array.Find(Plugin.APSave.Locations, location => location.Name == "The Battlesphere - Time Capsule 2");  break;
                        case 8:  location = Array.Find(Plugin.APSave.Locations, location => location.Name == "The Battlesphere - Time Capsule 3");  break;
                        case 9:  location = Array.Find(Plugin.APSave.Locations, location => location.Name == "The Battlesphere - Time Capsule 4");  break;
                        case 10: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "The Battlesphere - Time Capsule 5");  break;
                        case 11: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "The Battlesphere - Time Capsule 6");  break;
                        case 12: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "The Battlesphere - Time Capsule 7");  break;
                        case 13: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "The Battlesphere - Time Capsule 8");  break;
                        case 14: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "The Battlesphere - Time Capsule 9");  break;
                        case 15: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "The Battlesphere - Time Capsule 10"); break;
                        case 16: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "The Battlesphere - Time Capsule 11"); break;
                        case 17: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "The Battlesphere - Time Capsule 12"); break;
                    }
                    break;

                // Panic if we haven't handled this stage ID.
                default: Console.WriteLine($"Stage ID {FPStage.currentStage.stageID} not handled!"); break;
            }

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
