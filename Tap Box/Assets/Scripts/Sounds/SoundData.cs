using System;
using UnityEngine;

namespace Sounds
{
    [Serializable]
    public class SoundData
    {
        public enum SoundType
        {
            None,
            Game,
            Collisions,
            BackgroundMusic,
            UI
        }

        public SoundType TypeOfSound;
        public AudioSource AudioSource;
        public ClipsDictionary Dictionary;
    }
}