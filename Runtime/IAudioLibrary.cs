using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
        /// <param name="audioName"></param>
        /// <returns></returns>
        bool IsAudioAvailable(string audioName);
    }
}