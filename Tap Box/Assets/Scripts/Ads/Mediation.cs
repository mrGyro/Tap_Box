using UnityEngine;

namespace Ads
{
    public class Mediation : MonoBehaviour
    {
        string YOUR_APP_KEY = "19023bd35";

        private InterstitialAds _interstitial;
        private RewardedAds _rewarded;
        private Banner _banner;
        private void Start()
        {
            IronSource.Agent.init(YOUR_APP_KEY);
            IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
        }

        public void ShowBanner()
        {
            
        }
        
        public void ShowRewarded()
        {
            
        }
        
        public void ShowInterstitial()
        {
            
        }

        private void SdkInitializationCompletedEvent()
        {
            _rewarded.Init();
            _interstitial.Init();
            _banner.Init();
            
            
            Debug.LogError("Init complete");
        }

        void OnApplicationPause(bool isPaused)
        {
            IronSource.Agent.onApplicationPause(isPaused);
        }
    }
}