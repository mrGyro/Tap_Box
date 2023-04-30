using System.Collections.Generic;
using System.Linq;
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

        public void Play(ClipDataMessage data)
        {
            OnSoundPlay(data);
        }

        private void OnSoundPlay(ClipDataMessage data)
        {
            switch (data.SoundType)
            {
                case SoundData.SoundType.None:
                    break;
                case SoundData.SoundType.Game:
                    PlayOneOfMeny(data);
                    break;
                case SoundData.SoundType.Collisions:
                    PlayOne(data);
                    break;
                case SoundData.SoundType.BackgroundMusic:
                    PlayOne(data);
                    break;
                case SoundData.SoundType.UI:
                    PlayOne(data);
                    break;
            }
        }

        private void PlayOneOfMeny(ClipDataMessage data)
        {
            var sound = _soundData.FirstOrDefault(x => x.TypeOfSound == data.SoundType);
            if (sound == null)
            {
                return;
            }

            var clips = sound.Dictionary.Clips.Where(x => x.Id == data.Id).ToList();

            if (clips.Count == 0)
            {
                return;
            }

            AudioClip clip = clips.Count == 1 ? clips[0].Clip : clips[Random.Range(0, clips.Count)].Clip;
            sound.AudioSource.PlayOneShot(clip);
        }

        private void PlayOne(ClipDataMessage data)
        {
            var sound = _soundData.FirstOrDefault(x => x.TypeOfSound == data.SoundType);
            if (sound == null)
            {
                return;
            }
            var clip = sound.Dictionary.Clips.FirstOrDefault(x => x.Id == data.Id);
            
            if (clip == null)
            {
                return;
            }
            sound.AudioSource.PlayOneShot(clip.Clip);

        }
    }
}