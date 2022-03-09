using UnityEditor;
using WhateverDevs.Core.Editor.Utils;
using WhateverDevs.Core.Runtime.Common;

namespace WhateverDevs.TwoDAudio.Editor
{
    /// <summary>
    /// Class to enable or disable the addressable mode in the audio library.
    /// </summary>
    public class AddressablesEnabler : Loggable<AddressablesEnabler>
    {
        /// <summary>
        /// Enable or disable the addressables.
        /// </summary>
        #if WHATEVERDEVS_2DAUDIO_ADDRESSABLES
        [MenuItem("WhateverDevs/2D Audio/Disable Addressables")]
        #else
        [MenuItem("WhateverDevs/2D Audio/Enable Addressables")]
        #endif
        public static void ToggleAddressables()
        {
            // ReSharper disable once StringLiteralTypo
            #if WHATEVERDEVS_2DAUDIO_ADDRESSABLES
            ScriptingDefines.SetDefine("WHATEVERDEVS_2DAUDIO_ADDRESSABLES", false);
            #else
            ScriptingDefines.SetDefine("WHATEVERDEVS_2DAUDIO_ADDRESSABLES", true);
            #endif
        }
    }
}