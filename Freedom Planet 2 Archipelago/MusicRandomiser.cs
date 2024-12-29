using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Freedom_Planet_2_Archipelago
{
    // TODO: If the custom music hits an issue, then the game hangs indefinitely, fix this in some way.
    class MusicRandomiser
    {
        // A list of songs that the music randomiser shouldn't choose or change.
        static readonly List<string> BlacklistedSongs = ["M_Clear", "M_ClearSilent", "M_Invincibility", "M_SpeedGate"];

        // The text file that specifies custom song loop points.
        static string[] CustomSongLoops = [];

        /// <summary>
        /// Swaps out a call for one of the game's normal jingles to one of our custom ones.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPAudio), "PlayJingle", new Type[] { typeof(int) })]
        static bool PlayCustomJingle(ref int bgmID, ref FPAudio ___audioRef)
        {
            // Set up an audio clip.
            AudioClip clip = null;

            // Select the clip based on the ID that the function received.
            switch (bgmID)
            {
                // Invincibility.
                case 0:
                    if (Plugin.CustomInvincibility.Length == 0)
                        clip = ___audioRef.jingleInvincibility;
                    else
                        clip = Plugin.CustomInvincibility[Plugin.Randomiser.Next(0, Plugin.CustomInvincibility.Length)];
                    break;

                // Stage Clear.
                case 1:
                    if (Plugin.CustomInvincibility.Length == 0)
                        clip = ___audioRef.jingleStageComplete;
                    else
                        clip = Plugin.CustomClear[Plugin.Randomiser.Next(0, Plugin.CustomClear.Length)];
                    break;

                // Game Over (unused?).
                case 2:
                    clip = ___audioRef.jingleGameOver;
                    break;

                // 1UP.
                case 3:
                    if (Plugin.Custom1UP.Length == 0)
                        clip = ___audioRef.jingle1up;
                    else
                        clip = Plugin.Custom1UP[Plugin.Randomiser.Next(0, Plugin.Custom1UP.Length)];
                    break;
            }
            
            // Play the chosen clip using the OTHER PlayJingle function.
            FPAudio.PlayJingle(clip);

            // Stop the original copy of this function from running.
            return false;
        }

        /// <summary>
        /// Randomises the music that the game should play.
        /// </summary>
        /// <param name="bgmMusic">The music that the PlayMusic function in FPAudio has been told to use.</param>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPAudio), nameof(FPAudio.PlayMusic), new Type[] { typeof(AudioClip), typeof(float) })]
        static void RandomiseMusicPlayback(ref AudioClip bgmMusic)
        {
            // Check that the play music function has a bgm assigned.
            if (bgmMusic != null)
            {
                // Check that we've actually gotten a list of music. If we haven't, then return.
                if (Plugin.Music == null || Plugin.Music.Count == 0)
                    return;

                // Check if the track the game called for is on the blacklist.
                if (!BlacklistedSongs.Contains(bgmMusic.name))
                {
                    // Select a random music track to replace the called for song.
                    int musicIndex = Plugin.Randomiser.Next(0, Plugin.Music.Count);

                    // Select again if the track chosen is on the blacklist.
                    while (BlacklistedSongs.Contains(Plugin.Music[musicIndex].name))
                        musicIndex = Plugin.Randomiser.Next(0, Plugin.Music.Count);

                    // Check if the track we've chosen is a placeholder.
                    if (Plugin.Music[musicIndex].name.StartsWith("imported_") && Plugin.Music[musicIndex].length == 0)
                    {
                        // Inform the user that we're importing the custom song.
                        Console.WriteLine($@"Importing {Paths.GameRootPath}\mod_overrides\Archipelago\music\{Plugin.Music[musicIndex].name.Replace("imported_", "")}.ogg, game may hang for a short while.");

                        using (WWW audioLoader = new(FilePathToFileUrl($@"{Paths.GameRootPath}\mod_overrides\Archipelago\music\{Plugin.Music[musicIndex].name.Replace("imported_", "")}.ogg")))
                        {
                            // Freeze the game until the audio loader is done.
                            while (!audioLoader.isDone)
                                System.Threading.Thread.Sleep(1);

                            // Create an audio clip from the loaded file.
                            AudioClip audio = audioLoader.GetAudioClip(false, true, AudioType.OGGVORBIS);

                            // Freeze the application until the audio clip is loaded fully.
                            while (!(audio.loadState == AudioDataLoadState.Loaded))
                                System.Threading.Thread.Sleep(1);

                            // Copy over the name, as the newly loaded audio clip doesn't have it.
                            audio.name = Plugin.Music[musicIndex].name;

                            // Add the loaded audio to our list of music tracks.
                            bgmMusic = audio;

                            // Replace the placeholder for this track in the music list (this causes an eventual out of memory problem, OH WELL!).
                            Plugin.Music[musicIndex] = audio;
                        }
                    }
                    else
                    {
                        // Set the music to the chosen track.
                        bgmMusic = Plugin.Music[musicIndex];
                    }
                }
            }
        }

        /// <summary>
        /// Sets the loop point for a custom music track.
        /// </summary>
        /// <param name="bgmMusic">The music playing from the PlayMusic function.</param>
        /// <param name="___loopStart">The already set loop start value.</param>
        /// <param name="___loopEnd">The already set loop end value.</param>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPAudio), nameof(FPAudio.PlayMusic), new Type[] { typeof(AudioClip), typeof(float) })]
        static void CustomMusicLoopSet(ref AudioClip bgmMusic, ref float ___loopStart, ref float ___loopEnd)
        {
            // Check that the play music function has a bgm assigned.
            if (bgmMusic != null)
            {
                // Check if this track has the imported_ tag.
                if (bgmMusic.name.StartsWith("imported_"))
                {
                    // Loop through each entry in the loops file.
                    foreach (string loop in CustomSongLoops)
                    {
                        // Split this line on the divider character.
                        string[] split = loop.Split('|');

                        // Check if the name in the first part of the split is our current track's name, minus the imported_ tag.
                        if (split[0] == $"{bgmMusic.name.Replace("imported_", "")}")
                        {
                            // Parse the values from the split.
                            float loopStart = float.Parse(split[1]);
                            float loopEnd = float.Parse(split[2]);
                            bool inSeconds = false;

                            // If the inSeconds value is present, then parse it too.
                            if (split.Length > 3)
                                inSeconds = bool.Parse(split[3]);

                            // Check if we're in seconds and handle setting the loop points to the read values.
                            if (inSeconds)
                            {
                                ___loopStart = loopStart;
                                ___loopEnd = loopEnd;
                            }

                            // If we're not in seconds, then calculate the loop time in seconds by dividing the sample point read again the total sample count and length.
                            else
                            {
                                ___loopStart = (float)(loopStart / Math.Round(bgmMusic.samples / bgmMusic.length));
                                ___loopEnd = (float)(loopEnd / Math.Round(bgmMusic.samples / bgmMusic.length));
                            }

                            // Write the loaded loop points to the console.
                            Console.WriteLine($"Successfully loaded custom loop points ({___loopStart} - {___loopEnd}) for {bgmMusic.name.Replace("imported_", "")}.ogg.");

                            // Break out of the loop so we don't pointlessly check enteries past this one.
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Populates the music list.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuSpawner), "Start")]
        static void LookForMusic()
        {
            // Check if we haven't already gotten a list of music and that we're on the debug menu scene.
            if (Plugin.Music == null && SceneManager.GetActiveScene().name == "StageDebugMenu")
            {
                // Initialise the music list.
                Plugin.Music = [];

                // If the music directory doesn't exist, then return.
                if (!Directory.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\music"))
                    return;

                // Set the path where the custom music is located.
                string[] customTracks = Directory.GetFiles($@"{Paths.GameRootPath}\mod_overrides\Archipelago\music", "*.ogg");

                // If there wasn't any music, then also return.
                if (customTracks.Length == 0)
                    return;

                // Inform the user that we're finding custom music.
                Console.WriteLine("Finding custom music, game may hang for a short while.");

                // Loop through each OGG file in the mod overrides folder and create a placeholder audio clip for it.
                foreach (string oggFile in customTracks)
                    Plugin.Music.Add(AudioClip.Create($"imported_{Path.GetFileNameWithoutExtension(oggFile)}", 1, 2, 44100, true));

                // Get the custom jingles.
                Plugin.CustomInvincibility = GetJingles($@"{Paths.GameRootPath}\mod_overrides\Archipelago\jingles\invincibility");
                Plugin.CustomClear = GetJingles($@"{Paths.GameRootPath}\mod_overrides\Archipelago\jingles\clear");
                Plugin.Custom1UP = GetJingles($@"{Paths.GameRootPath}\mod_overrides\Archipelago\jingles\1up");

                // Print how much custom music has been loaded.
                Console.WriteLine($"Custom music has been loaded.\r\n\t{Plugin.Music.Count} songs.\r\n\t{Plugin.CustomInvincibility.Length} invincibility jingles.\r\n\t{Plugin.CustomClear.Length} clear jingles.\r\n\t{Plugin.Custom1UP.Length} 1UP jingles.");

                // Check that the custom loop points file exists and read the values if so.
                if (File.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\music\loop_points.txt"))
                    CustomSongLoops = File.ReadAllLines($@"{Paths.GameRootPath}\mod_overrides\Archipelago\music\loop_points.txt");
            }
        }

        /// <summary>
        /// Returns an array of audio clips from the OGG files in a directory.
        /// </summary>
        /// <param name="directory">The directory to get OGG files from.</param>
        /// <returns>An array of audio clips.</returns>
        private static AudioClip[] GetJingles(string directory)
        {
            // Return an empty array if the specified directory doesn't exist.
            if (!Directory.Exists(directory))
                return [];

            // Find all the OGG files in the given directory.
            string[] jingles = Directory.GetFiles(directory, "*.ogg");

            // Set up an array of audio clips.
            AudioClip[] clips = new AudioClip[jingles.Length];

            // Loop through each jingle.
            for (int jingleIndex = 0; jingleIndex < jingles.Length; jingleIndex++)
            {
                using (WWW audioLoader = new(FilePathToFileUrl(jingles[jingleIndex])))
                {
                    // Freeze the game until the audio loader is done.
                    while (!audioLoader.isDone)
                        System.Threading.Thread.Sleep(1);

                    // Create an audio clip from the loaded file.
                    AudioClip audio = audioLoader.GetAudioClip(false, true, AudioType.OGGVORBIS);

                    // Freeze the application until the audio clip is loaded fully.
                    while (!(audio.loadState == AudioDataLoadState.Loaded))
                        System.Threading.Thread.Sleep(1);

                    // Add the loaded audio to our array of audio clips.
                    clips[jingleIndex] = audio;
                }
            }

            // Return our array.
            return clips;
        }

        /// <summary>
        /// Makes the game actually play the Invincibility jingle.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemBox), "Action_Invincibility")]
        public static void PlayInvincibilityMusic()
        {
            // Stop any actively playing jingles.
            FPAudio.StopJingle();

            // Tell the game to play the Invincibility jingle.
            FPAudio.PlayJingle(FPAudio.JINGLE_INVINCIBILITY);
        }

        /// <summary>
        /// Converts a file path to a URL so that Unity's audio loader can get it, taken from https://github.com/Kuborros/MusicReplacer/blob/master/MusicReplacer/Plugin.cs#L30.
        /// </summary>
        /// <param name="filePath">The path to the file we're wanting to load.</param>
        /// <returns>The "URL" of the file.</returns>
        private static string FilePathToFileUrl(string filePath)
        {
            StringBuilder uri = new();
            foreach (char v in filePath)
            {
                if ((v >= 'a' && v <= 'z') || (v >= 'A' && v <= 'Z') || (v >= '0' && v <= '9') ||
                  v == '+' || v == '/' || v == ':' || v == '.' || v == '-' || v == '_' || v == '~' ||
                  v > '\xFF')
                {
                    uri.Append(v);
                }
                else if (v == Path.DirectorySeparatorChar || v == Path.AltDirectorySeparatorChar)
                {
                    uri.Append('/');
                }
                else
                {
                    uri.Append(string.Format("%{0:X2}", (int)v));
                }
            }
            if (uri.Length >= 2 && uri[0] == '/' && uri[1] == '/') // UNC path
                uri.Insert(0, "file:");
            else
                uri.Insert(0, "file:///");
            return uri.ToString();
        }
    }
}
