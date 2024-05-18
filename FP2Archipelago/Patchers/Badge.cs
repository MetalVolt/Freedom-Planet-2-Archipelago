using System.Linq;

namespace FP2Archipelago.Patchers
{
    internal class Badge
    {
        /// <summary>
        /// Edits the message on a badge with an ID of 19 to show the last recieved item or 59 to show the cause for a DeathLink.
        /// TODO: Figure out why killing the header just isn't working despite using the same code as the Rando'.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BadgeMessage), "Update")]
        static void EditBadgeNotification()
        {
            // Find the badge message.
            BadgeMessage badge = UnityEngine.Object.FindObjectOfType<BadgeMessage>();

            // Check we've actually found the badge message.
            if (badge != null)
            {
                // Check if the badge ID is set to 19.
                if (badge.id == 19)
                {
                    // Set the badge's X offset to the center of the screen.
                    badge.xOffset = 320;

                    // Disable the badge's icon sprite.
                    badge.badgeIcon.enabled = false;

                    // Determine who sent the recieved item.
                    string sourcePlayer = GlobalValues.Session.Players.GetPlayerName(GlobalValues.Session.Items.AllItemsReceived.Last().Player);

                    // Set the message depending on who sent the recieved item.
                    if (sourcePlayer != GlobalValues.Session.Players.GetPlayerName(GlobalValues.Session.ConnectionInfo.Slot))
                        badge.GetComponent<TextMesh>().text = $"Recieved {GlobalValues.Session.Items.GetItemName(GlobalValues.Session.Items.AllItemsReceived.Last().Item)} from {sourcePlayer}.";
                    else
                        badge.GetComponent<TextMesh>().text = $"Found {GlobalValues.Session.Items.GetItemName(GlobalValues.Session.Items.AllItemsReceived.Last().Item)}.";

                    // Find the badge text box.
                    GameObject badgeHighlight = GameObject.Find("hud_pause_textboxlarge");

                    // Check that we've actually found the badge text box.
                    if (badgeHighlight != null)
                    {
                        // Remove its X offset to center it.
                        badgeHighlight.transform.localPosition = new(0, badgeHighlight.transform.localPosition.y, badgeHighlight.transform.localPosition.z);

                        // Scale it by 1.5 times on the X axis.
                        badgeHighlight.transform.localScale = new(1.5f, badgeHighlight.transform.localScale.y, badgeHighlight.transform.localScale.z);
                    }
                }

                // Check if the badge ID is set to 59.
                if (badge.id == 59)
                {
                    // Set the badge's X offset to the center of the screen.
                    badge.xOffset = 320;

                    // Disable the badge's icon sprite.
                    badge.badgeIcon.enabled = false;

                    // Change the badge message, depending on if we have a death cause or not.
                    if (GlobalValues.LastDLCause != "")
                        badge.GetComponent<TextMesh>().text = GlobalValues.LastDLCause;
                    else
                        badge.GetComponent<TextMesh>().text = $"Death recieved from {GlobalValues.LastDLResponsible}.";

                    // Find the badge text box.
                    GameObject badgeHighlight = GameObject.Find("hud_pause_textboxlarge");

                    // Check that we've actually found the badge text box.
                    if (badgeHighlight != null)
                    {
                        // Remove its X offset to center it.
                        badgeHighlight.transform.localPosition = new(0, badgeHighlight.transform.localPosition.y, badgeHighlight.transform.localPosition.z);

                        // Scale it by 1.5 times on the X axis.
                        badgeHighlight.transform.localScale = new(1.5f, badgeHighlight.transform.localScale.y, badgeHighlight.transform.localScale.z);
                    }
                }
            }
        }
    }
}
