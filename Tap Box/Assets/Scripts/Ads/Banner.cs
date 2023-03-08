namespace Ads
{
    public class Banner
    {
        public void Init()
        {
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
                
        }
        
        /************* Banner AdInfo Delegates *************/
//Invoked once the banner has loaded
        void BannerOnAdLoadedEvent(IronSourceAdInfo adInfo) 
        {
        }
//Invoked when the banner loading process has failed.
        void BannerOnAdLoadFailedEvent(IronSourceError ironSourceError) 
        {
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