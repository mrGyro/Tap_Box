using Unity.Services.Core;
using UnityEngine;

namespace Ads
{
    public class Mediation : MonoBehaviour
    {
        public string AndroidGameID = "4847693";
        public string IOSGameID = "4847693";

        [Header("Rewarded")]
        public string RewardedAdsID_Android;
        public string RewardedAdsID_IOS;
        
        [Header("Interstitial")]
        public string InterstitialAdsID_Android;
        public string InterstitialAdsID_IOS;

        private InterstitialAds _interstitial;
        private RewardedAds _rewarded;
        async void Start()
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            
            switch (Application.platform)
            {
                // Instantiate a rewarded ad object with platform-specific Ad Unit ID
                case RuntimePlatform.Android:
                  //  initializationOptions.SetGameId(AndroidGameID);
                    break;
                case RuntimePlatform.IPhonePlayer:
                  //  initializationOptions.SetGameId(IOSGameID);
                    break;
            }
            
            await UnityServices.InitializeAsync(initializationOptions);
        
            _rewarded = new RewardedAds();
            _interstitial = new InterstitialAds();
            
            _rewarded.Init(RewardedAdsID_Android, RewardedAdsID_IOS);
            _interstitial.Init(InterstitialAdsID_Android, InterstitialAdsID_IOS);
        }
        
        public bool IsInterstitialAvailable()
        {
            return _interstitial.IsInterstitialAvailable();
        }
        
        public void ShowInterstitial()
        {
            _interstitial.ShowAd();
        }
        
        public bool IsRewardedAvailable()
        {
            return _rewarded.IsInterstitialAvailable();
        }
        
        public void ShowRewarded()
        {
            _rewarded.ShowAd();
        }
    }
}