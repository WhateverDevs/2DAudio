using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
        [InfoBox("Library hasn't been found!", InfoMessageType.Error, VisibleIf = "@AudioLibrary == null")]
        [DisableIf("@AudioLibrary == null")]
        [HideLabel]
        #endif
        public string Audio;

        /// <summary>
        /// Reference to the audio library to be cached in editor mode.
        /// </summary>
        [HideInInspector]
        [SerializeField]
        private AudioLibrary AudioLibrary;

        #if UNITY_EDITOR

        /// <summary>
        /// Retrieve all available audios.
        /// </summary>
        /// <returns>A list of all available audio names.</returns>
        private List<string> GetAvailableAudios()
        {
            // ReSharper disable once InvertIf
            if (AudioLibrary == null)
            {
                EditorUtility.DisplayProgressBar("2D Audio", "Looking for audio library...", .5f);

                try
                {
                    AudioLibrary = AssetManagementUtils.FindAssetsByType<AudioLibrary>().First();
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }
            }

            return AudioLibrary == null ? null : AudioLibrary.GetAllAudioNames();
        }

        #endif
    }
}