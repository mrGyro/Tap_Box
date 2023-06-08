using System.Collections.Generic;
using Core.MessengerStatic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class SkinsManager : IInitializable
    {
        private List<Image> _backgrounds = new();

        private Sprite _currentSpriteBg;
        private Material _currentMaterialBg;
        private GameObject _currentTail;
        private ParticleSystem _currentTailParticles;

        public async void Initialize()
        {
            Messenger<Sprite>.AddListener(Constants.Events.OnBackgroundSpriteChanged, OnBackgroundChanged);
            Messenger<Material>.AddListener(Constants.Events.OnBackgroundMaterialChanged, OnBackgroundChanged);
            Messenger<Transform, Vector3>.AddListener(Constants.Events.OnTailStart, CreateTail);

            await ChangeBackgroundSkin(GameManager.Instance.Progress.CurrentBackgroundSkin);
            ChangeTapSkin(GameManager.Instance.Progress.CurrentTapSkin);
            ChangeTailSkin(GameManager.Instance.Progress.CurrentTailSkin);
        }

        private void CreateTail(Transform arg1, Vector3 arg2)
        {
            if (_currentTailParticles == null)
                return;

            var shape = _currentTailParticles.shape;
            shape.scale = arg2;

            GameObject x = Object.Instantiate(_currentTail, arg1);
            x.transform.localPosition = Vector3.zero;
            x.transform.LookAt(arg1.transform.position - arg1.transform.parent.forward * 10);
        }

        public async void ChangeTapSkin(string key)
        {
            GameManager.Instance.Progress.CurrentTapSkin = key;
            var x = await AssetProvider.LoadAssetAsync<GameObject>($"{key}");
            Messenger<GameObject>.Broadcast(Constants.Events.OnTapSkinChanged, x);
        }

        public async void ChangeTailSkin(string key)
        {
            GameManager.Instance.Progress.CurrentTailSkin = key;

            GameManager.Instance.Progress.CurrentTailSkin = key;
            var x = await AssetProvider.LoadAssetAsync<GameObject>($"{key}");
            _currentTail = x;
            _currentTailParticles = x.GetComponent<ParticleSystem>();
        }

        public async UniTask<GameObject> GetTapSkin()
        {
            var x = await AssetProvider.LoadAssetAsync<GameObject>($"{GameManager.Instance.Progress.CurrentTapSkin}");
            return x;
        }

        public async UniTask ChangeBackgroundSkin(string key)
        {
            if (AssetProvider.AddressableResourceExists(key, typeof(Sprite)))
            {
                var x = await AssetProvider.LoadAssetAsync<Sprite>($"{key}");
                GameManager.Instance.Progress.CurrentBackgroundSkin = key;
                OnBackgroundChanged(x);
            }

            if (AssetProvider.AddressableResourceExists(key, typeof(Material)))
            {
                var x = await AssetProvider.LoadAssetAsync<Material>($"{key}");
                GameManager.Instance.Progress.CurrentBackgroundSkin = key;
                OnBackgroundChanged(x);
            }
        }

        public void SetBackgroundSkinSprite(Image bg)
        {
            if (_currentSpriteBg == null)
            {
                bg.sprite = _currentSpriteBg;
            }

            if (_currentMaterialBg == null)
            {
                bg.material = _currentMaterialBg;
            }
        }

        public void AddBackground(Image bg)
        {
            _backgrounds ??= new List<Image>();

            if (_backgrounds.Exists(x => x == bg))
                return;
            
            _backgrounds.Add(bg);
        }

        private void OnBackgroundChanged(Sprite obj)
        {
            int max = Screen.width > Screen.height ? Screen.width : Screen.height;
            var size = new Vector2(max, max);
            
            foreach (var bg in _backgrounds)
            {
                if (bg == null)
                {
                    continue;
                }

                bg.sprite = obj;
                bg.material = null;
                bg.rectTransform.sizeDelta = size;
            }
        }

        private void OnBackgroundChanged(Material obj)
        {
            foreach (var bg in _backgrounds)
            {
                if (bg == null)
                {
                    continue;
                }

                bg.sprite = null;
                bg.material = obj;
            }
        }

        // Messenger<Sprite>.RemoveListener(Constants.Events.OnBackgroundSpriteChanged, OnBackgroundChanged);
        // Messenger<Material>.RemoveListener(Constants.Events.OnBackgroundMaterialChanged, OnBackgroundChanged);
    }
}