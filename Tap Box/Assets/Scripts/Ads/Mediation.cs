using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Ads
{
    public class Mediation : MonoBehaviour, IInitializable
    {
        private const string YOUR_APP_KEY = "19023bd35";
        private readonly Dictionary<string, IAdElement> _adElements = new();
        private bool _isInitialized;

        public void Initialize()
        {
            _adElements.Add(Constants.Ads.Interstitial, new InterstitialAds());
            _adElements.Add(Constants.Ads.Rewarded, new RewardedAds());
            _adElements.Add(Constants.Ads.Banner, new Banner());

            foreach (var adElement in _adElements)
            {
                adElement.Value.Init();
            }

            IronSource.Agent.init(YOUR_APP_KEY);
            Debug.LogError("start init");
            IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;

            LoadAddCycle();
        }

        public void Show(string adType)
        {
            if (!IsReady(adType))
            {
                Debug.LogError(adType + " not ready initialize = " + _isInitialized);
                return;
            }

            if (_adElements.ContainsKey(adType))
                _adElements[adType].Show();
        }

        public IAdElement GetAddElement(string adType)
        {
            return _adElements.ContainsKey(adType) ? _adElements[adType] : null;
        }

        public void Hide(string adType)
        {
            if (_adElements.ContainsKey(adType))
                _adElements[adType].Hide();
        }

        public void Load(string adType)
        {
            if (_adElements.ContainsKey(adType))
                _adElements[adType].Load();
        }

        public bool IsReady(string adType)
            => _adElements.ContainsKey(adType) && _adElements[adType].IsReady.Value;

        private async void LoadAddCycle()
        {
            while (true)
            {
                await UniTask.Delay(2000);

                if (!_isInitialized)
                    continue;

                foreach (var adElement in _adElements)
                {
                    if (adElement.Value.IsReady.Value)
                        adElement.Value.Load();
                }
            }
        }

        private void SdkInitializationCompletedEvent()
        {
            // foreach (var adElement in _adElements)
            // {
            //     adElement.Value.Init();
            // }

            Debug.LogError("Init complete");
            _isInitialized = true;
        }

        private void OnApplicationPause(bool isPaused)
        {
            IronSource.Agent.onApplicationPause(isPaused);
        }
    }
}