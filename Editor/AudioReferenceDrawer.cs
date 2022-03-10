#if ODIN_INSPECTOR_3
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using WhateverDevs.Core.Editor.Utils;
using WhateverDevs.TwoDAudio.Runtime;

namespace WhateverDevs.TwoDAudio.Editor
{
    /// <summary>
    /// Custom drawer for audio references.
    /// </summary>
    [UsedImplicitly]
    public class AudioReferenceDrawer : OdinValueDrawer<AudioReference>
    {
        /// <summary>
        /// Reference to the audio library.
        /// </summary>
        private AudioLibrary library;

        /// <summary>
        /// Paint the property.
        /// </summary>
        /// <param name="label"></param>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            AudioReference reference = ValueEntry.SmartValue;

            if (library == null)
            {
                EditorUtility.DisplayProgressBar("2D Audio", "Looking for audio library...", .5f);

                try
                {
                    library = AssetManagementUtils.FindAssetsByType<AudioLibrary>().First();
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }

                if (library == null)
                    library =
                        (AudioLibrary)EditorGUILayout.ObjectField(new GUIContent("Audio library",
                                                                      "A reference to the audio library is needed."),
                                                                  library,
                                                                  typeof(AudioLibrary),
                                                                  false);
            }
            else if (library.GetAllAudioNames().Count == 0)
                EditorGUILayout.HelpBox("There are no audios in the library.", MessageType.Error);
            else
            {
                if (!library.GetAllAudioNames().Contains(reference.Audio))
                    reference.Audio = library.GetAllAudioNames()[0];

                reference.Audio =
                    library.GetAllAudioNames()
                        [EditorGUILayout.Popup(label,
                                               library.GetAllAudioNames().IndexOf(reference.Audio),
                                               library.GetAllAudioNames().ToArray())];
            }

            ValueEntry.SmartValue = reference;
        }
    }
}
#endif