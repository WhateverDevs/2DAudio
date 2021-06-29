using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using WhateverDevs.Core.Runtime.Common;
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
        /// List that holds all the 2D audios in the game.
        /// </summary>
        [SerializeField]
        private List<AssetReferenceT<AudioClip>> Audios;

        /// <summary>
        /// Cached list of all audio names.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private List<string> AudioNames;

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
        public List<AssetReferenceT<AudioClip>> GetAllAudios() => Audios.ShallowClone();

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
        /// Translate audio name to asset reference.
        /// </summary>
        /// <param name="audioName"></param>
        /// <returns></returns>
        private AssetReferenceT<AudioClip> NameToAssetReference(string audioName) =>
            Audios[AudioNames.IndexOf(audioName)];

        #if UNITY_EDITOR

        /// <summary>
        /// Cache the audio names when the list is modified.
        /// </summary>
        [InfoBox("Remember clicking the button when you modify the list!")]
        [PropertyOrder(-1)]
        [Button]
        private void CacheAudioNames()
        {
            AudioNames = new List<string>();

            for (int i = 0; i < Audios.Count; ++i)
                if (Audios[i] != null && Audios[i].editorAsset != null)
                    AudioNames.Add(Audios[i].editorAsset.name);
        }
        #endif
    }
}