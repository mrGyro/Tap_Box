using System;
using Ads;
using Core.MessengerStatic;
using Currency;
using DefaultNamespace.Managers;
using Managers;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.NotEnoughCoin
{
    public class NotEnoughCoinPopup : PopUpBase
    {
        [SerializeField] private Button _watchAdButton;
        [SerializeField] private Button _close;
        [SerializeField] private int _coinsCountForWatchAd;
        [SerializeField] private CurrencyCounter _currencyCounter;

        private IDisposable _isReadyRewarded;
        private IDisposable _isReadyInterstitial;
        private IAdElement _rewardedAd;
        private IAdElement _interstitialAd;

        public override void Initialize()
        {
            ID = Constants.PopUps.NotEnoughCoinPopup;
            Priority = 100;
            _currencyCounter.Initialize();

            _watchAdButton.onClick.RemoveAllListeners();
            _watchAdButton.onClick.AddListener(WatchAd);

            _close.onClick.RemoveAllListeners();
            _close.onClick.AddListener(CloseClick);

            Messenger<string>.AddListener(Constants.Events.OnRewardedVideoReward, OnRewardedAdDone);

            _rewardedAd = GameManager.Instance.Mediation.GetAddElement(Constants.Ads.Rewarded);
            if (_rewardedAd != null)
            {
                _isReadyRewarded = _rewardedAd.IsReady.Subscribe(OnSiReadyStatusChanged);
            }

            _interstitialAd = GameManager.Instance.Mediation.GetAddElement(Constants.Ads.Interstitial);
            if (_interstitialAd != null)
            {
                _isReadyInterstitial = _interstitialAd.IsReady.Subscribe(OnSiReadyStatusChanged);
            }

            OnSiReadyStatusChanged(false);
        }
        
        private void OnSiReadyStatusChanged(bool value)
        {
            _watchAdButton.interactable = 
                (_rewardedAd != null && _rewardedAd.IsReady.Value) 
                || 
                (_interstitialAd != null && _interstitialAd.IsReady.Value);
            
#if UNITY_EDITOR
            _watchAdButton.interactable = true;
#endif
        }

        private async void OnRewardedAdDone(string placeId)
        {
            if (placeId != ID)
            {
                return;
            }

            GameManager.Instance.CurrencyController.AddCurrency(CurrencyController.Type.Coin, _coinsCountForWatchAd);
            await GameManager.Instance.Progress.Save();
        }

        private async void WatchAd()
        {
#if UNITY_EDITOR
            GameManager.Instance.CurrencyController.AddCurrency(CurrencyController.Type.Coin, _coinsCountForWatchAd);
            await GameManager.Instance.Progress.Save();

#endif

            if (GameManager.Instance.Mediation.IsReady(Constants.Ads.Rewarded))
            {
                GameManager.Instance.Mediation.Show(Constants.Ads.Rewarded, ID);
                return;
            }

            if (GameManager.Instance.Mediation.IsReady(Constants.Ads.Interstitial))
            {
                GameManager.Instance.Mediation.Show(Constants.Ads.Interstitial, ID);
                return;
            }
        }

        public override void Show()
        {
            gameObject.SetActive(true);
        }

        private void CloseClick()
        {
            GameManager.Instance.UIManager.ClosePopUp(ID);
        }

        public override void Close()
        {
            _isReadyRewarded?.Dispose();
            _isReadyInterstitial?.Dispose();
            Messenger<string>.RemoveListener(Constants.Events.OnRewardedVideoReward, OnRewardedAdDone);
            gameObject.SetActive(false);
        }
    }
}