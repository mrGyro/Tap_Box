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

        public void Initialize()
        {
            Messenger<Sprite>.AddListener(Constants.Events.OnBackgroundSpriteChanged, OnBackgroundChanged);
            Messenger<Material>.AddListener(Constants.Events.OnBackgroundMaterialChanged, OnBackgroundChanged);
            ChangeBackgroundSkin(GameManager.Instance.Progress.CurrentBackgroundSkin);
            ChangeTapSkin(GameManager.Instance.Progress.CurrentTapSkin);
        }

        public async void ChangeTapSkin(string key)
        {
            
        }
        
        public async UniTask ChangeBackgroundSkin(string key)
        {
            if (AssetProvider.AddressableResourceExists(key, typeof(Sprite)))
            {
                var x = await AssetProvider.LoadAssetAsync<Sprite>($"{key}");
                Messenger<Sprite>.Broadcast(Constants.Events.OnBackgroundSpriteChanged, x);
                GameManager.Instance.Progress.CurrentBackgroundSkin = key;
            }
                    
            if (AssetProvider.AddressableResourceExists(key, typeof(Material)))
            {
                var x = await AssetProvider.LoadAssetAsync<Material>($"{key}");
                Messenger<Material>.Broadcast(Constants.Events.OnBackgroundMaterialChanged, x);
                GameManager.Instance.Progress.CurrentBackgroundSkin = key;
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
            if (_backgrounds == null)
            {
                _backgrounds = new List<Image>();
            }

            _backgrounds.Add(bg);
        }

        private void OnBackgroundChanged(Sprite obj)
        {
            foreach (var bg in _backgrounds)
            {
                if (bg == null)
                {
                    continue;
                }

                bg.sprite = obj;
                bg.material = null;
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