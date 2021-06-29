using UnityEngine;
using WhateverDevs.Core.Runtime.Common;

namespace WhateverDevs.TwoDAudio.Runtime
{
    /// <summary>
    /// Library that stores all the 2D audio files in the project.
    /// </summary>
    [CreateAssetMenu(menuName = "WhateverDevs/2DAudio/Library", fileName = "2DAudioLibrary")]
    public class AudioLibrary : LoggableScriptableObject<AudioLibrary>
    {
    }
}