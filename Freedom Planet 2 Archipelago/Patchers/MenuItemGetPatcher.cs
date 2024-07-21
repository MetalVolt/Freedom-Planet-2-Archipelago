namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuItemGetPatcher
    {
        /// <summary>
        /// Sends out a location check when buying a shop item.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuItemGet), "Start")]
        static void SendShopLocationCheck()
        {
            // Set up a location.
            Location location = null;

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
                        case FPPowerup.ELEMENT_BURST:   location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Element Burst");      break;
                        case FPPowerup.MORE_PETALS:     location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Crystals To Petals"); break;
                        case FPPowerup.PETAL_ARMOR:     location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Petal Armor");        break;
                        case FPPowerup.EXTRA_STOCK:     location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Extra Stock");        break;
                        case FPPowerup.STRONG_REVIVALS: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Strong Revivals");    break;
                        case FPPowerup.CHEAPER_STOCKS:  location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Cheaper Stocks");     break;
                        case FPPowerup.REGENERATION:    location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Healing Strike");     break;
                        case FPPowerup.ATTACK_UP:       location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Attack Up");          break;
                        case FPPowerup.STRONG_SHIELDS:  location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Strong Shields");     break;
                        case FPPowerup.SPEED_UP:        location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Accelerator");        break;
                        case FPPowerup.JUMP_UP:         location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Super Feather");      break;
                        case FPPowerup.MAX_LIFE_UP:     location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Max Life Up");        break;
                        case FPPowerup.ONE_HIT_KO:      location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - One Hit KO");         break;
                        case FPPowerup.BIPOLAR_LIFE:    location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Life Oscillation");   break;
                        case FPPowerup.ITEMS_TO_BOMBS:  location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Items To Bombs");     break;
                        case FPPowerup.POWERUP_START:   location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Powerup Start");      break;
                        case FPPowerup.SHADOW_GUARD:    location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Shadow Guard");       break;
                        case FPPowerup.PAYBACK_RING:    location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Payback Ring");       break;
                        case FPPowerup.WOOD_CHARM:      location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Wood Charm");         break;
                        case FPPowerup.EARTH_CHARM:     location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Earth Charm");        break;
                        case FPPowerup.WATER_CHARM:     location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Water Charm");        break;
                        case FPPowerup.FIRE_CHARM:      location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Fire Charm");         break;
                        case FPPowerup.METAL_CHARM:     location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Metal Charm");        break;
                        case FPPowerup.RAINBOW_CHARM:   location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Rainbow Charm");      break;
                        default:                        Console.WriteLine($"Item ID {itemGetMenu.powerup} not handled!"); break;
                    }
                }
                
                // If not, then get the location from its music ID instead.
                else
                {
                    switch (itemGetMenu.musicID)
                    {
                        case 1:  location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Title Screen");            break;
                        case 2:  location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Main Menu");               break;
                        case 13: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Globe Opera 2A");          break;
                        case 28: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Weapon's Core");           break;
                        case 29: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Boss - Robot A");          break;
                        case 30: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Boss - Robot B");          break;
                        case 31: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Boss - Aaa");              break;
                        case 32: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Boss - Phoenix Highway");  break;
                        case 33: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Boss - Zao Land");         break;
                        case 34: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Boss - Arena");            break;
                        case 35: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Boss - Captain Kalaw");    break;
                        case 36: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Boss - Serpentine A");     break;
                        case 37: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Boss - Serpentine B");     break;
                        case 38: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Boss - Beast One/Two");    break;
                        case 39: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Boss - Beast Three");      break;
                        case 40: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Boss - BFF2000");          break;
                        case 41: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Boss - Diamond Point");    break;
                        case 42: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Boss - Arboretum");        break;
                        case 43: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Boss - Merga");            break;
                        case 44: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Boss - Weapon's Core");    break;
                        case 45: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Stage Clear");             break;
                        case 46: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Results - Lilac");         break;
                        case 47: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Results - Carol");         break;
                        case 48: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Results - Milla");         break;
                        case 49: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Results - Neera");         break;
                        case 50: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Bonus Stage");             break;
                        case 51: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Speed Gate");              break;
                        case 52: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Shopping");                break;
                        case 53: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Map - Shang Tu");          break;
                        case 54: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Map - Shang Mu");          break;
                        case 55: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Map - Shuigang");          break;
                        case 56: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Map - Opera");             break;
                        case 57: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Map - Parusa");            break;
                        case 58: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Map - Floating Island");   break;
                        case 59: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Map - Volcano");           break;
                        case 60: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Map - Bakunawa");          break;
                        case 61: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Singing Water Temple");    break;
                        case 62: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Royal Palace");            break;
                        case 63: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Battlesphere Commercial"); break;
                        case 64: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Battlesphere Lobby");      break;
                        case 65: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Battlesphere Course");     break;
                        case 66: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Captain Kalaw's Theme");   break;
                        case 67: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Shuigang");                break;
                        case 68: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Adventure Square");        break;
                        case 69: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Paradise Prime");          break;
                        case 70: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Cutscene - Generic");      break;
                        case 71: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Cutscene - Call to Arms"); break;
                        case 72: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Cutscene - Big Mood A");   break;
                        case 73: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Cutscene - Big Mood B");   break;
                        case 74: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Lilac's Theme");           break;
                        case 75: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Carol's Theme");           break;
                        case 76: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Milla's Theme");           break;
                        case 77: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Neera's Theme");           break;
                        case 78: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Corazon's Theme");         break;
                        case 79: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Merga's Theme");           break;
                        case 80: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Audio Log A");             break;
                        case 81: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Audio Log B");             break;
                        case 82: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Audio Log C");             break;
                        case 83: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Basic Tutorial");          break;
                        case 84: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Boss - Merga (Pinch)");    break;
                        case 85: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Cutscene - Heroic");       break;
                        case 86: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Cutscene - Preparation");  break;
                        case 87: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Cutscene - Bakunawa");     break;
                        case 88: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Gallery");                 break;
                        case 89: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - Serpentine's Theme");      break;
                        case 90: location = Array.Find(Plugin.APSave.Locations, location => location.Name == "Shop - Vinyl - City Hall");               break;
                        default: Console.WriteLine($"Vinyl ID {itemGetMenu.musicID} not handled!");             break;
                    }
                }
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
