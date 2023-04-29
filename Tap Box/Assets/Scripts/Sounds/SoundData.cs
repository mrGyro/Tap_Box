using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sounds
{
    [Serializable]
    public class SoundData
    {
        public enum SoundType
        {
            None,
            Taps,
            Collisions
        }

        public SoundType TypeOfSound;
        public List<AudioClip> Clips;
    }
}