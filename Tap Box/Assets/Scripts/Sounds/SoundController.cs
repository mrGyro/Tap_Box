using System.Collections.Generic;
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
        }
    }
    
}
