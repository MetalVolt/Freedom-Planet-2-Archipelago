using System.Collections.Generic;

namespace Freedom_Planet_2_Archipelago
{
    internal class EasterEggMessage
    {
        // Set up the list of messages for the easter egg.
        private static readonly List<string> EasterEggMessages =
        [
            "Closed due to rumours of sacrificial rituals in local theme park.", // OpenRCT2
            "Closed due to infestation of Silverfish from contaminated cobblestone.", // Minecraft
            "gone to burger king be back l8r", // General Multiworld
            "Closed on Wednesdays. Just go to Joja instead.", // Stardew Valley
            "Closed to tidy up excessive amount of broken pots", // Zelda
            "Tending to Chao. Come back later.", // Sonic Adventure
            "out hunting bounties", // Metroid
            "Won a trip to Phobos. Closed until further notice.", // Doom
            "Roommate let all the monsters in again.", // Terraria
            "Closed due to Maverick attack", // Megaman X
            "off hiking for strawberries", // Celeste
            "Closed while I go find all my hourglasses AGAIN! >:(", // A Hat in Time
            "Binging Disney Movies. Do Not Disturb.", // Kingdom Hearts
            "Closed until my voice comes back.", // Mario & Luigi: Superstar Saga
            "Waiting on a fresh stock of Ukuleles.", // Risk of Rain 2
            "gone swimming", // Subnautica
            "Reading this closed notice. It fills you with determination.", // Undertale
            "Closed until new (MORE ORGANISED!) chefs found.", // Overcooked
            "gone fishing. how unfortunate" // FNaF World
        ];

        // Set up a Random Number Generator
        private static readonly Random rng = new();

        /// <summary>
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPHubNPC), "SetCurrentDialog")]
        static void SetEasterEggDialog(ref NPCDialog[] ___dialog)
        {
            // Generate some numbers to get message indices.
            int dojoMessageIndex = rng.Next(EasterEggMessages.Count);
            int tavernMessageIndex = rng.Next(EasterEggMessages.Count);
            while (tavernMessageIndex == dojoMessageIndex)
                tavernMessageIndex = rng.Next(EasterEggMessages.Count);

            // Change the Dojo message.
            if (___dialog[0].lines[0].text == "No one's inside. I hope everyone evacuated safely.")
            {
                ___dialog[0].lines[0].text = EasterEggMessages[dojoMessageIndex];
                ___dialog[0].lines[1].text = EasterEggMessages[dojoMessageIndex];
                ___dialog[0].lines[2].text = EasterEggMessages[dojoMessageIndex];
                ___dialog[0].lines[3].text = EasterEggMessages[dojoMessageIndex];
            }

            // Change the Tavern message.
            if (___dialog[0].lines[0].text == "It's empty inside. Even the drink barrels are gone.")
            {
                ___dialog[0].lines[0].text = EasterEggMessages[tavernMessageIndex];
                ___dialog[0].lines[1].text = EasterEggMessages[tavernMessageIndex];
                ___dialog[0].lines[2].text = EasterEggMessages[tavernMessageIndex];
                ___dialog[0].lines[3].text = EasterEggMessages[tavernMessageIndex];
            }
        }
    }
}
