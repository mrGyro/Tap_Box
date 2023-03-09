using System;
using Core.MessengerStatic;
using Currency;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Skins
{
    public class SkinsButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private GameObject getTypeGameObject;
        [SerializeField] private Image icon;
        [SerializeField] private Image getTypeBg;
        [SerializeField] private Image getTypeIcon;
        [SerializeField] private TMP_Text getTypeText;
        [Space] [SerializeField] private SkinData data;

        private IDisposable _isReady;

        public SkinData GetSkinData()
            => data;

        public void SetSkinData(SkinData value)
            => data = value;

        public async void Setup()
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);

            getTypeGameObject.SetActive(!data.IsOpen);

            if (data.IsOpen)
                return;

            switch (data.Type)
            {
                case CurrencyController.Type.Coin:
                    getTypeIcon.sprite = await AssetProvider.LoadAssetAsync<Sprite>($"{Constants.Currency.Coins}_icon");
                    getTypeText.text = data.Price.ToString();
                    break;
                case CurrencyController.Type.RewardedAds:
                    getTypeIcon.sprite = await AssetProvider.LoadAssetAsync<Sprite>($"{Constants.Currency.Ads}_icon");
                    getTypeText.text = "Open";
                    var adElement = Managers.Instance.Mediation.GetAddElement(Constants.Ads.Rewarded);
                    if (adElement != null)
                    {
                        button.interactable = adElement.IsReady.Value;
                        _isReady = adElement.IsReady.Subscribe(OnSiReadyStatusChanged);
                    }

                    break;
                case CurrencyController.Type.InterstitialAds:
                    getTypeIcon.sprite = await AssetProvider.LoadAssetAsync<Sprite>($"{Constants.Currency.Ads}_icon");
                    getTypeText.text = "Open";
                    break;
            }
        }

        private async void OnClick()
        {
            if (data.IsOpen)
            {
                await Managers.Instance.Progress.ChangeBlock(data.SkinAddressableName);
                await Managers.Instance.Progress.Save();
                return;
            }

            switch (data.Type)
            {
                case CurrencyController.Type.Coin:
                    if (!Managers.Instance.Progress.Currencies.ContainsKey(data.Type))
                        return;

                    if (Managers.Instance.Progress.Currencies[data.Type] >= data.Price)
                    {
                        Managers.Instance.CurrencyController.RemoveCurrency(data.Type, data.Price);
                        BuySkin();
                        Setup();
                        await Managers.Instance.Progress.Save();
                    }

                    break;
                case CurrencyController.Type.RewardedAds:

                    if (!Managers.Instance.Mediation.IsReady(Constants.Ads.Rewarded))
                        return;

                    Messenger<bool>.AddListener(Constants.Events.OnRewardedClosed, OnRewardedDone);
                    Managers.Instance.Mediation.Show(Constants.Ads.Rewarded);

                    Setup();
                    await Managers.Instance.Progress.Save();

                    return;
                case CurrencyController.Type.InterstitialAds:
                    if (!Managers.Instance.Mediation.IsReady(Constants.Ads.Interstitial))
                        return;
                    
                    Managers.Instance.Mediation.Show(Constants.Ads.Interstitial);
                    BuySkin();
                    Setup();
                    await Managers.Instance.Progress.Save();
                    return;
            }
        }

        private void OnRewardedDone(bool value)
        {
            if (value)
                BuySkin();

            Messenger<bool>.RemoveListener(Constants.Events.OnRewardedClosed, OnRewardedDone);
        }

        private void OnSiReadyStatusChanged(bool value)
        {
            button.interactable = value;
        }

        private async void BuySkin()
        {
            data.IsOpen = true;
            Managers.Instance.CurrencyController.AddSkin(data.Type, data.SkinAddressableName);
            await Managers.Instance.Progress.ChangeBlock(data.SkinAddressableName);
        }

        private void OnDestroy()
        {
            _isReady.Dispose();
        }
    }
}