using System;
using UnityEngine;

namespace Ads
{
    public class InterstitialAds
    {
        public void Init()
        {
            //Add AdInfo Interstitial Events
            IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
            IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnAdLoadFailed;
            IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialOnAdOpenedEvent;
            IronSourceInterstitialEvents.onAdClickedEvent += InterstitialOnAdClickedEvent;
            IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
            IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
            IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;
        }

        public void Show()
        {
                
        }

/************* Interstitial AdInfo Delegates *************/
// Invoked when the interstitial ad was loaded succesfully.
        private void InterstitialOnAdReadyEvent(IronSourceAdInfo adInfo)
        {
        }

// Invoked when the initialization process has failed.
        private void InterstitialOnAdLoadFailed(IronSourceError ironSourceError)
        {
        }

// Invoked when the Interstitial Ad Unit has opened. This is the impression indication. 
        private void InterstitialOnAdOpenedEvent(IronSourceAdInfo adInfo)
        {
        }

// Invoked when end user clicked on the interstitial ad
        private void InterstitialOnAdClickedEvent(IronSourceAdInfo adInfo)
        {
        }

// Invoked when the ad failed to show.
        void InterstitialOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
        {
        }

// Invoked when the interstitial ad closed and the user went back to the application screen.
        void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo)
        {
        }

// Invoked before the interstitial ad was opened, and before the InterstitialOnAdOpenedEvent is reported.
// This callback is not supported by all networks, and we recommend using it only if  
// it's supported by all networks you included in your build. 
        private void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo adInfo)
        {
        }
    }
}