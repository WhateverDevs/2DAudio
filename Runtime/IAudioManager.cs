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

        /// <summary>
        /// Play an audio once.
        /// </summary>
        /// <param name="audioReference">The audio to play.</param>
        /// <param name="loop">Loop the audio?</param>
        /// <param name="pitch">Pitch of the audio. This affects both pitch and tempo.</param>
        /// <param name="volume">Volume of the audio.</param>
        void PlayAudio(AudioReference audioReference, bool loop = false, float pitch = 1, float volume = 1);

        /// <summary>
        /// Stop the given audio if it's playing.
        /// </summary>
        /// <param name="audioReference"></param>
        void StopAudio(AudioReference audioReference);

        /// <summary>
        /// Stop all audios, probably only used in abort situations.
        /// </summary>
        void StopAllAudios();
    }
}