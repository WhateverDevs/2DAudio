using System.Collections.Generic;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;
using Zenject;

#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif

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
        /// <param name="pitch">Pitch of the audio. This affects both pitch and tempo.</param>
        /// <param name="volume">Volume of the audio.</param>
        #if ODIN_INSPECTOR_3
        [FoldoutGroup("Debug")]
        [Button]
        #endif
        public void PlayAudio(AudioReference audioReference, bool loop = false, float pitch = 1, float volume = 1) =>
            audioLibrary.GetAudioAsset(audioReference,
                                       (success, clip, group) =>
                                       {
                                           if (!success) return;

                                           AudioSource audioSource = GetFreeAudioSource();

                                           audioSource.outputAudioMixerGroup = group;
                                           audioSource.clip = clip;
                                           audioSource.loop = loop;
                                           audioSource.pitch = pitch;
                                           audioSource.volume = volume;
                                           audioSource.Play();
                                       });

        /// <summary>
        /// Stop the given audio if it's playing.
        /// </summary>
        /// <param name="audioReference"></param>
        public void StopAudio(AudioReference audioReference) =>
            audioLibrary.GetAudioAsset(audioReference,
                                       (success, clip, _) =>
                                       {
                                           if (!success) return;

                                           for (int i = 0; i < audioSourcePool.Count; ++i)
                                               if (audioSourcePool[i].clip == clip)
                                                   audioSourcePool[i].Stop();
                                       });

        /// <summary>
        /// Stop all audios, probably only used in abort situations.
        /// </summary>
        #if ODIN_INSPECTOR_3
        [FoldoutGroup("Debug")]
        [Button]
        #endif
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
            return audioSourcePool[^1];
        }

        /// <summary>
        /// Check each audio source and made those finished available.
        /// </summary>
        private void CheckAudioSourceAvailability()
        {
            List<AudioClip> clipsToFree = new List<AudioClip>();

            for (int i = 0; i < audioSourcePool.Count; ++i)
            {
                if (audioSourceAvailability[i] || audioSourcePool[i].isPlaying) continue;

                clipsToFree.Add(audioSourcePool[i].clip);

                audioSourcePool[i].clip = null;
                audioSourcePool[i].outputAudioMixerGroup = null;
                audioSourcePool[i].loop = false;
                audioSourceAvailability[i] = true;
            }

            for (int i = 0; i < clipsToFree.Count; ++i)
            {
                bool canFree = true;

                for (int j = 0; j < audioSourcePool.Count; ++j)
                    if (audioSourcePool[j].clip == clipsToFree[i])
                        canFree = false;

                if (canFree) audioLibrary.FreeAudioAsset(clipsToFree[i].name);
            }
        }
    }
}