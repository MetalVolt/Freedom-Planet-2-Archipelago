using UnityEngine;
using UnityEngine.SceneManagement;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class ItemChestPatcher
    {
        /// <summary>
        /// Stupid hack used to find and steal an initialised ItemLabel from a Dragon Valley chest.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemChest), "Start")]
        static void FetchItemLabel(ref ItemLabel ___label)
        {
            // Check that this chest has an initialised label and that our plugin doesn't.
            if (___label != null && Plugin.ItemLabelTemplate == null)
            {
                // Copy this chest's label onto the plugin one.
                Plugin.ItemLabelTemplate = ___label;

                // Boot the player out to the classic menu.
                SceneManager.LoadScene("ClassicMenu");
            }
        }

        /// <summary>
        /// Gets the location at a chest.
        /// </summary>
        /// <param name="contents">The chest contents (either a Brave Stone or Vinyl).</param>
        /// <param name="powerupType">The Brave Stone in this chest.</param>
        /// <param name="musicID">The ID of the Vinyl in this chest.</param>
        /// <returns>The location for this chest.</returns>
        private static Location GetLocationAtChest(FPItemChestContent contents, FPPowerup powerupType, FPMusicTrack musicID)
        {
            // Check if this chest is a power up and get the location from it.
            if (contents == FPItemChestContent.POWERUP)
            {
                switch (powerupType)
                {
                    case FPPowerup.TIME_LIMIT:       return Array.Find(Plugin.APSave.Locations, location => location.Name == "Dragon Valley - Brave Stone");  
                    case FPPowerup.STOCK_DRAIN:      return Array.Find(Plugin.APSave.Locations, location => location.Name == "Shenlin Park - Brave Stone");   
                    case FPPowerup.NO_GUARDING:      return Array.Find(Plugin.APSave.Locations, location => location.Name == "Tiger Falls - Brave Stone");    
                    case FPPowerup.NO_PETALS:        return Array.Find(Plugin.APSave.Locations, location => location.Name == "Avian Museum - Brave Stone");   
                    case FPPowerup.DOUBLE_DAMAGE:    return Array.Find(Plugin.APSave.Locations, location => location.Name == "Airship Sigwada - Brave Stone");
                    case FPPowerup.NO_REVIVALS:      return Array.Find(Plugin.APSave.Locations, location => location.Name == "Phoenix Highway - Brave Stone");
                    case FPPowerup.EXPENSIVE_STOCKS: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Zao Land - Brave Stone");       
                }
            }

            // Check if this chest is a vinyl and get the location from it.
            if (contents == FPItemChestContent.MUSIC)
            {
                switch (musicID)
                {
                    case FPMusicTrack.STAGE_VALLEY:    return Array.Find(Plugin.APSave.Locations, location => location.Name == "Dragon Valley - Vinyl");       
                    case FPMusicTrack.STAGE_PARK:      return Array.Find(Plugin.APSave.Locations, location => location.Name == "Shenlin Park - Vinyl");        
                    case FPMusicTrack.STAGE_FALLS:     return Array.Find(Plugin.APSave.Locations, location => location.Name == "Tiger Falls - Vinyl");         
                    case FPMusicTrack.STAGE_GRAVEYARD: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Robot Graveyard - Vinyl");     
                    case FPMusicTrack.STAGE_ARMORY:    return Array.Find(Plugin.APSave.Locations, location => location.Name == "Shade Armory - Vinyl");        
                    case FPMusicTrack.STAGE_MUSEUM:    return Array.Find(Plugin.APSave.Locations, location => location.Name == "Avian Museum - Vinyl");        
                    case FPMusicTrack.STAGE_SIGWADA:   return Array.Find(Plugin.APSave.Locations, location => location.Name == "Airship Sigwada - Vinyl");     
                    case FPMusicTrack.STAGE_HIGHWAY:   return Array.Find(Plugin.APSave.Locations, location => location.Name == "Phoenix Highway - Vinyl");     
                    case FPMusicTrack.STAGE_ZAOLAND:   return Array.Find(Plugin.APSave.Locations, location => location.Name == "Zao Land - Vinyl");            
                    case FPMusicTrack.STAGE_OPERA1:    return Array.Find(Plugin.APSave.Locations, location => location.Name == "Globe Opera 1 - Vinyl");       
                    case FPMusicTrack.STAGE_OPERA2B:   return Array.Find(Plugin.APSave.Locations, location => location.Name == "Globe Opera 2 - Vinyl");       
                    case FPMusicTrack.STAGE_COURTYARD: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Palace Courtyard - Vinyl");    
                    case FPMusicTrack.STAGE_GATE:      return Array.Find(Plugin.APSave.Locations, location => location.Name == "Tidal Gate - Vinyl");          
                    case FPMusicTrack.STAGE_BRIDGE:    return Array.Find(Plugin.APSave.Locations, location => location.Name == "Sky Bridge - Vinyl");          
                    case FPMusicTrack.STAGE_TOWER:     return Array.Find(Plugin.APSave.Locations, location => location.Name == "Lightning Tower - Vinyl");     
                    case FPMusicTrack.STAGE_JUNGLE:    return Array.Find(Plugin.APSave.Locations, location => location.Name == "Zulon Jungle - Vinyl");        
                    case FPMusicTrack.STAGE_LAKE:      return Array.Find(Plugin.APSave.Locations, location => location.Name == "Nalao Lake - Vinyl");          
                    case FPMusicTrack.STAGE_FORGE:     return Array.Find(Plugin.APSave.Locations, location => location.Name == "Ancestral Forge - Vinyl");     
                    case FPMusicTrack.STAGE_STARSCAPE: return Array.Find(Plugin.APSave.Locations, location => location.Name == "Magma Starscape - Vinyl");     
                    case FPMusicTrack.STAGE_BUBBLE:    return Array.Find(Plugin.APSave.Locations, location => location.Name == "Gravity Bubble - Vinyl");      
                    case FPMusicTrack.STAGE_FINALE1:   return Array.Find(Plugin.APSave.Locations, location => location.Name == "Bakunawa Rush - Vinyl");       
                    case FPMusicTrack.STAGE_FINALE2:   return Array.Find(Plugin.APSave.Locations, location => location.Name == "Clockwork Arboretum - Vinyl"); 
                    case FPMusicTrack.STAGE_FINALE3:   return Array.Find(Plugin.APSave.Locations, location => location.Name == "Inversion Dynamo - Vinyl");    
                    case FPMusicTrack.STAGE_FINALE4:   return Array.Find(Plugin.APSave.Locations, location => location.Name == "Lunar Cannon - Vinyl");        
                }
            }

            // If we haven't found anything, return null;
            return null;
        }

        /// <summary>
        /// Sets the sprite and message for the item displayed when opening the chest.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemChest), "Start")]
        static void SetSpriteAndMessage(ref FPItemChestContent ___contents, ref FPPowerup ___powerupType, ref FPMusicTrack ___musicID,
                                        ref Sprite ___itemSprite, ref string ___labelMessage)
        {
            // Read the location at this chest (if there is one).
            Location location = GetLocationAtChest(___contents, ___powerupType, ___musicID);

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
        /// Sends a location out from opening a Vinyl/Brave Stone chest.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemChest), "BoxHit")]
        static void SendChestLocationCheck(ref FPItemChestContent ___contents, ref FPPowerup ___powerupType, ref FPMusicTrack ___musicID)
        {
            // Read the location at this chest (if there is one).
            Location location = GetLocationAtChest(___contents, ___powerupType, ___musicID);

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
