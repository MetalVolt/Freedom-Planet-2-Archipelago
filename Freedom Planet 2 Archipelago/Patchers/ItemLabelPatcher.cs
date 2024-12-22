using UnityEngine;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class ItemLabelPatcher
    {
        /// <summary>
        /// Extends the background on the ItemLabel to fit more text.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemLabel), "Update")]
        static void BackgroundExtend()
        {
            // Find the background of this ItemLabel.
            SpriteRenderer labelBG = UnityEngine.Object.FindObjectOfType<ItemLabel>().GetComponent<SpriteRenderer>();

            // Change the X value on its local scale to 1.75.
            labelBG.transform.localScale = new(1.75f, labelBG.transform.localScale.y, labelBG.transform.localScale.z);
        }
    }
}
