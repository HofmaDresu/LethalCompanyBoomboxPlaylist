using HofmaDresuBoomboxPlaylist.Managers;
using HarmonyLib;

namespace HofmaDresuBoomboxPlaylist.Patches
{
    [HarmonyPatch(typeof(StartOfRound), "Awake")]
    internal class StartOfRound_Awake
    {
        static void Prefix() => AudioManager.Load();
    }
}