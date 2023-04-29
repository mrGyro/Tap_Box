using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Sounds
{
    public class SoundController : MonoBehaviour, IInitializable
    {
        [SerializeField] private AudioListener _audioListener;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private List<SoundData> _soundData;

        public void Initialize()
        {
        }
    }
    
}
