using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace HofmaDresuBoomboxPlaylist.Patches
{
    [HarmonyPatch(typeof(BoomboxItem), "StartMusic")]
    internal class BoomboxItem_StartMusic
    {
        static void Postfix(BoomboxItem __instance, bool startMusic)
        {
            if (startMusic)
            {
                switch(__instance.boomboxAudio.clip.name)
                {
                    case "Bitter Regret.mp3":
                        __instance.boomboxAudio.volume = 0.6f;
                        break;
                    case "Lethal Company Blues.mp3":
                        __instance.boomboxAudio.volume = 0.4f;
                        break;
                }
                HofmaDresuBoomboxPlaylist.LogInfo($"Playing {__instance.boomboxAudio.clip.name} at volume {__instance.boomboxAudio.volume}");
            }
        }
    }
}
