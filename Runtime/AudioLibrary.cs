using System;
using System.Collections;
using System.Collections.Generic;
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
        /// Seconds to wait until free audios are unloaded from RAM.
        /// </summary>
        [Tooltip("Seconds to wait until free audios are unloaded from RAM.")]
        [FoldoutGroup("Configuration")]
        [HideInPlayMode]
        public float SecondsToUnLoadFreeAudiosFromRam = 120f;

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
        /// Dictionary with the audios that are loaded and not in use and the time they haven't been in use.
        /// </summary>
        [SerializeField]
        [ReadOnly]
        [HideInEditorMode]
        private SerializableDictionary<string, float> FreeAudios;

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

            FreeAudios = new SerializableDictionary<string, float>();

            AppEventsListener.Instance.AppUpdate += CheckFreeAudiosAndUnload;

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
        public bool IsAudioAvailable(string audioName) => IsAudioAvailable(NameToAssetReference(audioName));

        /// <summary>
        /// Check if an audio is available to play.
        /// </summary>
        /// <param name="audio"></param>
        /// <returns></returns>
        public bool IsAudioAvailable(AssetReferenceT<AudioClip> audio)
        {
            if (IsInitialized()) return addressableManager.IsAssetAvailable(audio);

            Logger.Error("Audio library not initialized yet!");
            return false;
        }

        /// <summary>
        /// Get an audio asset and group from its reference.
        /// </summary>
        /// <param name="audioReference"></param>
        /// <param name="callback">Callback with bool of success, the audio clip to play and its mixer group.</param>
        public void GetAudioAsset(AudioReference audioReference, Action<bool, AudioClip, AudioMixerGroup> callback) =>
            GetAudioAsset(audioReference.Audio, callback);

        /// <summary>
        /// Get an audio asset and group from its name.
        /// </summary>
        /// <param name="audioName"></param>
        /// <param name="callback">Callback with bool of success, the audio clip to play and its mixer group.</param>
        public void GetAudioAsset(string audioName, Action<bool, AudioClip, AudioMixerGroup> callback) =>
            GetAudioAsset(NameToAssetReference(audioName), callback);

        /// <summary>
        /// Get an audio asset and group from its asset reference.
        /// </summary>
        /// <param name="audio"></param>
        /// <param name="callback">Callback with bool of success, the audio clip to play and its mixer group.</param>
        public void
            GetAudioAsset(AssetReferenceT<AudioClip> audio, Action<bool, AudioClip, AudioMixerGroup> callback) =>
            CoroutineRunner.Instance.StartCoroutine(GetAudioAssetRoutine(audio, callback));

        /// <summary>
        /// Get an audio asset and group from its asset reference.
        /// </summary>
        /// <param name="audio"></param>
        /// <param name="callback">Callback with bool of success, the audio clip to play and its mixer group.</param>
        private IEnumerator GetAudioAssetRoutine(AssetReferenceT<AudioClip> audio,
                                                 Action<bool, AudioClip, AudioMixerGroup> callback)
        {
            if (!initialized) yield return new WaitUntil(() => initialized);

            if (!IsAudioAvailable(audio)) callback?.Invoke(false, null, null);

            if (audio.Asset == null)
            {
                if (!audio.OperationHandle.IsValid())
                    yield return audio.LoadAssetAsync();
                else
                    yield return new WaitUntil(() => audio.OperationHandle.IsDone);
            }

            if (FreeAudios.ContainsKey(audio.Asset.name)) FreeAudios.Remove(audio.Asset.name);

            callback?.Invoke(true, (AudioClip) audio.Asset, GetGroupForAudio(audio));
        }

        /// <summary>
        /// Used to inform that an audio asset is no longer in use.
        /// </summary>
        /// <param name="audioReference">The audio reference to free.</param>
        public void FreeAudioAsset(AudioReference audioReference) => FreeAudioAsset(audioReference.Audio);

        /// <summary>
        /// Used to inform that an audio asset is no longer in use.
        /// </summary>
        /// <param name="audioName">The audio name to free.</param>
        public void FreeAudioAsset(string audioName)
        {
            if (!FreeAudios.ContainsKey(audioName)) FreeAudios[audioName] = 0;
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

        /// <summary>
        /// Check the available audios and unload if they've too much time without being used.
        /// </summary>
        /// <param name="deltaTime"></param>
        private void CheckFreeAudiosAndUnload(float deltaTime)
        {
            List<string> audiosToUnload = new List<string>();

            foreach (KeyValuePair<string, float> keyValuePair in FreeAudios)
            {
                FreeAudios[keyValuePair.Key] += deltaTime;

                if (FreeAudios[keyValuePair.Key] > SecondsToUnLoadFreeAudiosFromRam)
                    audiosToUnload.Add(keyValuePair.Key);
            }

            for (int i = 0; i < audiosToUnload.Count; ++i)
            {
                FreeAudios.Remove(audiosToUnload[i]);

                NameToAssetReference(audiosToUnload[i]).ReleaseAsset();
            }
        }

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