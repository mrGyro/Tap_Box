using System;
using System.Collections.Generic;
using Core.MessengerStatic;
using Unity.VisualScripting;
using UnityEngine;

namespace Sounds
{
    public class SoundController : MonoBehaviour, IInitializable
    {
        [SerializeField] private AudioListener _audioListener;
        [SerializeField] private List<SoundData> _soundData;

        public void Initialize()
        {
            Messenger<ClipDataMessage>.AddListener(Constants.Events.OnPlaySound, OnSoundPlay);
        }

        private void OnSoundPlay(ClipDataMessage data)
        {
            switch (data.SoundType)
            {
                case SoundData.SoundType.None:
                    break;
                case SoundData.SoundType.Taps:
                    break;
                case SoundData.SoundType.Collisions:
                    break;
                case SoundData.SoundType.BackgroundMusic:
                    break;
                case SoundData.SoundType.UI:
                    break;
            }
        }
    }
    
}
