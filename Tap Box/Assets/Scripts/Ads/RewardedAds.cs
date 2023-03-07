using System;
using Unity.Services.Mediation;
using UnityEngine;

namespace Ads
{
    public class RewardedAds
    {
        private string _androidAdUnitId;
        private string _iosAdUnitId;
        IRewardedAd rewardedAd;

        public void Init(string androidAdUnitId, string iosAdUnitId)
        {
            _androidAdUnitId = androidAdUnitId;
            _iosAdUnitId = iosAdUnitId;
            
            switch (Application.platform)
            {
                // Instantiate a rewarded ad object with platform-specific Ad Unit ID
                case RuntimePlatform.Android:
                    rewardedAd = new RewardedAd(_androidAdUnitId);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    rewardedAd = new RewardedAd(_iosAdUnitId);
                    break;
#if UNITY_EDITOR
                default:
                    rewardedAd = new RewardedAd("myExampleAdUnitId");
                    break;
#endif
            }

            // Subscribe callback methods to load events:
            rewardedAd.OnLoaded += AdLoaded;
            rewardedAd.OnFailedLoad += AdFailedToLoad;

            // Subscribe callback methods to show events:
            rewardedAd.OnShowed += AdShown;
            rewardedAd.OnFailedShow += AdFailedToShow;
            rewardedAd.OnUserRewarded += UserRewarded;
            rewardedAd.OnClosed += AdClosed;
            rewardedAd.LoadAsync();
        }
        
        public bool IsInterstitialAvailable()
        {
            return rewardedAd.AdState == AdState.Loaded;
        }

        // Implement load event callback methods:
        void AdLoaded(object sender, EventArgs args)
        {
            Debug.Log("rewardedAd loaded.");
           // _eventManager.SendMessage(Events.Ads.OnRewardedAdAdReady);

            // Execute logic for when the ad has loaded
        }

        void AdFailedToLoad(object sender, LoadErrorEventArgs args)
        {
            Debug.Log("rewardedAd failed to load.");
            // Execute logic for the ad failing to load.
            rewardedAd.LoadAsync();
        }

        // Implement show event callback methods:
        void AdShown(object sender, EventArgs args)
        {
            Debug.Log("rewardedAd shown successfully.");
            // Execute logic for the ad showing successfully.
        }

        void UserRewarded(object sender, RewardEventArgs args)
        {
            Debug.Log("rewardedAd has rewarded user.");
            rewardedAd.LoadAsync();
           // _eventManager.SendMessage(Events.Ads.OnRewardedAdComplete);
            // Execute logic for rewarding the user.
        }

        void AdFailedToShow(object sender, ShowErrorEventArgs args)
        {
            Debug.Log("rewardedAd failed to show.");
            // Execute logic for the ad failing to show.
        }

        void AdClosed(object sender, EventArgs e)
        {
            Debug.Log("rewardedAd is closed.");
            // Execute logic for the user closing the ad.
            rewardedAd.LoadAsync();
        }

        public void ShowAd()
        {
            // Ensure the ad has loaded, then show it.
            if (rewardedAd.AdState == AdState.Loaded)
            {
                rewardedAd.ShowAsync();
            }
        }
    }
}