using System.Reflection;
using UnityEngine;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuItemSelectPatcher
    {
        /// <summary>
        /// Creates the Item Select Menu, reconstructed from the original code to remove a check that ruins everything.
        /// TODO: This feels stupid.
        /// TODO: Interacting with the menu when there's no unlocked items breaks.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuItemSelect), "Start")]
        static bool StartItemMenu(ref bool[] ___potions, ref bool[] ___amulets, ref int ___ps, ref int ___buttonCount, ref MenuOption[] ___menuOptions,
                                  ref SpriteRenderer[] ___menuButtons, ref FPHudDigit[] ___equipIcons, ref GameObject[] ___equipSlots,
                                  ref GameObject ___pfEquipSlot, ref GameObject ___itemWindow, ref GameObject ___bottle, ref FPHudDigit ___pfPowerupIcon,
                                  ref Vector2 ___itemSlotSpacing, ref SpriteRenderer ___itemFolder, ref FPPowerup[] ___amuletList)
        {
            // Get a reference to this menu.
            MenuItemSelect menu = UnityEngine.Object.FindObjectOfType<MenuItemSelect>();

            // Set the potions and brave stone unlocked lists.
            ___potions = Plugin.APSave.UnlockedPotions;
            ___amulets = Plugin.APSave.UnlockedBraveStones;

            // Get the potion count.
            ___ps = FPSaveManager.GetPotionSlots();

            // Get the button count.
            ___buttonCount = ___menuOptions.Length;

            // Set up the menu buttons array.
            ___menuButtons = new SpriteRenderer[___buttonCount];

            // Get each option's associated button.
            for (int buttonIndex = 0; buttonIndex < ___buttonCount; buttonIndex++)
                ___menuButtons[buttonIndex] = ___menuOptions[buttonIndex].GetComponent<SpriteRenderer>();

            // Set up the equip icons and slots.
            ___equipIcons = new FPHudDigit[FPSaveManager.GetItemSlots()];
            ___equipSlots = new GameObject[FPSaveManager.GetItemSlots()];

            // Loop through each slot.
            for (int slotIndex = 0; slotIndex < ___equipSlots.Length; slotIndex++)
            {
                // Create this slot.
                ___equipSlots[slotIndex] = UnityEngine.Object.Instantiate(___pfEquipSlot);

                // Parent this slot to the item window.
                ___equipSlots[slotIndex].transform.parent = ___itemWindow.transform;

                // Set this slot's local position based on the bottle's.
                ___equipSlots[slotIndex].transform.localPosition = ___bottle.transform.localPosition;

                // Create this icon.
                ___equipIcons[slotIndex] = UnityEngine.Object.Instantiate(___pfPowerupIcon);

                // Set this icon's value to the approriate power up's.
                ___equipIcons[slotIndex].SetDigitValue((int)FPSaveManager.itemSets[FPSaveManager.activeItemSet].powerups[slotIndex]);

                // Parent this icon to the slot.
                ___equipIcons[slotIndex].transform.parent = ___equipSlots[slotIndex].transform;

                // Move this icon back on the Z axis a bit?
                ___equipIcons[slotIndex].transform.localPosition = new Vector3(0f, 0f, -1f);

                // Increment the bottle's local position.
                ___bottle.transform.localPosition += new Vector3(___itemSlotSpacing.x, ___itemSlotSpacing.y, 0f);
            }

            // Increment the bottle's local position.
            ___bottle.transform.localPosition += new Vector3(___itemSlotSpacing.x * 0.5f, ___itemSlotSpacing.y * 0.5f, 0f);

            // Parent the item folder to the bottle.
            ___itemFolder.transform.position = ___bottle.transform.position;

            // Get and run the Draw Item Slots function.
            MethodInfo function = typeof(MenuItemSelect).GetMethod("DrawItemSlots", BindingFlags.NonPublic | BindingFlags.Instance);
            function.Invoke(menu, new object[] { ___amuletList, Plugin.APSave.UnlockedBraveStones });

            // Get and run the Draw Potion function.
            function = typeof(MenuItemSelect).GetMethod("DrawPotion", BindingFlags.NonPublic | BindingFlags.Instance);
            function.Invoke(menu, new object[] { });

            // Get and run the Update Folders function twice. The original code does this for some reason, does it HAVE to be run twice to function?
            function = typeof(MenuItemSelect).GetMethod("UpdateFolders", BindingFlags.NonPublic | BindingFlags.Instance);
            function.Invoke(menu, new object[] { });
            function.Invoke(menu, new object[] { });

            // Get and run the Go To Options function to force the state to change.
            function = typeof(MenuItemSelect).GetMethod("GoTo_Options", BindingFlags.NonPublic | BindingFlags.Instance);
            function.Invoke(menu, new object[] { });

            // Block the original function from running.
            return false;
        }
    }
}
