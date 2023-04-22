using System;
using Core.MessengerStatic;
using Currency;
using Cysharp.Threading.Tasks;
using Managers;
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
        private string _skinButton = "skin_button_";

        public SkinData GetSkinData() => data;

        public void SetSkinData(SkinData value) => data = value;

        public async void Setup()
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
            
            getTypeGameObject.SetActive(!data.IsOpen);

            if (data.IsOpen)
                return;

            switch (data.WayToGet)
            {
                case CurrencyController.Type.Coin:
                    getTypeIcon.sprite = await AssetProvider.LoadAssetAsync<Sprite>($"{Constants.Currency.Coins}_icon");
                    getTypeText.text = data.Price.ToString();
                    break;
                case CurrencyController.Type.RewardedAds:
                    getTypeIcon.sprite = await AssetProvider.LoadAssetAsync<Sprite>($"{Constants.Currency.Ads}_icon");
                    getTypeText.text = "Open";
                    var rewardedAd = GameManager.Instance.Mediation.GetAddElement(Constants.Ads.Rewarded);
                    if (rewardedAd != null)
                    {
                        button.interactable = rewardedAd.IsReady.Value;
                        _isReady = rewardedAd.IsReady.Subscribe(OnSiReadyStatusChanged);
                    }

                    break;
                case CurrencyController.Type.InterstitialAds:
                    getTypeIcon.sprite = await AssetProvider.LoadAssetAsync<Sprite>($"{Constants.Currency.Ads}_icon");
                    getTypeText.text = "Open";

                    var interstitialAd = GameManager.Instance.Mediation.GetAddElement(Constants.Ads.Interstitial);
                    if (interstitialAd != null)
                    {
                        button.interactable = interstitialAd.IsReady.Value;
                        _isReady = interstitialAd.IsReady.Subscribe(OnSiReadyStatusChanged);
                    }

                    break;
            }
        }

        private async void OnClick()
        {
            if (data.IsOpen)
            {
                await ChangeSkin();
                return;
            }

            switch (data.WayToGet)
            {
                case CurrencyController.Type.Coin:
                    if (!GameManager.Instance.Progress.Currencies.ContainsKey(data.WayToGet))
                        return;

                    if (GameManager.Instance.Progress.Currencies[data.WayToGet] >= data.Price)
                    {
                        GameManager.Instance.CurrencyController.RemoveCurrency(data.WayToGet, data.Price);
                        BuySkin();
                        Setup();
                        await GameManager.Instance.Progress.Save();
                    }

                    break;
                case CurrencyController.Type.RewardedAds:

                    if (!GameManager.Instance.Mediation.IsReady(Constants.Ads.Rewarded))
                        return;

                    Messenger<string>.AddListener(Constants.Events.OnRewardedVideoReward, OnRewardedDone);
                    GameManager.Instance.Mediation.Show(Constants.Ads.Rewarded, _skinButton + data.SkinAddressableName);

                    return;
                case CurrencyController.Type.InterstitialAds:
                    if (!GameManager.Instance.Mediation.IsReady(Constants.Ads.Interstitial))
                        return;

                    GameManager.Instance.Mediation.Show(Constants.Ads.Interstitial, _skinButton + data.SkinAddressableName);
                    BuySkin();
                    Setup();
                    await GameManager.Instance.Progress.Save();
                    return;
            }
        }

        private async UniTask ChangeSkin()
        {
            Debug.LogError("---" + data.Type);

            switch (data.Type)
            {
                case CurrencyController.Type.BoxSkin:
                    await GameManager.Instance.Progress.ChangeBlock(data.SkinAddressableName);
                    await GameManager.Instance.Progress.Save();
                    break;
                case CurrencyController.Type.BackgroundSkin:
                   await GameManager.Instance.SkinsManager.ChangeBackgroundSkin(data.SkinAddressableName);
                    await GameManager.Instance.Progress.Save();
                    break;
                case CurrencyController.Type.TailSkin:
                    GameManager.Instance.SkinsManager.ChangeTailSkin(data.SkinAddressableName);
                    await GameManager.Instance.Progress.Save();
                    break;
                case CurrencyController.Type.TapSkin:
                    GameManager.Instance.SkinsManager.ChangeTapSkin(data.SkinAddressableName);
                    await GameManager.Instance.Progress.Save();
                    break;
            }
            
            
        }

        private async void OnRewardedDone(string value)
        {
            if (value != _skinButton + data.SkinAddressableName)
            {
                return;
            }

            BuySkin();
            Setup();
            await GameManager.Instance.Progress.Save();
        }

        private void OnSiReadyStatusChanged(bool value)
        {
            button.interactable = data.IsOpen || value;
        }

        private async void BuySkin()
        {
            data.IsOpen = true;
            GameManager.Instance.CurrencyController.AddSkin(data.WayToGet, data.Type, data.SkinAddressableName);
            await GameManager.Instance.Progress.ChangeBlock(data.SkinAddressableName);
        }

        private void OnDestroy()
        {
            _isReady?.Dispose();
        }
    }
}