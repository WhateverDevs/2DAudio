using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using WhateverDevs.Core.Runtime.DataStructures;

namespace WhateverDevs.TwoDAudio.Runtime
{
    /// <summary>
    /// Data structure to support audio asset references and mixer groups into a single serializable list.
    /// We would normally use a Serializable Dictionary, but the drawer for the Asset Reference won't work for some reason.
    /// </summary>
    [Serializable]
    public class AudioAssetReferenceAudioMixerGroupPair : ObjectPair<AssetReferenceT<AudioClip>, AudioMixerGroup>
    {
    }
}