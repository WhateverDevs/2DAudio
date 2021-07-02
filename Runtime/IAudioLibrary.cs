using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

namespace WhateverDevs.TwoDAudio.Runtime
{
    /// <summary>
    /// Interface that defines how the audio library should work.
    /// </summary>
    public interface IAudioLibrary
    {
        /// <summary>
        /// Method to know if the library has been initialized.
        /// </summary>
        bool IsInitialized();
        
        /// <summary>
        /// Get a list of all the audio names available.
        /// </summary>
        /// <returns>A list of strings with the audio names.</returns>
        List<string> GetAllAudioNames();

        /// <summary>
        /// Get a list of all audios.
        /// </summary>
        /// <returns></returns>
        List<AssetReferenceT<AudioClip>> GetAllAudios();
        
        /// <summary>
        /// Check if an audio is available to play.
        /// </summary>
        /// <param name="audioReference"></param>
        /// <returns></returns>
        bool IsAudioAvailable(AudioReference audioReference);

        /// <summary>
        /// Check if an audio is available to play.
        /// </summary>
        /// <param name="audioName"></param>
        /// <returns></returns>
        bool IsAudioAvailable(string audioName);
        
        /// <summary>
        /// Check if an audio is available to play.
        /// </summary>
        /// <param name="audio"></param>
        /// <returns></returns>
        bool IsAudioAvailable(AssetReferenceT<AudioClip> audio);

        /// <summary>
        /// Get an audio asset and group from its reference.
        /// </summary>
        /// <param name="audioReference"></param>
        /// <param name="callback">Callback with bool of success, the audio clip to play and its mixer group.</param>
        void GetAudioAsset(AudioReference audioReference, Action<bool, AudioClip, AudioMixerGroup> callback);
        
        /// <summary>
        /// Get an audio asset and group from its name.
        /// </summary>
        /// <param name="audioName"></param>
        /// <param name="callback">Callback with bool of success, the audio clip to play and its mixer group.</param>
        void GetAudioAsset(string audioName, Action<bool, AudioClip, AudioMixerGroup> callback);
        
        /// <summary>
        /// Get an audio asset and group from its asset reference.
        /// </summary>
        /// <param name="audio"></param>
        /// <param name="callback">Callback with bool of success, the audio clip to play and its mixer group.</param>
        void GetAudioAsset(AssetReferenceT<AudioClip> audio, Action<bool, AudioClip, AudioMixerGroup> callback);

        /// <summary>
        /// Used to inform that an audio asset is no longer in use.
        /// </summary>
        /// <param name="audioReference">The audio reference to free.</param>
        void FreeAudioAsset(AudioReference audioReference);
        
        /// <summary>
        /// Used to inform that an audio asset is no longer in use.
        /// </summary>
        /// <param name="audioName">The audio name to free.</param>
        void FreeAudioAsset(string audioName);

        /// <summary>
        /// Get the mixer group of an audio.
        /// </summary>
        /// <param name="audioReference"></param>
        /// <returns></returns>
        AudioMixerGroup GetGroupForAudio(AudioReference audioReference);

        /// <summary>
        /// Get the mixer group of an audio.
        /// </summary>
        /// <param name="audioName"></param>
        /// <returns></returns>
        AudioMixerGroup GetGroupForAudio(string audioName);
        
        /// <summary>
        /// Get the mixer group of an audio.
        /// </summary>
        /// <param name="audio"></param>
        /// <returns></returns>
        AudioMixerGroup GetGroupForAudio(AssetReferenceT<AudioClip> audio);
    }
}