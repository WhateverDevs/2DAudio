using System;
using System.Collections.Generic;
using System.Linq;
#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif
#if UNITY_EDITOR
using UnityEditor;
using WhateverDevs.Core.Editor.Utils;
#endif

namespace WhateverDevs.TwoDAudio.Runtime
{
    /// <summary>
    /// Data class to store the reference to an audio.
    /// </summary>
    [Serializable]
    [InlineProperty]
    public struct AudioReference
    {
        /// <summary>
        /// Audio string that matches the audio
        /// </summary>
        #if ODIN_INSPECTOR_3
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAvailableAudios))]
        #endif
        [Searchable]
        [InfoBox("Library hasn't been found!", InfoMessageType.Error, VisibleIf = nameof(libraryNotFound))]
        [DisableIf(nameof(libraryNotFound))]
        [HideLabel]
        #endif
        public string Audio;

        /// <summary>
        /// Flag to mark the library not found error.
        /// </summary>
        private bool libraryNotFound;

        #if UNITY_EDITOR

        /// <summary>
        /// Retrieve all available audios.
        /// </summary>
        /// <returns>A list of all available audio names.</returns>
        private List<string> GetAvailableAudios()
        {
            AudioLibrary library;

            EditorUtility.DisplayProgressBar("2D Audio", "Looking for audio library...", .5f);

            try
            {
                library = AssetManagementUtils.FindAssetsByType<AudioLibrary>().First();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            libraryNotFound = library == null;

            return libraryNotFound ? null : library.GetAllAudioNames();
        }

        #endif
    }
}