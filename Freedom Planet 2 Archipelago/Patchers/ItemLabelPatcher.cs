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
            // Find this ItemLabel.
            ItemLabel label = UnityEngine.Object.FindObjectOfType<ItemLabel>();

            // Check that the label actually exists (you'd think it would but...)
            if (label != null)
            {
                // Find this label's background.
                SpriteRenderer labelBG = label.GetComponent<SpriteRenderer>();

                // Change the X value on its local scale to 1.75.
                if (labelBG != null )
                    labelBG.transform.localScale = new(1.75f, labelBG.transform.localScale.y, labelBG.transform.localScale.z);
            }
        }
    }
}
