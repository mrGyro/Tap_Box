using System;
using UniRx;

namespace Ads
{
    public class InterstitialAds : IAdElement
    {
        public ReactiveProperty<bool> IsReady { get; set; }

        public Action<string> OnAddLoad { get; set; }

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
            IronSource.Agent.showInterstitial();
        }

        public void Hide()
        {
        }

        public void Load()
        {
            if (!IsReady.Value)
                return;

            IronSource.Agent.loadInterstitial();
            OnAddLoad?.Invoke(Constants.Ads.Interstitial);
        }
        

/************* Interstitial AdInfo Delegates *************/
// Invoked when the interstitial ad was loaded succesfully.
        private void InterstitialOnAdReadyEvent(IronSourceAdInfo adInfo)
        {
            IsReady.SetValueAndForceNotify(true);
        }

// Invoked when the initialization process has failed.
        private void InterstitialOnAdLoadFailed(IronSourceError ironSourceError)
        {
            IsReady.SetValueAndForceNotify(false);
        }

// Invoked when the Interstitial Ad Unit has opened. This is the impression indication. 
        private void InterstitialOnAdOpenedEvent(IronSourceAdInfo adInfo)
        {
            IsReady.SetValueAndForceNotify(false);
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