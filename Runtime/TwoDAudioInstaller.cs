using UnityEngine;
using UnityEngine.Audio;
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
        [SerializeField]
        private AudioLibrary AudioLibrary;

        /// <summary>
        /// Main mixer group.
        /// </summary>
        [SerializeField]
        private AudioMixer MainMixer;

        /// <summary>
        /// Inject the references.
        /// </summary>
        public override void InstallBindings()
        {
            Container.QueueForInject(AudioLibrary);

            IAudioManager audioManager = AudioManager.Instance;
            
            Container.QueueForInject(audioManager);

            Container.Bind<AudioMixer>().FromInstance(MainMixer).WhenInjectedInto<IAudioManager>().Lazy();

            Container.Bind<IAudioLibrary>().FromInstance(AudioLibrary).AsSingle().Lazy();

            Container.Bind<IAudioManager>().FromInstance(audioManager).AsSingle().Lazy();
        }
    }
}