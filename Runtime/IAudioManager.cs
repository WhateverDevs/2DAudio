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
        void PlayAudio(AudioReference audioReference, bool loop = false);

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