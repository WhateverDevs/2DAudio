using UnityEditor;
using WhateverDevs.Core.Editor.Utils;
using WhateverDevs.Core.Runtime.Common;

namespace WhateverDevs.TwoDAudio.Editor
{
    /// <summary>
    /// Class to enable or disable the DoTween integration in the audio library.
    /// </summary>
    public class DoTweenIntegrationEnabler : Loggable<DoTweenIntegrationEnabler>
    {
        /// <summary>
        /// Enable or disable the integration.
        /// </summary>
        #if WHATEVERDEVS_2DAUDIO_DOTWEEN
        [MenuItem("WhateverDevs/2D Audio/Disable DoTween Integration")]
        #else
        [MenuItem("WhateverDevs/2D Audio/Enable DoTween Integration")]
        #endif
        public static void ToggleIntegration()
        {
            // ReSharper disable once StringLiteralTypo
            #if WHATEVERDEVS_2DAUDIO_DOTWEEN
            ScriptingDefines.SetDefine("WHATEVERDEVS_2DAUDIO_DOTWEEN", false);
            #else
            ScriptingDefines.SetDefine("WHATEVERDEVS_2DAUDIO_DOTWEEN", true);
            #endif
        }
    }
}