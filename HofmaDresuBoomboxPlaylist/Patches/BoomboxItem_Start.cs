using HarmonyLib;
using HofmaDresuBoomboxPlaylist.Managers;
namespace HofmaDresuBoomboxPlaylist.Patches
{
    [HarmonyPatch(typeof(BoomboxItem), "Start")]
    internal class BoomboxItem_Start
    {
        static void Postfix(BoomboxItem __instance)
        {
            if (AudioManager.FinishedLoading)
                AudioManager.ApplyClips(__instance);
            else
                AudioManager.OnAllSongsLoaded += () => AudioManager.ApplyClips(__instance);
        }
    }
}
