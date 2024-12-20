namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuWorldMapConfirmPatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuWorldMapConfirm), "Start")]
        static bool LockChapters(ref int ___selectedStageID, ref FPMapLocationType ___selectedStageType)
        {
            switch (___selectedStageID)
            {
                // Sky Pirate Panic.
                // The Sigwada is handled in case 4, as it shares its ID with The Battlesphere.
                case 3:
                    if (!Plugin.APSave.UnlockedChapters[1])
                    {
                        UnityEngine.Object.Destroy(UnityEngine.Object.FindObjectOfType<MenuWorldMapConfirm>().gameObject);
                        return false;
                    }
                    break;

                // The Sigwada AND The Battlesphere.
                case 4:
                    if (!Plugin.APSave.UnlockedChapters[1] && ___selectedStageType == FPMapLocationType.STAGE)
                    {
                        UnityEngine.Object.Destroy(UnityEngine.Object.FindObjectOfType<MenuWorldMapConfirm>().gameObject);
                        return false;
                    }
                    else if (!Plugin.APSave.UnlockedChapters[2] && ___selectedStageType == FPMapLocationType.HUB)
                    {
                        UnityEngine.Object.Destroy(UnityEngine.Object.FindObjectOfType<MenuWorldMapConfirm>().gameObject);
                        return false;
                    }
                    break;

                // Mystery of the Frozen North.
                case 5:
                case 6:
                case 7:
                case 8:
                    if (!Plugin.APSave.UnlockedChapters[0])
                    {
                        UnityEngine.Object.Destroy(UnityEngine.Object.FindObjectOfType<MenuWorldMapConfirm>().gameObject);
                        return false;
                    }
                    break;

                // Enter The Battlesphere.
                // The Battlesphere is handled in case 4, as it shares its ID with The Sigwada.
                case 9:
                case 10:
                    if (!Plugin.APSave.UnlockedChapters[2])
                    {
                        UnityEngine.Object.Destroy(UnityEngine.Object.FindObjectOfType<MenuWorldMapConfirm>().gameObject);
                        return false;
                    }
                    break;

                // Globe Opera.
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                    if (!Plugin.APSave.UnlockedChapters[3])
                    {
                        UnityEngine.Object.Destroy(UnityEngine.Object.FindObjectOfType<MenuWorldMapConfirm>().gameObject);
                        return false;
                    }
                    break;

                // Robot Wars! Snake VS Tarsier.
                case 16:
                case 17:
                    if (!Plugin.APSave.UnlockedChapters[5])
                    {
                        UnityEngine.Object.Destroy(UnityEngine.Object.FindObjectOfType<MenuWorldMapConfirm>().gameObject);
                        return false;
                    }
                    break;

                // Justice in the Sky Paradise.
                case 18:
                case 19:
                    if (!Plugin.APSave.UnlockedChapters[4])
                    {
                        UnityEngine.Object.Destroy(UnityEngine.Object.FindObjectOfType<MenuWorldMapConfirm>().gameObject);
                        return false;
                    }
                    break;

                // Echoes of the Dragon War.
                case 20:
                case 21:
                case 22:
                    if (!Plugin.APSave.UnlockedChapters[6])
                    {
                        UnityEngine.Object.Destroy(UnityEngine.Object.FindObjectOfType<MenuWorldMapConfirm>().gameObject);
                        return false;
                    }
                    break;

                // Bakunawa.
                case 23:
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                case 32:
                    if (!Plugin.APSave.UnlockedChapters[7])
                    {
                        UnityEngine.Object.Destroy(UnityEngine.Object.FindObjectOfType<MenuWorldMapConfirm>().gameObject);
                        return false;
                    }
                    break;
            }

            return true;
        }
    }
}
