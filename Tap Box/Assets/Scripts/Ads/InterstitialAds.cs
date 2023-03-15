using UniRx;
using UnityEngine;

namespace Ads
{
    public class InterstitialAds : IAdElement
    {
        public ReactiveProperty<bool> IsReady { get; set; }

        public void Init()
        {
            IsReady = new ReactiveProperty<bool>(false);

            //Add AdInfo Interstitial Events
            IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
            IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnAdLoadFailed;
            IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialOnAdOpenedEvent;
            IronSourceInterstitialEvents.onAdClickedEvent += InterstitialOnAdClickedEvent;
            IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
            IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
            IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;
        }

        public void Show(string place)
        {
            if (!IsReady.Value)
                return;
            
            IsReady.SetValueAndForceNotify(false);
            IronSource.Agent.showInterstitial();
        }

        public void Hide()
        {
        }

        public void Load()
        {
            if (IronSource.Agent.isInterstitialReady())
            {
                IsReady.SetValueAndForceNotify(true);
                return;
            }
            
            IronSource.Agent.loadInterstitial();
        }
        

/************* Interstitial AdInfo Delegates *************/
// Invoked when the interstitial ad was loaded succesfully.
        private void InterstitialOnAdReadyEvent(IronSourceAdInfo adInfo)
        {
            Debug.LogError("InterstitialOnAdReadyEvent");
            IsReady.SetValueAndForceNotify(true);
        }

// Invoked when the initialization process has failed.
        private void InterstitialOnAdLoadFailed(IronSourceError ironSourceError)
        {
            Debug.LogError("InterstitialOnAdLoadFailed");

        }

// Invoked when the Interstitial Ad Unit has opened. This is the impression indication. 
        private void InterstitialOnAdOpenedEvent(IronSourceAdInfo adInfo)
        {
            Debug.LogError("InterstitialOnAdOpenedEvent");

        }

// Invoked when end user clicked on the interstitial ad
        private void InterstitialOnAdClickedEvent(IronSourceAdInfo adInfo)
        {
            Debug.LogError("InterstitialOnAdClickedEvent");

        }

// Invoked when the ad failed to show.
        void InterstitialOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
        {
            Debug.LogError("InterstitialOnAdShowFailedEvent");

        }

// Invoked when the interstitial ad closed and the user went back to the application screen.
        void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo)
        {
            Debug.LogError("InterstitialOnAdClosedEvent");

        }

// Invoked before the interstitial ad was opened, and before the InterstitialOnAdOpenedEvent is reported.
// This callback is not supported by all networks, and we recommend using it only if  
// it's supported by all networks you included in your build. 
        private void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo adInfo)
        {
            Debug.LogError("InterstitialOnAdShowSucceededEvent");

        }
    }
}