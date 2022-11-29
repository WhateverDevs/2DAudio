using System;
using System.Collections;
using UnityEngine;

namespace WhateverDevs.TwoDAudio.Runtime
{
    /// <summary>
    /// Interface that defines the class that other classes will call to play specific 2D audios.
    /// </summary>
    public interface IAudioManager
    {
        /// <summary>
        /// Check if an audio is available to play.
        /// </summary>
        /// <param name="audioReference"></param>
        /// <returns></returns>
        bool IsAudioAvailable(AudioReference audioReference);

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
        void PlayAudio(AudioReference audioReference,
                       bool loop = false,
                       float pitch = 1,
                       float volume = 1
                       #if WHATEVERDEVS_2DAUDIO_DOTWEEN
                       ,
                       float fadeTime = 0
            #endif
        );

        /// <summary>
        /// Check if an audio is playing.
        /// </summary>
        /// <param name="audioReference">Audio to check if its playing.</param>
        /// <param name="result">Event returning true if it is playing.</param>
        IEnumerator IsAudioPlaying(AudioReference audioReference, Action<bool> result);

        #if !WHATEVERDEVS_2DAUDIO_DOTWEEN
        // ReSharper disable once InvalidXmlDocComment
        #endif
        /// <summary>
        /// Stop the given audio if it's playing.
        /// </summary>
        /// <param name="audioReference">Audio to stop.</param>
        /// <param name="fadeTime">If DoTween integration is available, set a time for the audio to fade out.</param>
        void StopAudio(AudioReference audioReference
                       #if WHATEVERDEVS_2DAUDIO_DOTWEEN
                       ,
                       float fadeTime = 0
            #endif
        );

        #if !WHATEVERDEVS_2DAUDIO_DOTWEEN
        // ReSharper disable once InvalidXmlDocComment
        #endif
        /// <summary>
        /// Unmute all the audios without stopping them.
        /// </summary>
        /// <param name="fadeTime">If DoTween integration is available, set a time for the audios to fade in.</param>
        IEnumerator UnmuteAllAudios(
            #if WHATEVERDEVS_2DAUDIO_DOTWEEN
            float fadeTime = 0
            #endif
        );

        #if !WHATEVERDEVS_2DAUDIO_DOTWEEN
        // ReSharper disable once InvalidXmlDocComment
        #endif
        /// <summary>
        /// Mute all the audios without stopping them.
        /// </summary>
        /// <param name="fadeTime">If DoTween integration is available, set a time for the audios to fade out.</param>
        IEnumerator MuteAllAudios(
            #if WHATEVERDEVS_2DAUDIO_DOTWEEN
            float fadeTime = 0
            #endif
        );

        /// <summary>
        /// Stop all audios, probably only used in abort situations.
        /// </summary>
        void StopAllAudios();

        /// <summary>
        /// We perceive volume as a logarithmic value, so we need to convert it.
        /// We clamp the minimum to 0.0001f because logarithms with 0s are no good.
        /// Credits to: https://gamedevbeginner.com/the-right-way-to-make-a-volume-slider-in-unity-using-logarithmic-conversion/
        /// </summary>
        /// <param name="linearVolume"></param>
        public static float LinearVolumeToLogarithmicVolume(float linearVolume) =>
            Mathf.Log10(Mathf.Clamp(linearVolume, 0.0001f, 1)) * 20;

        /// <summary>
        /// We perceive volume as a logarithmic value, so we need to convert it.
        /// We clamp the minimum to 0.0001f because logarithms with 0s are no good.
        /// Credits to: https://gamedevbeginner.com/the-right-way-to-make-a-volume-slider-in-unity-using-logarithmic-conversion/
        /// </summary>
        /// <param name="logarithmicVolume"></param>
        public static float LogarithmicVolumeToLinearVolume(float logarithmicVolume) =>
            Mathf.Pow(10, logarithmicVolume / 20f);
    }
}