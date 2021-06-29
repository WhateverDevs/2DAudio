using System;

namespace WhateverDevs.TwoDAudio.Runtime
{
    /// <summary>
    /// Data class to store the reference to an audio.
    /// </summary>
    [Serializable]
    public struct AudioReference
    {
        /// <summary>
        /// Audio string that matches the audio
        /// </summary>
        public string Audio;
    }
}