using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.SceneManagement.Runtime.AddressableManagement;
using Zenject;

namespace WhateverDevs.TwoDAudio.Runtime
{
    /// <summary>
    /// Library that stores all the 2D audio files in the project.
    /// </summary>
    public class AudioLibrary : LoggableScriptableObject<AudioLibrary>, IAudioLibrary
    {
        /// <summary>
        /// List that holds all the 2D audios in the game and the mixer group they belong to.
        /// </summary>
        [SerializeField]
        private List<AudioAssetReferenceAudioMixerGroupPair> Audios;

        /// <summary>
        /// Cached list of the audio assets for easy handling.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private List<AssetReferenceT<AudioClip>> AudioAssets;

        /// <summary>
        /// Cached list of all audio names.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private List<string> AudioNames;

        /// <summary>
        /// Cached list of the groups on the same order as the audios.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private List<AudioMixerGroup> CachedGroups;

        /// <summary>
        /// Reference to the addressable manager.
        /// </summary>
        private IAddressableManager addressableManager;

        /// <summary>
        /// Flag to know if the library has been initialized.
        /// </summary>
        private bool initialized;

        /// <summary>
        /// Get the reference to the addressable manager and initialize.
        /// </summary>
        /// <param name="addressableManagerReference">Reference to the addressable manager.</param>
        [Inject]
        public void Construct(IAddressableManager addressableManagerReference)
        {
            initialized = false;
            addressableManager = addressableManagerReference;

            addressableManager.CheckAvailableAddressables(report => initialized = true);
        }

        /// <summary>
        /// Method to know if the library has been initialized.
        /// </summary>
        public bool IsInitialized() => initialized;

        /// <summary>
        /// Get a list of all the audio names available.
        /// </summary>
        /// <returns>A list of strings with the audio names.</returns>
        public List<string> GetAllAudioNames() => AudioNames.ShallowClone();

        /// <summary>
        /// Get a list of all audios.
        /// </summary>
        /// <returns></returns>
        public List<AssetReferenceT<AudioClip>> GetAllAudios() => AudioAssets.ShallowClone();

        /// <summary>
        /// Check if an audio is available to play.
        /// </summary>
        /// <param name="audioReference"></param>
        /// <returns></returns>
        public bool IsAudioAvailable(AudioReference audioReference) => IsAudioAvailable(audioReference.Audio);

        /// <summary>
        /// Check if an audio is available to play.
        /// </summary>
        /// <param name="audioName"></param>
        /// <returns></returns>
        public bool IsAudioAvailable(string audioName)
        {
            if (IsInitialized()) return addressableManager.IsAssetAvailable(NameToAssetReference(audioName));

            Logger.Error("Audio library not initialized yet!");
            return false;
        }

        /// <summary>
        /// Get the mixer group of an audio.
        /// </summary>
        /// <param name="audioReference"></param>
        /// <returns></returns>
        public AudioMixerGroup GetGroupForAudio(AudioReference audioReference) =>
            GetGroupForAudio(audioReference.Audio);

        /// <summary>
        /// Get the mixer group of an audio.
        /// </summary>
        /// <param name="audioName"></param>
        /// <returns></returns>
        public AudioMixerGroup GetGroupForAudio(string audioName) => GetGroupForAudio(NameToAssetReference(audioName));

        /// <summary>
        /// Get the mixer group of an audio.
        /// </summary>
        /// <param name="audio"></param>
        /// <returns></returns>
        public AudioMixerGroup GetGroupForAudio(AssetReferenceT<AudioClip> audio) =>
            CachedGroups[AudioAssets.IndexOf(audio)];

        /// <summary>
        /// Translate audio name to asset reference.
        /// </summary>
        /// <param name="audioName"></param>
        /// <returns></returns>
        private AssetReferenceT<AudioClip> NameToAssetReference(string audioName) =>
            AudioAssets[AudioNames.IndexOf(audioName)];

        #if UNITY_EDITOR

        /// <summary>
        /// Cache the audio names when the list is modified.
        /// </summary>
        [InfoBox("Remember clicking the button when you modify the list!")]
        [PropertyOrder(-1)]
        [Button]
        private void CacheAudioReferences()
        {
            AudioAssets = new List<AssetReferenceT<AudioClip>>();

            AudioNames = new List<string>();

            CachedGroups = new List<AudioMixerGroup>();

            for (int i = 0; i < Audios.Count; ++i)
                if (Audios[i] != null && Audios[i].Key.editorAsset != null)
                {
                    AudioAssets.Add(Audios[i].Key);
                    AudioNames.Add(AudioAssets[i].editorAsset.name);
                    CachedGroups.Add(Audios[i].Value);
                }
        }
        #endif
    }
}