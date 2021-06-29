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
    }
}