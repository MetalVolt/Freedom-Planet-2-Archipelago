namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuArenaChallengeSelectPatcher
    {
        /// <summary>
        /// Replaces the reward sprite for a challenge in the menu with the AP logo and handles applying the checkmark.
        /// TODO: Hide the sprite if the challenge is locked?
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuArenaChallengeSelect), "State_Challenge")]
        static void HandleAPIcon(ref SpriteRenderer ___rewardItem, ref int ___challengeSelection, ref int ___challengeIDOffset, ref int[] ___slotID, ref GameObject ___rewardCheckmark)
        {
            // Activate the checkmark depending on if a time is recorded for the selected challenge or not.
            if (FPSaveManager.challengeRecord[___slotID[___challengeSelection] + ___challengeIDOffset] > 0)
                ___rewardCheckmark.SetActive(true);
            else
                ___rewardCheckmark.SetActive(false);

            // Check if we have a reward item sprite renderer and that we're not selecting either of the last two challenges.
            if (___rewardItem != null && ___challengeSelection <= 17)
            {
                // Get the location for this challenge.
                int challengeID = ___challengeSelection + 1;
                Location location = Array.Find(Plugin.APSave.Locations, location => location.Name == $"The Battlesphere - Challenge {challengeID}");

                // If we've found a location for this challenge, then set the sprite, respecting the shop_information slot data setting.
                if (location != null)
                    ___rewardItem.sprite = Plugin.GetItemSprite(location, true);
            }
        }
    }
}
