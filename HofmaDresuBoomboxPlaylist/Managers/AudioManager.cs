using BepInEx;
using HofmaDresuBoomboxPlaylist.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace HofmaDresuBoomboxPlaylist.Managers
{
    internal static class AudioManager
    {

        public static event Action? OnAllSongsLoaded;
        public static bool FinishedLoading => finishedLoading;

        static readonly List<AudioClip> clips = [];
        static bool firstRun = true;
        static bool finishedLoading = false;

        // r2modman seems to install all files in hofmadresu-HofmaDresuBoomboxPlaylist instead of in my structure. This needs to be investigated, but hopefully this works for now.
#if DEBUG
        static readonly string directory = Path.Combine(Paths.BepInExRootPath, "plugins", "HofmaDresuBoomboxPlaylistSongs");
#else
        static readonly string directory = Path.Combine(Paths.BepInExRootPath, "plugins", "hofmadresu-HofmaDresuBoomboxPlaylist");
#endif

        public static void Load()
        {
            if (firstRun)
            {
                firstRun = false;
                string[] allSongPaths = Directory.GetFiles(directory);

                if (allSongPaths.Length == 0)
                {
                    HofmaDresuBoomboxPlaylist.LogInfo("No songs found!");
                    return;
                }

                HofmaDresuBoomboxPlaylist.LogInfo("Preparing to load AudioClips...");

                var coroutines = new List<Coroutine>();
                foreach (var track in allSongPaths)
                {
                    var coroutine = SharedCoroutineStarter.StartCoroutine(LoadAudioClip(track));
                    coroutines.Add(coroutine);
                }

                SharedCoroutineStarter.StartCoroutine(WaitForAllClips(coroutines));
            }
        }

        static readonly string[] allowedSongs = ["Bitter Regret.mp3", "Lethal Company Blues.mp3"];

        private static IEnumerator LoadAudioClip(string filePath)
        {
            HofmaDresuBoomboxPlaylist.LogInfo($"Loading {filePath}!");

            // Completely skip non-mp3 without logging
            if (!filePath.EndsWith(".mp3"))
            {
                yield break;
            }

            if (!allowedSongs.Any(filePath.EndsWith))
            {
                HofmaDresuBoomboxPlaylist.LogError($"The song {filePath} is not a part of this package, skipping!");
                yield break;
            }

            var loader = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.MPEG);

            if (loader.downloadHandler is DownloadHandlerAudioClip handler)
            {
                handler.streamAudio = true;
            }

            loader.SendWebRequest();

            while (true)
            {
                if (loader.isDone) break;
                yield return null;
            }

            if (loader.error != null)
            {
                HofmaDresuBoomboxPlaylist.LogInfo($"Error loading clip from path: {filePath}\n{loader.error}");
                HofmaDresuBoomboxPlaylist.LogInfo(loader.error);
                yield break;
            }

            var clip = DownloadHandlerAudioClip.GetContent(loader);
            if (clip && clip.loadState == AudioDataLoadState.Loaded)
            {
                HofmaDresuBoomboxPlaylist.LogInfo($"Loaded {filePath}");
                clip.name = Path.GetFileName(filePath);
                clips.Add(clip);
                yield break;
            }

            // Failed to load.
            HofmaDresuBoomboxPlaylist.LogInfo($"Failed to load clip at: {filePath}\nThis might be due to an mismatch between the audio codec and the file extension!");
        }

        private static IEnumerator WaitForAllClips(List<Coroutine> coroutines)
        {
            foreach (var coroutine in coroutines)
            {
                yield return coroutine;
            }

            clips.Sort((first, second) => first.name.CompareTo(second.name));

            finishedLoading = true;
            OnAllSongsLoaded?.Invoke();
            OnAllSongsLoaded = null;
        }

        public static void ApplyClips(BoomboxItem __instance)
        {
            HofmaDresuBoomboxPlaylist.LogInfo($"Applying clips!");

            if (clips.Any())
                __instance.musicAudios = [.. clips];

            HofmaDresuBoomboxPlaylist.LogInfo($"Total Clip Count: {__instance.musicAudios.Length}");
        }
    }
}
