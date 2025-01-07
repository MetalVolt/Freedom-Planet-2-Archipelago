namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuArenaChallengeSelectPatcher
    {
        /// <summary>
        /// Unlocks challenges based on the number of Battlesphere Keys received.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuArenaChallengeSelect), "Start")]
        static void ChallengeUnlocking(ref int[] ___challengeRewards, ref int[] ___challengeUnlockRequirement)
        {
            // Replace the contents of challengeRewards with a version that throws out the last two challenges, as we never use them.
            int[] trimmedRewards = new int[18];
            for (int rewardIndex = 0; rewardIndex < trimmedRewards.Length; rewardIndex++)
                trimmedRewards[rewardIndex] = ___challengeRewards[rewardIndex];
            ___challengeRewards = trimmedRewards;

            // Set each challenge unlock requirement to 39.
            for (int challengeIndex = 0; challengeIndex < ___challengeUnlockRequirement.Length; challengeIndex++) ___challengeUnlockRequirement[challengeIndex] = 39;

            // Loop through and unlock the right number of challenges based on our Battlesphere Key count.
            for (int keyIndex = 0; keyIndex < Plugin.APSave.BattlesphereKeyCount; keyIndex++) ___challengeUnlockRequirement[keyIndex] = -1;
        }
        
        /// <summary>
        /// Updates the number of unlocked challenges on the main arena menu.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuArena), "Update")]
        static void ChallengeUnlockingCount(ref int ___unlockedArenas) => ___unlockedArenas = Plugin.APSave.BattlesphereKeyCount;

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
