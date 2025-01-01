namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuArenaChallengeSelectPatcher
    {
        /// <summary>
        /// Replaces the reward sprite for a challenge in the menu with the AP logo and handles applying the checkmark.
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
                // Set up a new texture.
                Texture2D texture = new(32, 32);

                // Change the texture to use point filtering.
                texture.filterMode = FilterMode.Point;

                // Load the Archipelago logo.
                texture.LoadImage(Plugin.APLogo);

                // Set the sprite of the reward item to our AP logo.
                ___rewardItem.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1);
            }
        }
    }
}
