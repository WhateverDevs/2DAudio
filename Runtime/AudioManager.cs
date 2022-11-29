using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;
using Zenject;

#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif

#if WHATEVERDEVS_2DAUDIO_DOTWEEN
using DG.Tweening;
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
        /// Dictionary used to store the original volumes of each audio source when they are muted.
        /// </summary>
        private Dictionary<AudioSource, float> originalVolumes;

        /// <summary>
        /// Check if an audio is available to play.
        /// </summary>
        /// <param name="audioReference"></param>
        /// <returns></returns>
        public bool IsAudioAvailable(AudioReference audioReference) => audioLibrary.IsAudioAvailable(audioReference);

        #if !WHATEVERDEVS_2DAUDIO_DOTWEEN
        // ReSharper disable once InvalidXmlDocComment
        #endif
        /// <summary>
        /// Play an audio once.
        /// </summary>
        /// <param name="audioReference">The audio to play.</param>
        /// <param name="loop">Loop the audio?</param>
        /// <param name="pitch">Pitch of the audio. This affects both pitch and tempo.</param>
        /// <param name="volume">Volume of the audio.</param>
        /// <param name="fadeTime">If DoTween integration is available, set a time for the audio to fade in.</param>
        #if ODIN_INSPECTOR_3
        [FoldoutGroup("Debug")]
        [Button]
        #endif
        public void PlayAudio(AudioReference audioReference,
                              bool loop = false,
                              float pitch = 1,
                              float volume = 1
            #if WHATEVERDEVS_2DAUDIO_DOTWEEN
                              ,
                              float fadeTime = 0
            #endif
        ) =>
            audioLibrary.GetAudioAsset(audioReference,
                                       (success, clip, group) =>
                                       {
                                           if (!success) return;

                                           AudioSource audioSource = GetFreeAudioSource();

                                           audioSource.outputAudioMixerGroup = group;
                                           audioSource.clip = clip;
                                           audioSource.loop = loop;
                                           audioSource.pitch = pitch;
                                           #if !WHATEVERDEVS_2DAUDIO_DOTWEEN
                                           audioSource.volume = volume;
                                           #else
                                           audioSource.volume = 0;
                                           #endif
                                           audioSource.Play();
                                           #if WHATEVERDEVS_2DAUDIO_DOTWEEN
                                           audioSource.DOFade(volume, fadeTime);
                                           #endif
                                       });

        /// <summary>
        /// Check if an audio is playing.
        /// </summary>
        /// <param name="audioReference">Audio to check if its playing.</param>
        /// <param name="result">Event returning true if it is playing.</param>
        public IEnumerator IsAudioPlaying(AudioReference audioReference, Action<bool> result)
        {
            bool finished = false;

            audioLibrary.GetAudioAsset(audioReference,
                                       (success, clip, _) =>
                                       {
                                           if (!success) result.Invoke(false);

                                           result.Invoke(audioSourcePool.Any(source => source.clip == clip));

                                           finished = true;
                                       });

            yield return new WaitUntil(() => finished);
        }

        #if !WHATEVERDEVS_2DAUDIO_DOTWEEN
        // ReSharper disable once InvalidXmlDocComment
        #endif
        /// <summary>
        /// Stop the given audio if it's playing.
        /// </summary>
        /// <param name="audioReference">Audio to stop.</param>
        /// <param name="fadeTime">If DoTween integration is available, set a time for the audio to fade out.</param>
        #if ODIN_INSPECTOR_3
        [FoldoutGroup("Debug")]
        [Button]
        #endif
        public void StopAudio(AudioReference audioReference
            #if WHATEVERDEVS_2DAUDIO_DOTWEEN
                              ,
                              float fadeTime = 0
            #endif
        ) =>
            audioLibrary.GetAudioAsset(audioReference,
                                       (success, clip, _) =>
                                       {
                                           if (!success) return;

                                           for (int i = 0; i < audioSourcePool.Count; ++i)
                                               if (audioSourcePool[i].clip == clip)
                                               {
                                                   #if WHATEVERDEVS_2DAUDIO_DOTWEEN
                                                   int index = i;

                                                   audioSourcePool[i]
                                                      .DOFade(0, fadeTime)
                                                      .OnComplete(() => audioSourcePool[index].Stop());
                                                   #else
                                                   audioSourcePool[i].Stop();
                                                   #endif
                                               }
                                       });

        #if !WHATEVERDEVS_2DAUDIO_DOTWEEN
        // ReSharper disable once InvalidXmlDocComment
        #endif
        /// <summary>
        /// Unmute all the audios without stopping them.
        /// </summary>
        /// <param name="fadeTime">If DoTween integration is available, set a time for the audios to fade in.</param>
        [FoldoutGroup("Debug")]
        [Button("Unmute All")]
        private void TestUnmuteAllAudios(
            #if WHATEVERDEVS_2DAUDIO_DOTWEEN
            float fadeTime = 0
            #endif
        ) =>
            StartCoroutine(UnmuteAllAudios(
                                           #if WHATEVERDEVS_2DAUDIO_DOTWEEN
                                            fadeTime
                                           #endif
                                          ));

        #if !WHATEVERDEVS_2DAUDIO_DOTWEEN
        // ReSharper disable once InvalidXmlDocComment
        #endif
        /// <summary>
        /// Unmute all the audios without stopping them.
        /// </summary>
        /// <param name="fadeTime">If DoTween integration is available, set a time for the audios to fade in.</param>
        public IEnumerator UnmuteAllAudios(
            #if WHATEVERDEVS_2DAUDIO_DOTWEEN
            float fadeTime = 0
            #endif
        )
        {
            foreach (AudioSource audioSource in audioSourcePool.Where(audioSource => audioSource.isPlaying))
            {
                #if WHATEVERDEVS_2DAUDIO_DOTWEEN
                audioSource.DOFade(originalVolumes[audioSource], fadeTime);
                #else
                audioSource.volume = originalVolumes[audioSource];
                #endif
            }

            #if WHATEVERDEVS_2DAUDIO_DOTWEEN
            yield return new WaitForSeconds(fadeTime);
            #else
            yield break;
            #endif
        }

        #if !WHATEVERDEVS_2DAUDIO_DOTWEEN
        // ReSharper disable once InvalidXmlDocComment
        #endif
        /// <summary>
        /// Mute all the audios without stopping them.
        /// </summary>
        /// <param name="fadeTime">If DoTween integration is available, set a time for the audios to fade out.</param>
        [FoldoutGroup("Debug")]
        [Button("Mute All")]
        private void TestMuteAllAudios(
            #if WHATEVERDEVS_2DAUDIO_DOTWEEN
            float fadeTime = 0
            #endif
        ) =>
            StartCoroutine(MuteAllAudios(
                                         #if WHATEVERDEVS_2DAUDIO_DOTWEEN
                                            fadeTime
                                         #endif
                                        ));

        #if !WHATEVERDEVS_2DAUDIO_DOTWEEN
        // ReSharper disable once InvalidXmlDocComment
        #endif
        /// <summary>
        /// Mute all the audios without stopping them.
        /// </summary>
        /// <param name="fadeTime">If DoTween integration is available, set a time for the audios to fade out.</param>
        public IEnumerator MuteAllAudios(
            #if WHATEVERDEVS_2DAUDIO_DOTWEEN
            float fadeTime = 0
            #endif
        )
        {
            originalVolumes = new Dictionary<AudioSource, float>();

            foreach (AudioSource audioSource in audioSourcePool.Where(audioSource => audioSource.isPlaying))
            {
                originalVolumes[audioSource] = audioSource.volume;

                #if WHATEVERDEVS_2DAUDIO_DOTWEEN
                audioSource.DOFade(0, fadeTime);
                #else
                audioSource.volume = 0;
                #endif
            }

            #if WHATEVERDEVS_2DAUDIO_DOTWEEN
            yield return new WaitForSeconds(fadeTime);
            #else
            yield break;
            #endif
        }

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