using System.Collections.Generic;
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

        /// <summary>
        /// List of all the available audio sources this object has.
        /// </summary>
        private List<AudioSource> audioSourcePool;

        /// <summary>
        /// List with the availability of each audio source.
        /// </summary>
        private List<bool> audioSourceAvailability;

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
        /// <param name="loop">Loop the audio?</param>
        [FoldoutGroup("Debug")]
        [Button]
        public void PlayAudio(AudioReference audioReference, bool loop = false) =>
            audioLibrary.GetAudioAsset(audioReference,
                                       (success, clip, group) =>
                                       {
                                           if (!success) return;

                                           AudioSource audioSource = GetFreeAudioSource();

                                           audioSource.outputAudioMixerGroup = group;
                                           audioSource.clip = clip;
                                           audioSource.loop = loop;
                                           audioSource.Play();
                                       });

        /// <summary>
        /// Stop the given audio if it's playing.
        /// </summary>
        /// <param name="audioReference"></param>
        public void StopAudio(AudioReference audioReference) =>
            audioLibrary.GetAudioAsset(audioReference,
                                       (success, clip, group) =>
                                       {
                                           if (!success) return;

                                           for (int i = 0; i < audioSourcePool.Count; ++i)
                                               if (audioSourcePool[i].clip == clip)
                                               {
                                                   audioSourcePool[i].Stop();
                                               }
                                       });

        /// <summary>
        /// Stop all audios, probably only used in abort situations.
        /// </summary>
        [FoldoutGroup("Debug")]
        [Button]
        public void StopAllAudios()
        {
            for (int i = 0; i < audioSourcePool.Count; ++i) audioSourcePool[i].Stop();
        }

        /// <summary>
        /// Create the pools.
        /// </summary>
        private void OnEnable()
        {
            audioSourcePool = new List<AudioSource>();
            audioSourceAvailability = new List<bool>();
        }

        /// <summary>
        /// Check the availability of audio sources each frame.
        /// </summary>
        private void Update() => CheckAudioSourceAvailability();

        /// <summary>
        /// Get a free audio source to play a sound.
        /// </summary>
        /// <returns></returns>
        private AudioSource GetFreeAudioSource()
        {
            for (int i = 0; i < audioSourceAvailability.Count; ++i)
                if (audioSourceAvailability[i])
                {
                    audioSourceAvailability[i] = false;
                    return audioSourcePool[i];
                }

            audioSourcePool.Add(gameObject.AddComponent<AudioSource>());
            audioSourceAvailability.Add(false);
            return audioSourcePool[audioSourcePool.Count - 1];
        }

        /// <summary>
        /// Check each audio source and made those finished available.
        /// </summary>
        private void CheckAudioSourceAvailability()
        {
            for (int i = 0; i < audioSourcePool.Count; ++i)
            {
                if (audioSourceAvailability[i] || audioSourcePool[i].isPlaying) continue;
                audioSourcePool[i].clip = null;
                audioSourcePool[i].clip = null;
                audioSourcePool[i].loop = false;
                audioSourceAvailability[i] = true;
            }
        }
    }
}