using System.IO;
using UnityEditor;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.TwoDAudio.Runtime;

namespace WhateverDevs.TwoDAudio.Editor
{
    /// <summary>
    /// Create the audio library from a menu option.
    /// </summary>
    public class AudioLibraryCreator : Loggable<AudioLibraryCreator>
    {
        /// <summary>
        /// Path to the data folder.
        /// </summary>
        private const string DataPath = "Assets/Data";

        /// <summary>
        /// Path to the audio folder.
        /// </summary>
        private const string AudioDataPath = DataPath + "/Audio";

        /// <summary>
        /// Path to the asset.
        /// </summary>
        public const string AudioLibraryPath = AudioDataPath + "/2DAudioLibrary.asset";

        /// <summary>
        /// Create the manager.
        /// </summary>
        [MenuItem("WhateverDevs/2D Audio/Create Library")]
        public static void CreateSceneManager()
        {
            if (!Directory.Exists(DataPath)) Directory.CreateDirectory(DataPath);
            if (!Directory.Exists(AudioDataPath)) Directory.CreateDirectory(AudioDataPath);

            if (File.Exists(AudioLibraryPath))
                StaticLogger.Error("Audio library already exists!");
            else
            {
                AudioLibrary audioLibrary = (AudioLibrary) ScriptableObject.CreateInstance(typeof(AudioLibrary));
                AssetDatabase.CreateAsset(audioLibrary, AudioLibraryPath);
                AssetDatabase.SaveAssets();
                StaticLogger.Info("Created Audio Library at " + AudioLibraryPath + ".");
            }
        }
    }
}