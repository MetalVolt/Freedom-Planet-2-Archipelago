using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class FPPlayerPatcher
    {
        /// <summary>
        /// Whether the player is in a position to send a DeathLink.
        /// </summary>
        public static bool canSendDeathLink = true;

        /// <summary>
        /// Whether or not another player has sent a DeathLink that the game is waiting on.
        /// </summary>
        public static bool hasBufferedDeathLink = false;

        /// <summary>
        /// Handles resetting the DeathLink flag.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "Start")]
        static void Start()
        {
            // Reenable the DeathLink flag.
            canSendDeathLink = true;
        }

        /// <summary>
        /// Recieves a DeathLink.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "Update")]
        static void RecieveDeathLink()
        {
            // Check that the stage has finished loading and that we have a DeathLink waiting.
            if (FPStage.objectsRegistered && hasBufferedDeathLink)
            {
                // Turn off our can send flag so we don't send a DeathLink of our own.
                canSendDeathLink = false;

                // Find the player.
                FPPlayer player = UnityEngine.Object.FindObjectOfType<FPPlayer>();

                // If the DeathLink slot value is just enable, then force run the player's crush action.
                if ((long)Plugin.SlotData["death_link"] == 1)
                    player.Action_Crush();

                // If the DeathLink slot value is enable_survive, then kill the player normally.
                if ((long)Plugin.SlotData["death_link"] == 2)
                {
                    // Remove the player's invincibility, guard and health.
                    player.invincibilityTime = 0;
                    player.guardTime = 0;
                    player.health = 0;

                    // Damage the player.
                    player.Action_Hurt();
                }

                // Turn our buffered flag back off.
                hasBufferedDeathLink = false;
            }
        }

        /// <summary>
        /// Handles changing gravity when either a Moon Gravity Trap or Double Gravity Trap is active.
        /// If both are active at once, then the double gravity trap should take priority?
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "Update")]
        static void GravityTraps(ref float ___gravityStrength)
        {
            // Check if the Moon Gravity Trap timer is going.
            if (Plugin.MoonGravityTrapTimer > 0f)
            {
                // Take the delta time away from the trap timer.
                Plugin.MoonGravityTrapTimer -= FPStage.deltaTime;

                // If we haven't already halved the gravity, then do so.
                if (___gravityStrength != -0.1875f)
                    ___gravityStrength = -0.1875f;
            }

            // Check if the Double Gravity Trap timer is going.
            if (Plugin.DoubleGravityTrapTimer > 0f)
            {
                // Take the delta time away from the trap timer.
                Plugin.DoubleGravityTrapTimer -= FPStage.deltaTime;

                // If we haven't already doubled the gravity, then do so.
                if (___gravityStrength != -0.75f)
                    ___gravityStrength = -0.75f;
            }

            // If both trap timers aren't running and we don't have the correct gravity, then reset it.
            if (Plugin.MoonGravityTrapTimer <= 0f && Plugin.DoubleGravityTrapTimer <= 0f && ___gravityStrength != -0.375f)
                ___gravityStrength = -0.375f;
        }

        /// <summary>
        /// Handles flipping the screen when a Mirror Trap is active.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "Update")]
        static void MirrorTrapScreenFlip(ref FPPlayerInput ___input)
        {
            // Check if the Mirror Trap timer is going.
            if (Plugin.MirrorTrapTimer > 0f)
            {
                // Take the delta time away from the trap timer.
                Plugin.MirrorTrapTimer -= FPStage.deltaTime;

                // Find the Pixel Art Target renderer.
                GameObject pixelArtTarget = GameObject.Find("Pixel Art Target");

                // If we've found it, then check if it has a positive X scale, if so, invert it.
                if (pixelArtTarget != null)
                    if (pixelArtTarget.transform.localScale.x > 0)
                        pixelArtTarget.transform.localScale = new Vector3(pixelArtTarget.transform.localScale.x * -1f, pixelArtTarget.transform.localScale.y, pixelArtTarget.transform.localScale.z);
            }

            // Check if the Mirror Trap timer has gone below 0.
            else if (Plugin.MirrorTrapTimer < 0f)
            {
                // Set the trap timer to 0 so this check doesn't refire.
                Plugin.MirrorTrapTimer = 0f;

                // Find the Pixel Art Target renderer.
                GameObject pixelArtTarget = GameObject.Find("Pixel Art Target");

                // If we've found it, then check if it has a negative X scale, if so, invert it.
                if (pixelArtTarget != null)
                    if (pixelArtTarget.transform.localScale.x < 0)
                        pixelArtTarget.transform.localScale = new Vector3(pixelArtTarget.transform.localScale.x * -1f, pixelArtTarget.transform.localScale.y, pixelArtTarget.transform.localScale.z);
            }
        }

        /// <summary>
        /// Handles flipping left and right controls when a Mirror Trap is active.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPPlayer), "ProcessInputControl")]
        static bool MirrorTrapControls_NoRewired(ref FPPlayerInput ___input)
        {
            // If the Mirror Trap Timer isn't going, then simply run the original function instead.
            if (Plugin.MirrorTrapTimer <= 0f)
                return true;

            // Get the axis input from the controller.
            float xAxis = InputControl.GetAxis(Controls.axes.horizontal);
            float yAxis = InputControl.GetAxis(Controls.axes.vertical);

            // Reset the directional presses.
            ___input.upPress = false;
            ___input.downPress = false;
            ___input.leftPress = false;
            ___input.rightPress = false;

            // Check if the player is pressing left or not and set the flags for pressing right accordingly.
            if (xAxis < 0f - InputControl.joystickThreshold)
            {
                if (!___input.right)
                    ___input.rightPress = true;

                ___input.right = true;
            }
            else
                ___input.right = false;

            // Check if the player is pressing right or not and set the flags for pressing left accordingly.
            if (xAxis > InputControl.joystickThreshold)
            {
                if (!___input.left)
                    ___input.leftPress = true;

                ___input.left = true;
            }
            else
                ___input.left = false;

            // Check if the player is pressing up or not and set the flags accordingly.
            if (yAxis > InputControl.joystickThreshold)
            {
                if (!___input.up)
                    ___input.upPress = true;

                ___input.up = true;
            }
            else
                ___input.up = false;

            // Check if the player is pressing down or not and set the flags accordingly.
            if (yAxis < 0f - InputControl.joystickThreshold)
            {
                if (!___input.down)
                    ___input.downPress = true;

                ___input.down = true;
            }
            else
                ___input.down = false;

            // Check if the player is pressing the face buttons and set the flags accordingly.
            ___input.jumpPress = InputControl.GetButtonDown(Controls.buttons.jump);
            ___input.jumpHold = InputControl.GetButton(Controls.buttons.jump);
            ___input.attackPress = InputControl.GetButtonDown(Controls.buttons.attack);
            ___input.attackHold = InputControl.GetButton(Controls.buttons.attack);
            ___input.specialPress = InputControl.GetButtonDown(Controls.buttons.special);
            ___input.specialHold = InputControl.GetButton(Controls.buttons.special);
            ___input.guardPress = InputControl.GetButtonDown(Controls.buttons.guard);
            ___input.guardHold = InputControl.GetButton(Controls.buttons.guard);
            ___input.confirm = ___input.jumpPress | InputControl.GetButtonDown(Controls.buttons.pause);
            ___input.cancel = ___input.attackPress | Input.GetKey(KeyCode.Escape);

            // Stop the original function from running.
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPPlayer), "ProcessRewired")]
        static bool MirrorTrapControls_Rewired(ref FPPlayerInput ___input, ref Player ___rewiredPlayerInput)
        {
            // If the Mirror Trap Timer isn't going, then simply run the original function instead.
            if (Plugin.MirrorTrapTimer <= 0f)
                return true;

            // Reset the directional presses.
            ___input.upPress = false;
            ___input.downPress = false;
            ___input.leftPress = false;
            ___input.rightPress = false;

            // Check if the player is pressing left or not and set the flags for pressing right accordingly.
            if (___rewiredPlayerInput.GetButton("Left"))
            {
                if (!___input.right)
                    ___input.rightPress = true;

                ___input.right = true;
            }
            else
                ___input.right = false;

            // Check if the player is pressing right or not and set the flags for pressing left accordingly.
            if (___rewiredPlayerInput.GetButton("Right"))
            {
                if (!___input.left)
                    ___input.leftPress = true;

                ___input.left = true;
            }
            else
                ___input.left = false;

            // Check if the player is pressing up or not and set the flags accordingly.
            if (___rewiredPlayerInput.GetButton("Up"))
            {
                if (!___input.up)
                    ___input.upPress = true;

                ___input.up = true;
            }
            else
                ___input.up = false;

            // Check if the player is pressing down or not and set the flags accordingly.
            if (___rewiredPlayerInput.GetButton("Down"))
            {
                if (!___input.down)
                    ___input.downPress = true;

                ___input.down = true;
            }
            else
                ___input.down = false;

            // Check if the player is pressing the face buttons and set the flags accordingly.
            ___input.jumpPress = ___rewiredPlayerInput.GetButtonDown("Jump");
            ___input.jumpHold = ___rewiredPlayerInput.GetButton("Jump");
            ___input.attackPress = ___rewiredPlayerInput.GetButtonDown("Attack");
            ___input.attackHold = ___rewiredPlayerInput.GetButton("Attack");
            ___input.specialPress = ___rewiredPlayerInput.GetButtonDown("Special");
            ___input.specialHold = ___rewiredPlayerInput.GetButton("Special");
            ___input.guardPress = ___rewiredPlayerInput.GetButtonDown("Guard");
            ___input.guardHold = ___rewiredPlayerInput.GetButton("Guard");
            ___input.confirm = ___input.jumpPress | InputControl.GetButtonDown(Controls.buttons.pause);
            ___input.cancel = ___input.attackPress | Input.GetKey(KeyCode.Escape);

            // Stop the original function from running.
            return false;
        }

        /// <summary>
        /// Sets the flag for being able to send DeathLinks upon reviving.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "State_KO_Recover")]
        static void KORecover() => canSendDeathLink = true;

        /// <summary>
        /// Calls the SendDeathLink function depending on the player state.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "State_KO")]
        static void KOed() => SendDeathLink($"{GetPlayer()} got slapped.", false);
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "Action_Crush")]
        static void Crush() => SendDeathLink($"{GetPlayer()} became a pancake.", false);
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "State_CrushKO")]
        static void Fall() => SendDeathLink($"{GetPlayer()} fell in a hole.", true);
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "State_FallKO")]
        static void RingOut()
        {
            if (SceneManager.GetActiveScene().name == "Battlesphere_RingOut")
                SendDeathLink($"{GetPlayer()} fell in a hole.", false);
        }

        /// <summary>
        /// Sends a DeathLink.
        /// <paramref name="reason">The reason shown to other clients.</paramref>
        /// </summary>
        static void SendDeathLink(string reason, bool checkHealth)
        {
            // If DeathLink is disabled, then don't run any of this code.
            if ((long)Plugin.SlotData["death_link"] == 0)
                return;

            // Check if we can actually send a DeathLink.
            if (canSendDeathLink)
            {
                // Check if this DeathLink relies on the player's heatlh status.
                if (checkHealth)
                {
                    // Find the player.
                    FPPlayer player = UnityEngine.Object.FindObjectOfType<FPPlayer>();

                    // Check the player actually exists and see if they have any health. If so, don't send a DeathLink.
                    if (player != null)
                        if (player.health >= 0f)
                            return;
                }

                // Send a DeathLink.
                Plugin.DeathLink.SendDeathLink(new Archipelago.MultiClient.Net.BounceFeatures.DeathLink.DeathLink("Freedom Planet 2", reason));

                // Set the flag to avoid sending extras.
                canSendDeathLink = false;

                // Print the DeathLink to the console too.
                Console.WriteLine($"Sent DeathLink, reason:\r\n\t{reason}");
            }
        }

        /// <summary>
        /// Get the name of the active character.
        /// </summary>
        /// <returns>The character name.</returns>
        private static string GetPlayer()
        {
            switch (FPSaveManager.character)
            {
                case FPCharacterID.LILAC: return "Lilac";
                case FPCharacterID.CAROL: case FPCharacterID.BIKECAROL: return "Carol";
                case FPCharacterID.MILLA: return "Milla";
                case FPCharacterID.NEERA: return "Neera";
                default: return "Somebody we have no knowledge of";
            }
        }
    }
}
