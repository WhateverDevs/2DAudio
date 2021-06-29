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
        /// Check if an audio is available to play.
        /// </summary>
        /// <param name="audioReference"></param>
        /// <returns></returns>
        public bool IsAudioAvailable(AudioReference audioReference) =>
            audioLibrary.IsAudioAvailable(audioReference.Audio);
    }
}