using System;
using Unity.Services.Mediation;
using UnityEngine;

namespace Ads
{
    public class InterstitialAds 
    {
        private string _androidAdUnitId;
        private string _iosAdUnitId;
        
        IInterstitialAd interstitialAd;

        public void Init(string androidAdUnitId, string iosAdUnitId)
        {
            _androidAdUnitId = androidAdUnitId;
            _iosAdUnitId = iosAdUnitId;
            
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    interstitialAd = MediationService.Instance.CreateInterstitialAd(_androidAdUnitId);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    interstitialAd = MediationService.Instance.CreateInterstitialAd(_iosAdUnitId);
                    break;
#if UNITY_EDITOR

                default:
                    interstitialAd = MediationService.Instance.CreateInterstitialAd("myExampleAdUnitId");
                    break;
#endif

            }

            // Subscribe callback methods to load events:
            interstitialAd.OnLoaded += AdLoaded;
            interstitialAd.OnFailedLoad += AdFailedToLoad;

            // Subscribe callback methods to show events:
            interstitialAd.OnShowed += AdShown;
            interstitialAd.OnFailedShow += AdFailedToShow;
            interstitialAd.OnClosed += AdClosed;
            interstitialAd.LoadAsync();
        }

        public bool IsInterstitialAvailable()
        {
            return interstitialAd.AdState == AdState.Loaded;
        }

        // Implement load event callback methods:
        void AdLoaded(object sender, EventArgs args)
        {
            Debug.Log("interstitialAd loaded.");
            // Execute logic for when the ad has loaded
            //_eventsManager.SendMessage(Events.Ads.OnInterstitialAdReady);
        }

        void AdFailedToLoad(object sender, LoadErrorEventArgs args)
        {
            Debug.Log("interstitialAd failed to load.");
            // Execute logic for the ad failing to load.
            interstitialAd.LoadAsync();
        }

        // Implement show event callback methods:
        void AdShown(object sender, EventArgs args)
        {
            Debug.Log("interstitialAd shown successfully.");
            // Execute logic for the ad showing successfully.
        }

        void AdFailedToShow(object sender, ShowErrorEventArgs args)
        {
            Debug.Log("interstitialAd failed to show.");
            // Execute logic for the ad failing to show.
        }

        private void AdClosed(object sender, EventArgs e)
        {
            Debug.Log("interstitialAd has closed");
            // Execute logic after an ad has been closed.
            interstitialAd.LoadAsync();
            //_eventsManager.SendMessage(Events.Ads.OnInterstitialAdComplete);
        }

        public void ShowAd()
        {
            // Ensure the ad has loaded, then show it.
            if (interstitialAd.AdState == AdState.Loaded)
            {
                interstitialAd.ShowAsync();
            }
        }
    }
}