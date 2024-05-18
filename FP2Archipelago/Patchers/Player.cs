using Archipelago.MultiClient.Net.Models;

namespace FP2Archipelago.Patchers
{
    public class Player
    {
        /// <summary>
        /// Whether the player is in a position to send a DeathLink.
        /// </summary>
        public static bool canSendDL = true;

        /// <summary>
        /// Whether the player has various things buffered.
        /// </summary>
        public static bool bufferedDL = false;
        public static bool bufferedMoonGravity = false;
        public static bool bufferedDoubleGravity = false;
        public static bool bufferedIceTrap = false;

        /// <summary>
        /// Flips the screen if a Mirror Trap is active.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "Update")]
        static void FlipScreen()
        {
            // Check if we actually need to mirror the scene or not.
            if (GlobalValues.IsMirrored)
            {
                // Find the Pixel Art Target renderer.
                GameObject pixelArtTarget = GameObject.Find("Pixel Art Target");

                // If we've found it, then check if it has a positive X scale, if so, invert it.
                if (pixelArtTarget != null)
                    if (pixelArtTarget.transform.localScale.x > 0)
                        pixelArtTarget.transform.localScale = new Vector3(pixelArtTarget.transform.localScale.x * -1f, pixelArtTarget.transform.localScale.y, pixelArtTarget.transform.localScale.z);
            }
        }

        /// <summary>
        /// Disables a Reverse Trap if the stage timer matches the expire timer count.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "Update")]
        static void ReverseTrap()
        {
            if (GlobalValues.ReverseTrapExpireTime != 60)
                if (FPStage.currentStage.seconds == GlobalValues.ReverseTrapExpireTime)
                    GlobalValues.ReverseTrapExpireTime = 60;
        }

        /// <summary>
        /// Sets the flag for being able to send DeathLinks on stage start and handle any buffered traps.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "Start")]
        static void Start(ref float ___gravityStrength, ref float ___deceleration, ref float ___skidDeceleration)
        {
            if (bufferedMoonGravity)
            {
                ___gravityStrength /= 2;
                bufferedMoonGravity = false;
            }

            if (bufferedDoubleGravity)
            {
                ___gravityStrength *= 2;
                bufferedDoubleGravity = false;
            }

            if (bufferedIceTrap)
            {
                ___deceleration /= 4;
                ___skidDeceleration /= 4;
                bufferedIceTrap = false;
            }

            // If we don't have a buffered DeathLink queued, then set the flag to be able to send one.
            if (!bufferedDL)
            {
                canSendDL = true;
            }
            
            // If we do have a buffered DL, then recieve it.
            else
            {
                // Get the player.
                FPPlayer player = UnityEngine.Object.FindObjectOfType<FPPlayer>();

                // Check that the player has been found.
                if (player != null)
                {
                    // Remove the player's invincibility, guard and health.
                    player.invincibilityTime = 0;
                    player.guardTime = 0;
                    player.health = 0;

                    // Damage the player.
                    player.Action_Hurt();

                    // Disabled the buffered DeathLink flag.
                    bufferedDL = false;

                    // Create a badge message to give feedback.
                    BadgeMessage badgeMessage = UnityEngine.Object.Instantiate(FPStage.currentStage.badgeMessage);

                    // Set the badge message ID to 19 (the museum restoration one).
                    badgeMessage.id = 59;

                    // Calculate how long the badge message should be shown for.
                    badgeMessage.timer = 0f - FPStage.badgeDisplayOffset;

                    // Calculate and set the local position of the badge message.
                    badgeMessage.transform.localPosition += new Vector3(0f, Mathf.Ceil(FPStage.badgeDisplayOffset / 100f) % 3f * 64f, 0f);

                    // Increment the badge display offset for if multiple badge messages have to be shown at once for whatever reason.
                    FPStage.badgeDisplayOffset += 100f;
                }
            }
        }

        /// <summary>
        /// Sets the flag for being able to send DeathLinks upon reviving.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "State_KO_Recover")]
        static void KORecover() => canSendDL = true;

        /// <summary>
        /// Sends a DeathLink.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "State_KO")]
        static void KOed() => SendDeathLink($"{GetPlayer()} got slapped.", true);

        /// <summary>
        /// Sends a DeathLink.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "Action_Crush")]
        static void Crush() => SendDeathLink($"{GetPlayer()} became a pancake.", false);

        /// <summary>
        /// Sends a DeathLink.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "State_CrushKO")]
        static void Fall() => SendDeathLink($"{GetPlayer()} fell in a hole.", true);

        /// <summary>
        /// Sends a DeathLink.
        /// <paramref name="reason">The reason shown to other clients.</paramref>
        /// </summary>
        static void SendDeathLink(string reason, bool checkHealth)
        {
            // Check if we can actually send a DeathLink.
            if (canSendDL)
            {
                // Check if this DeathLink relies on the player's heatlh status.
                if (checkHealth)
                {
                    // Find the player.
                    FPPlayer player = UnityEngine.Object.FindObjectOfType<FPPlayer>();

                    // Check the player actually exists.
                    if (player != null)
                    {
                        // Check if the player's health is less than 0 (as this does get called by the standard damage function).
                        if (player.health < 0f)
                        {
                            // Send a DeathLink.
                            GlobalValues.DeathLink.SendDeathLink(new Archipelago.MultiClient.Net.BounceFeatures.DeathLink.DeathLink("Freedom Planet 2", reason));

                            // Set the flag to avoid sending extras.
                            canSendDL = false;

                            // Print the DeathLink to the console too.
                            Console.WriteLine($"Sent DeathLink, reason:\r\n\t{reason}");
                        }
                    }
                }
                
                // If not, just send it.
                else
                {
                    // Send a DeathLink.
                    GlobalValues.DeathLink.SendDeathLink(new Archipelago.MultiClient.Net.BounceFeatures.DeathLink.DeathLink("Freedom Planet 2", reason));

                    // Set the flag to avoid sending extras.
                    canSendDL = false;

                    // Print the DeathLink to the console too.
                    Console.WriteLine($"Sent DeathLink, reason:\r\n\t{reason}");
                }
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

    // Taken from https://github.com/Kuborros/MirrorMode with only very minor modifications.
    // TODO: Can I use this code?
    class PatchControlls
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPPlayer), "ProcessInputControl", MethodType.Normal)]
        static bool Prefix(FPPlayer __instance)
        {
            float axis = InputControl.GetAxis(Controls.axes.horizontal, false);
            float axis2 = InputControl.GetAxis(Controls.axes.vertical, false);
            __instance.input.upPress = false;
            __instance.input.downPress = false;
            __instance.input.leftPress = false;
            __instance.input.rightPress = false;
            if (GlobalValues.IsMirrored || GlobalValues.ReverseTrapExpireTime != 60)
            {
                if (GlobalValues.IsMirrored && GlobalValues.ReverseTrapExpireTime != 60)
                    return true;

                if (axis > InputControl.joystickThreshold)
                {
                    if (!__instance.input.right)
                    {
                        __instance.input.rightPress = true;
                    }
                    __instance.input.right = true;
                }
                else
                {
                    __instance.input.right = false;
                }
                if (axis < -InputControl.joystickThreshold)
                {
                    if (!__instance.input.left)
                    {
                        __instance.input.leftPress = true;
                    }
                    __instance.input.left = true;
                }
                else
                {
                    __instance.input.left = false;
                }
            }
            else
            {
                if (axis > InputControl.joystickThreshold)
                {
                    if (!__instance.input.right)
                    {
                        __instance.input.rightPress = true;
                    }
                    __instance.input.right = true;
                }
                else
                {
                    __instance.input.right = false;
                }
                if (axis < -InputControl.joystickThreshold)
                {
                    if (!__instance.input.left)
                    {
                        __instance.input.leftPress = true;
                    }
                    __instance.input.left = true;
                }
                else
                {
                    __instance.input.left = false;
                }
            }
            if (axis2 > InputControl.joystickThreshold)
            {
                if (!__instance.input.up)
                {
                    __instance.input.upPress = true;
                }
                __instance.input.up = true;
            }
            else
            {
                __instance.input.up = false;
            }
            if (axis2 < -InputControl.joystickThreshold)
            {
                if (!__instance.input.down)
                {
                    __instance.input.downPress = true;
                }
                __instance.input.down = true;
            }
            else
            {
                __instance.input.down = false;
            }
            __instance.input.jumpPress = InputControl.GetButtonDown(Controls.buttons.jump, false);
            __instance.input.jumpHold = InputControl.GetButton(Controls.buttons.jump, false);
            __instance.input.attackPress = InputControl.GetButtonDown(Controls.buttons.attack, false);
            __instance.input.attackHold = InputControl.GetButton(Controls.buttons.attack, false);
            __instance.input.specialPress = InputControl.GetButtonDown(Controls.buttons.special, false);
            __instance.input.specialHold = InputControl.GetButton(Controls.buttons.special, false);
            __instance.input.guardPress = InputControl.GetButtonDown(Controls.buttons.guard, false);
            __instance.input.guardHold = InputControl.GetButton(Controls.buttons.guard, false);
            __instance.input.confirm = (__instance.input.jumpPress | InputControl.GetButtonDown(Controls.buttons.pause, false));
            __instance.input.cancel = (__instance.input.attackPress | Input.GetKey(KeyCode.Escape));

            return false;
        }
    }

    class PatchControllsRewired
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPPlayer), "ProcessRewired", MethodType.Normal)]
        static bool Prefix(FPPlayer __instance)
        {
            __instance.input.upPress = false;
            __instance.input.downPress = false;
            __instance.input.leftPress = false;
            __instance.input.rightPress = false;
            if (GlobalValues.IsMirrored || GlobalValues.ReverseTrapExpireTime != 60)
            {
                if (GlobalValues.IsMirrored && GlobalValues.ReverseTrapExpireTime != 60)
                    return true;

                if (FPPlayer.rewiredPlayerInput.GetButton("Left"))
                {
                    if (!__instance.input.right)
                    {
                        __instance.input.rightPress = true;
                    }
                    __instance.input.right = true;
                }
                else
                {
                    __instance.input.right = false;
                }
                if (FPPlayer.rewiredPlayerInput.GetButton("Right"))
                {
                    if (!__instance.input.left)
                    {
                        __instance.input.leftPress = true;
                    }
                    __instance.input.left = true;
                }
                else
                {
                    __instance.input.left = false;
                }
            }
            else
            {
                if (FPPlayer.rewiredPlayerInput.GetButton("Right"))
                {
                    if (!__instance.input.right)
                    {
                        __instance.input.rightPress = true;
                    }
                    __instance.input.right = true;
                }
                else
                {
                    __instance.input.right = false;
                }
                if (FPPlayer.rewiredPlayerInput.GetButton("Left"))
                {
                    if (!__instance.input.left)
                    {
                        __instance.input.leftPress = true;
                    }
                    __instance.input.left = true;
                }
                else
                {
                    __instance.input.left = false;
                }
            }
            if (FPPlayer.rewiredPlayerInput.GetButton("Up"))
            {
                if (!__instance.input.up)
                {
                    __instance.input.upPress = true;
                }
                __instance.input.up = true;
            }
            else
            {
                __instance.input.up = false;
            }
            if (FPPlayer.rewiredPlayerInput.GetButton("Down"))
            {
                if (!__instance.input.down)
                {
                    __instance.input.downPress = true;
                }
                __instance.input.down = true;
            }
            else
            {
                __instance.input.down = false;
            }
            __instance.input.jumpPress = FPPlayer.rewiredPlayerInput.GetButtonDown("Jump");
            __instance.input.jumpHold = FPPlayer.rewiredPlayerInput.GetButton("Jump");
            __instance.input.attackPress = FPPlayer.rewiredPlayerInput.GetButtonDown("Attack");
            __instance.input.attackHold = FPPlayer.rewiredPlayerInput.GetButton("Attack");
            __instance.input.specialPress = FPPlayer.rewiredPlayerInput.GetButtonDown("Special");
            __instance.input.specialHold = FPPlayer.rewiredPlayerInput.GetButton("Special");
            __instance.input.guardPress = FPPlayer.rewiredPlayerInput.GetButtonDown("Guard");
            __instance.input.guardHold = FPPlayer.rewiredPlayerInput.GetButton("Guard");
            __instance.input.confirm = (__instance.input.jumpPress | InputControl.GetButtonDown(Controls.buttons.pause, false));
            __instance.input.cancel = (__instance.input.attackPress | Input.GetKey(KeyCode.Escape));

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPPlayer), "ProcessRewired", MethodType.Normal)]
        static void Postfix(FPPlayer __instance)
        {
            if (GlobalValues.ReverseTrapExpireTime != 60)
            {
                if (FPPlayer.rewiredPlayerInput.GetButton("Down"))
                {
                    if (!__instance.input.up)
                    {
                        __instance.input.upPress = true;
                    }
                    __instance.input.up = true;
                }
                else
                {
                    __instance.input.up = false;
                }
                if (FPPlayer.rewiredPlayerInput.GetButton("Up"))
                {
                    if (!__instance.input.down)
                    {
                        __instance.input.downPress = true;
                    }
                    __instance.input.down = true;
                }
                else
                {
                    __instance.input.down = false;
                }

                __instance.input.attackPress = FPPlayer.rewiredPlayerInput.GetButtonDown("Jump");
                __instance.input.attackHold = FPPlayer.rewiredPlayerInput.GetButton("Jump");
                __instance.input.jumpPress = FPPlayer.rewiredPlayerInput.GetButtonDown("Attack");
                __instance.input.jumpHold = FPPlayer.rewiredPlayerInput.GetButton("Attack");
                __instance.input.guardPress = FPPlayer.rewiredPlayerInput.GetButtonDown("Special");
                __instance.input.guardHold = FPPlayer.rewiredPlayerInput.GetButton("Special");
                __instance.input.specialPress = FPPlayer.rewiredPlayerInput.GetButtonDown("Guard");
                __instance.input.specialHold = FPPlayer.rewiredPlayerInput.GetButton("Guard");
            }
        }
    }
}
