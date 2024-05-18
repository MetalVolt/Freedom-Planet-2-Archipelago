namespace FP2Archipelago.Patchers
{
    internal class ItemGetMenu
    {
        /// <summary>
        /// Sends out a location check when buying a shop item.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuItemGet), "Start")]
        static void SendShopLocationCheck()
        {
            // Set up a value to store the location ID for the purchased item.
            long? locationID = null;

            // Find the item get menu.
            MenuItemGet itemGetMenu = UnityEngine.Object.FindObjectOfType<MenuItemGet>();

            // Check if we've actually found the item get menu.
            if (itemGetMenu != null)
            {
                // If this item get menu is referencing a power up, then get the location from it.
                if (itemGetMenu.powerup != FPPowerup.NONE)
                {
                    switch (itemGetMenu.powerup)
                    {
                        case FPPowerup.ELEMENT_BURST:   locationID = GlobalValues.Locations["Shop - Element Burst"];      break;
                        case FPPowerup.MORE_PETALS:     locationID = GlobalValues.Locations["Shop - Crystals To Petals"]; break;
                        case FPPowerup.PETAL_ARMOR:     locationID = GlobalValues.Locations["Shop - Petal Armor"];        break;
                        case FPPowerup.EXTRA_STOCK:     locationID = GlobalValues.Locations["Shop - Extra Stock"];        break;
                        case FPPowerup.STRONG_REVIVALS: locationID = GlobalValues.Locations["Shop - Strong Revivals"];    break;
                        case FPPowerup.CHEAPER_STOCKS:  locationID = GlobalValues.Locations["Shop - Cheaper Stocks"];     break;
                        case FPPowerup.REGENERATION:    locationID = GlobalValues.Locations["Shop - Healing Strike"];     break;
                        case FPPowerup.ATTACK_UP:       locationID = GlobalValues.Locations["Shop - Attack Up"];          break;
                        case FPPowerup.STRONG_SHIELDS:  locationID = GlobalValues.Locations["Shop - Strong Shields"];     break;
                        case FPPowerup.SPEED_UP:        locationID = GlobalValues.Locations["Shop - Accelerator"];        break;
                        case FPPowerup.JUMP_UP:         locationID = GlobalValues.Locations["Shop - Super Feather"];      break;
                        case FPPowerup.MAX_LIFE_UP:     locationID = GlobalValues.Locations["Shop - Max Life Up"];        break;
                        case FPPowerup.ONE_HIT_KO:      locationID = GlobalValues.Locations["Shop - One Hit KO"];         break;
                        case FPPowerup.BIPOLAR_LIFE:    locationID = GlobalValues.Locations["Shop - Life Oscillation"];   break;
                        case FPPowerup.ITEMS_TO_BOMBS:  locationID = GlobalValues.Locations["Shop - Items To Bombs"];     break;
                        case FPPowerup.POWERUP_START:   locationID = GlobalValues.Locations["Shop - Powerup Start"];      break;
                        case FPPowerup.SHADOW_GUARD:    locationID = GlobalValues.Locations["Shop - Shadow Guard"];       break;
                        case FPPowerup.PAYBACK_RING:    locationID = GlobalValues.Locations["Shop - Payback Ring"];       break;
                        case FPPowerup.WOOD_CHARM:      locationID = GlobalValues.Locations["Shop - Wood Charm"];         break;
                        case FPPowerup.EARTH_CHARM:     locationID = GlobalValues.Locations["Shop - Earth Charm"];        break;
                        case FPPowerup.WATER_CHARM:     locationID = GlobalValues.Locations["Shop - Water Charm"];        break;
                        case FPPowerup.FIRE_CHARM:      locationID = GlobalValues.Locations["Shop - Fire Charm"];         break;
                        case FPPowerup.METAL_CHARM:     locationID = GlobalValues.Locations["Shop - Metal Charm"];        break;
                        case FPPowerup.RAINBOW_CHARM:   locationID = GlobalValues.Locations["Shop - Rainbow Charm"];      break;
                        default:                        Console.WriteLine($"Item ID {itemGetMenu.powerup} not handled!"); break;
                    }
                }
                
                // If not, then get the location from its music ID instead.
                else
                {
                    switch (itemGetMenu.musicID)
                    {
                        case 1:  locationID = GlobalValues.Locations["Shop - Vinyl - Title Screen"];            break;
                        case 2:  locationID = GlobalValues.Locations["Shop - Vinyl - Main Menu"];               break;
                        case 13: locationID = GlobalValues.Locations["Shop - Vinyl - Globe Opera 2A"];          break;
                        case 28: locationID = GlobalValues.Locations["Shop - Vinyl - Weapon's Core"];           break;
                        case 29: locationID = GlobalValues.Locations["Shop - Vinyl - Boss - Robot A"];          break;
                        case 30: locationID = GlobalValues.Locations["Shop - Vinyl - Boss - Robot B"];          break;
                        case 31: locationID = GlobalValues.Locations["Shop - Vinyl - Boss - Aaa"];              break;
                        case 32: locationID = GlobalValues.Locations["Shop - Vinyl - Boss - Phoenix Highway"];  break;
                        case 33: locationID = GlobalValues.Locations["Shop - Vinyl - Boss - Zao Land"];         break;
                        case 34: locationID = GlobalValues.Locations["Shop - Vinyl - Boss - Arena"];            break;
                        case 35: locationID = GlobalValues.Locations["Shop - Vinyl - Boss - Captain Kalaw"];    break;
                        case 36: locationID = GlobalValues.Locations["Shop - Vinyl - Boss - Serpentine A"];     break;
                        case 37: locationID = GlobalValues.Locations["Shop - Vinyl - Boss - Serpentine B"];     break;
                        case 38: locationID = GlobalValues.Locations["Shop - Vinyl - Boss - Beast One/Two"];    break;
                        case 39: locationID = GlobalValues.Locations["Shop - Vinyl - Boss - Beast Three"];      break;
                        case 40: locationID = GlobalValues.Locations["Shop - Vinyl - Boss - BFF2000"];          break;
                        case 41: locationID = GlobalValues.Locations["Shop - Vinyl - Boss - Diamond Point"];    break;
                        case 42: locationID = GlobalValues.Locations["Shop - Vinyl - Boss - Arboretum"];        break;
                        case 43: locationID = GlobalValues.Locations["Shop - Vinyl - Boss - Merga"];            break;
                        case 44: locationID = GlobalValues.Locations["Shop - Vinyl - Boss - Weapon's Core"];    break;
                        case 45: locationID = GlobalValues.Locations["Shop - Vinyl - Stage Clear"];             break;
                        case 46: locationID = GlobalValues.Locations["Shop - Vinyl - Results - Lilac"];         break;
                        case 47: locationID = GlobalValues.Locations["Shop - Vinyl - Results - Carol"];         break;
                        case 48: locationID = GlobalValues.Locations["Shop - Vinyl - Results - Milla"];         break;
                        case 49: locationID = GlobalValues.Locations["Shop - Vinyl - Results - Neera"];         break;
                        case 50: locationID = GlobalValues.Locations["Shop - Vinyl - Bonus Stage"];             break;
                        case 51: locationID = GlobalValues.Locations["Shop - Vinyl - Speed Gate"];              break;
                        case 52: locationID = GlobalValues.Locations["Shop - Vinyl - Shopping"];                break;
                        case 53: locationID = GlobalValues.Locations["Shop - Vinyl - Map - Shang Tu"];          break;
                        case 54: locationID = GlobalValues.Locations["Shop - Vinyl - Map - Shang Mu"];          break;
                        case 55: locationID = GlobalValues.Locations["Shop - Vinyl - Map - Shuigang"];          break;
                        case 56: locationID = GlobalValues.Locations["Shop - Vinyl - Map - Opera"];             break;
                        case 57: locationID = GlobalValues.Locations["Shop - Vinyl - Map - Parusa"];            break;
                        case 58: locationID = GlobalValues.Locations["Shop - Vinyl - Map - Floating Island"];   break;
                        case 59: locationID = GlobalValues.Locations["Shop - Vinyl - Map - Volcano"];           break;
                        case 60: locationID = GlobalValues.Locations["Shop - Vinyl - Map - Bakunawa"];          break;
                        case 61: locationID = GlobalValues.Locations["Shop - Vinyl - Singing Water Temple"];    break;
                        case 62: locationID = GlobalValues.Locations["Shop - Vinyl - Royal Palace"];            break;
                        case 63: locationID = GlobalValues.Locations["Shop - Vinyl - Battlesphere Commercial"]; break;
                        case 64: locationID = GlobalValues.Locations["Shop - Vinyl - Battlesphere Lobby"];      break;
                        case 65: locationID = GlobalValues.Locations["Shop - Vinyl - Battlesphere Course"];     break;
                        case 66: locationID = GlobalValues.Locations["Shop - Vinyl - Captain Kalaw's Theme"];   break;
                        case 67: locationID = GlobalValues.Locations["Shop - Vinyl - Shuigang"];                break;
                        case 68: locationID = GlobalValues.Locations["Shop - Vinyl - Adventure Square"];        break;
                        case 69: locationID = GlobalValues.Locations["Shop - Vinyl - Paradise Prime"];          break;
                        case 70: locationID = GlobalValues.Locations["Shop - Vinyl - Cutscene - Generic"];      break;
                        case 71: locationID = GlobalValues.Locations["Shop - Vinyl - Cutscene - Call to Arms"]; break;
                        case 72: locationID = GlobalValues.Locations["Shop - Vinyl - Cutscene - Big Mood A"];   break;
                        case 73: locationID = GlobalValues.Locations["Shop - Vinyl - Cutscene - Big Mood B"];   break;
                        case 74: locationID = GlobalValues.Locations["Shop - Vinyl - Lilac's Theme"];           break;
                        case 75: locationID = GlobalValues.Locations["Shop - Vinyl - Carol's Theme"];           break;
                        case 76: locationID = GlobalValues.Locations["Shop - Vinyl - Milla's Theme"];           break;
                        case 77: locationID = GlobalValues.Locations["Shop - Vinyl - Neera's Theme"];           break;
                        case 78: locationID = GlobalValues.Locations["Shop - Vinyl - Corazon's Theme"];         break;
                        case 79: locationID = GlobalValues.Locations["Shop - Vinyl - Merga's Theme"];           break;
                        case 80: locationID = GlobalValues.Locations["Shop - Vinyl - Audio Log A"];             break;
                        case 81: locationID = GlobalValues.Locations["Shop - Vinyl - Audio Log B"];             break;
                        case 82: locationID = GlobalValues.Locations["Shop - Vinyl - Audio Log C"];             break;
                        case 83: locationID = GlobalValues.Locations["Shop - Vinyl - Basic Tutorial"];          break;
                        case 84: locationID = GlobalValues.Locations["Shop - Vinyl - Boss - Merga (Pinch)"];    break;
                        case 85: locationID = GlobalValues.Locations["Shop - Vinyl - Cutscene - Heroic"];       break;
                        case 86: locationID = GlobalValues.Locations["Shop - Vinyl - Cutscene - Preparation"];  break;
                        case 87: locationID = GlobalValues.Locations["Shop - Vinyl - Cutscene - Bakunawa"];     break;
                        case 88: locationID = GlobalValues.Locations["Shop - Vinyl - Gallery"];                 break;
                        case 89: locationID = GlobalValues.Locations["Shop - Vinyl - Serpentine's Theme"];      break;
                        case 90: locationID = GlobalValues.Locations["Shop - Vinyl - City Hall"];               break;
                        default: Console.WriteLine($"Vinyl ID {itemGetMenu.musicID} not handled!");             break;
                    }
                }
            }

            // If we've read a location ID, then send the item at it and mark it as checked locally.
            if (locationID != null)
            {
                GlobalValues.Session.Locations.CompleteLocationChecks((long)locationID);

                if (itemGetMenu.powerup != FPPowerup.NONE)
                    GlobalValues.MillaShopItems.Find(i => i.Location == GlobalValues.Session.Locations.GetLocationNameFromId((long)locationID)).Checked = true;
                else
                    GlobalValues.VinylShopItems.Find(i => i.Location == GlobalValues.Session.Locations.GetLocationNameFromId((long)locationID)).Checked = true;
            }
        }
    }
}
