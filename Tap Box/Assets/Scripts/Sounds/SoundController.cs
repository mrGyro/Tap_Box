using System.Collections.Generic;
using System.Linq;
using Managers;
using Unity.VisualScripting;
using UnityEngine;

namespace Sounds
{
    public class SoundController : MonoBehaviour, IInitializable
    {
        [SerializeField] private AudioListener _audioListener;
        [SerializeField] private List<SoundData> _soundData;

        private SoundData _sound;
        private List<ClipData> _clipDatas;
        public void Initialize()
        {
        }

        public void Play(ClipDataMessage data)
        {
            if (!GameManager.Instance.Progress.CurrentSoundSetting)
            {
                return;
            }
            
            OnSoundPlay(data);
        }

        private void OnSoundPlay(ClipDataMessage data)
        {
            if (!GameManager.Instance.Progress.CurrentSoundSetting)
            {
                return;
            }
            
            switch (data.SoundType)
            {
                case SoundData.SoundType.None:
                    break;
                case SoundData.SoundType.Game:
                    PlayTap(data);
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

        private void PlayTap(ClipDataMessage data)
        {
            _sound ??= _soundData.FirstOrDefault(x => x.TypeOfSound == data.SoundType);

            if (_sound == null)
            {
                return;
            }

            _clipDatas ??= _sound.Dictionary.Clips.Where(x => x.Id == data.Id).ToList();

            var clip = _clipDatas.Count == 1 ? _clipDatas[0].Clip : _clipDatas[Random.Range(0, _clipDatas.Count)].Clip;
            _sound.AudioSource.PlayOneShot(clip);
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