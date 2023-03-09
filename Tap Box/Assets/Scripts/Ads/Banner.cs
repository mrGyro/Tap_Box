using System;
using UniRx;

namespace Ads
{
    public class Banner : IAdElement
    {
        public ReactiveProperty<bool> IsReady { get; set; }

        public Action<string> OnAddLoad { get; set; }

        public void Init()
        {
            IsReady = new ReactiveProperty<bool>(false);

            //Add AdInfo Banner Events
            IronSourceBannerEvents.onAdLoadedEvent += BannerOnAdLoadedEvent;
            IronSourceBannerEvents.onAdLoadFailedEvent += BannerOnAdLoadFailedEvent;
            IronSourceBannerEvents.onAdClickedEvent += BannerOnAdClickedEvent;
            IronSourceBannerEvents.onAdScreenPresentedEvent += BannerOnAdScreenPresentedEvent;
            IronSourceBannerEvents.onAdScreenDismissedEvent += BannerOnAdScreenDismissedEvent;
            IronSourceBannerEvents.onAdLeftApplicationEvent += BannerOnAdLeftApplicationEvent;
        }

        public void Show()
        {
            IronSource.Agent.displayBanner();
        }

        public void Hide()
        {
            IronSource.Agent.hideBanner();
        }

        public void Load()
        {
            IronSource.Agent.loadBanner(IronSourceBannerSize.SMART, IronSourceBannerPosition.BOTTOM);
            OnAddLoad?.Invoke(Constants.Ads.Banner);
        }

        // public bool IsReady()
        // {
        //     return _isReady;
        // }

        /************* Banner AdInfo Delegates *************/
//Invoked once the banner has loaded
        void BannerOnAdLoadedEvent(IronSourceAdInfo adInfo)
        {
           // _isReady = true;
        }

//Invoked when the banner loading process has failed.
        void BannerOnAdLoadFailedEvent(IronSourceError ironSourceError)
        {
           // _isReady = false;
        }

// Invoked when end user clicks on the banner ad
        void BannerOnAdClickedEvent(IronSourceAdInfo adInfo)
        {
        }

//Notifies the presentation of a full screen content following user click
        void BannerOnAdScreenPresentedEvent(IronSourceAdInfo adInfo)
        {
        }

//Notifies the presented screen has been dismissed
        void BannerOnAdScreenDismissedEvent(IronSourceAdInfo adInfo)
        {
        }

//Invoked when the user leaves the app
        void BannerOnAdLeftApplicationEvent(IronSourceAdInfo adInfo)
        {
        }
    }
}