using HarmonyLib;
using HofmaDresuBoomboxPlaylist.Managers;
namespace HofmaDresuBoomboxPlaylist.Patches
{
    [HarmonyPatch(typeof(BoomboxItem), "Start")]
    internal class BoomboxItem_Start
    {
        const float targetVolume = 0.4f;

        static void Postfix(BoomboxItem __instance)
        {
            if (__instance.boomboxAudio.volume != targetVolume)
            {
               __instance.boomboxAudio.volume = targetVolume;
                HofmaDresuBoomboxPlaylist.LogInfo($"volume set to {targetVolume}");
            }


            if (AudioManager.FinishedLoading)
                AudioManager.ApplyClips(__instance);
            else
                AudioManager.OnAllSongsLoaded += () => AudioManager.ApplyClips(__instance);
        }
    }
}
