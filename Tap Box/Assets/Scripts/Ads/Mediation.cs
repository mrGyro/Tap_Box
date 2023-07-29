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

        public async void Initialize()
        {
            _adElements.Add(Constants.Ads.Interstitial, new InterstitialAds());
            _adElements.Add(Constants.Ads.Rewarded, new RewardedAds());
            _adElements.Add(Constants.Ads.Banner, new Banner());

            foreach (var adElement in _adElements)
            {
                adElement.Value.Init();
            }
            
            IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
            IronSource.Agent.init(YOUR_APP_KEY);

            await UniTask.Delay(1000);
            IronSource.Agent.validateIntegration();
            LoadAddCycle();
        }

        public void Show(string adType, string place)
        {
            if (!IsReady(adType))
            {
                Debug.LogError(adType + " not ready initialize = " + _isInitialized);
                return;
            }

            if (_adElements.ContainsKey(adType))
                _adElements[adType].Show(place);
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
        {
#if UNITY_EDITOR
            return true;
#endif 
          
            return _adElements.ContainsKey(adType) && _adElements[adType].IsReady.Value;
        } 

        private async void LoadAddCycle()
        {
            while (true)
            {
                await UniTask.Delay(2000);

                foreach (var adElement in _adElements)
                {
                    if (adElement.Value.IsReady.Value)
                        continue;

                    adElement.Value.Load();
                }
            }
        }

        private void SdkInitializationCompletedEvent()
        {
            Debug.LogError("Init complete");
            _isInitialized = true;
        }

        private void OnApplicationPause(bool isPaused)
        {
            IronSource.Agent.onApplicationPause(isPaused);
        }
    }
}