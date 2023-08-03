using UniRx;
using UnityEngine;

namespace Ads
{
    public class Banner : IAdElement
    {
        public ReactiveProperty<bool> IsReady { get; set; }
        public bool isEnable { get; set; }

        public void Init()
        {
            isEnable = true;
            IsReady = new ReactiveProperty<bool>(false);
            
            //Add AdInfo Banner Events
            IronSourceBannerEvents.onAdLoadedEvent += BannerOnAdLoadedEvent;
            IronSourceBannerEvents.onAdLoadFailedEvent += BannerOnAdLoadFailedEvent;
            IronSourceBannerEvents.onAdClickedEvent += BannerOnAdClickedEvent;
            IronSourceBannerEvents.onAdScreenPresentedEvent += BannerOnAdScreenPresentedEvent;
            IronSourceBannerEvents.onAdScreenDismissedEvent += BannerOnAdScreenDismissedEvent;
            IronSourceBannerEvents.onAdLeftApplicationEvent += BannerOnAdLeftApplicationEvent;
        }

        public void Show(string place)
        {
            if (isEnable)
            {
                Debug.Log("------show banner------" + place);
                IronSource.Agent.displayBanner();
            }
        }

        public void Hide()
        {
            IronSource.Agent.hideBanner();
        }

        public void Load()
        {
            IronSource.Agent.loadBanner(IronSourceBannerSize.SMART, IronSourceBannerPosition.BOTTOM);
        }

        /************* Banner AdInfo Delegates *************/
//Invoked once the banner has loaded
        void BannerOnAdLoadedEvent(IronSourceAdInfo adInfo)
        {
            Debug.LogError("BannerOnAdLoadedEvent");
            IsReady.SetValueAndForceNotify(true);
        }

//Invoked when the banner loading process has failed.
        void BannerOnAdLoadFailedEvent(IronSourceError ironSourceError)
        {
            Debug.LogError("BannerOnAdLoadFailedEvent");
            IsReady.SetValueAndForceNotify(false);
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