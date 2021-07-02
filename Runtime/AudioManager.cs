using Sirenix.OdinInspector;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;
using Zenject;

namespace WhateverDevs.TwoDAudio.Runtime
{
    /// <summary>
    /// Manager class that will have the audio sources to play the 2D audio and
    /// that other classes will call to play specific audios.
    /// </summary>
    public class AudioManager : Singleton<AudioManager>, IAudioManager
    {
        /// <summary>
        /// Reference to the audio library.
        /// </summary>
        [Inject]
        private IAudioLibrary audioLibrary;

        public AudioSource temp;

        /// <summary>
        /// Check if an audio is available to play.
        /// </summary>
        /// <param name="audioReference"></param>
        /// <returns></returns>
        public bool IsAudioAvailable(AudioReference audioReference) => audioLibrary.IsAudioAvailable(audioReference);

        /// <summary>
        /// Play an audio once.
        /// </summary>
        /// <param name="audioReference">The audio to play.</param>
        [FoldoutGroup("Debug")]
        [Button]
        public void PlayAudioOnce(AudioReference audioReference)
        {
            audioLibrary.GetAudioAsset(audioReference,
                                       (success, clip, group) =>
                                       {
                                           if (!success) return;

                                           temp.outputAudioMixerGroup = group;
                                           temp.clip = clip;
                                           temp.Play();
                                       });
        }
    }
}