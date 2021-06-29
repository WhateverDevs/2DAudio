using UnityEngine;
using Zenject;

namespace WhateverDevs.TwoDAudio.Runtime
{
    /// <summary>
    /// Installer for the 2D audio system.
    /// </summary>
    [CreateAssetMenu(menuName = "WhateverDevs/2DAudio/Installer", fileName = "2DAudioInstaller")]
    public class TwoDAudioInstaller : ScriptableObjectInstaller
    {
        /// <summary>
        /// Reference to the audio library.
        /// </summary>
        public AudioLibrary AudioLibrary;

        /// <summary>
        /// Inject the references.
        /// </summary>
        public override void InstallBindings()
        {
            Container.QueueForInject(AudioLibrary);

            Container.Bind<IAudioLibrary>().FromInstance(AudioLibrary).AsSingle().Lazy();
        }
    }
}