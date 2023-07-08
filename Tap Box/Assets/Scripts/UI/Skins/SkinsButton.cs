using System;
using System.Collections.Generic;
using Core.MessengerStatic;
using Currency;
using Cysharp.Threading.Tasks;
using Managers;
using TMPro;
using UniRx;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif

using UnityEngine;
using UnityEngine.UI;

namespace UI.Skins
{
    public class SkinsButton : MonoBehaviour
    {
        private static Action OnSelectionChanged;
        [SerializeField] private Button button;
        [SerializeField] private GameObject getTypeGameObject;
        [SerializeField] private GameObject _uunounBg;
        [SerializeField] private Image icon;
        [SerializeField] private Image getTypeBg;
        [SerializeField] private Image _selector;
        [SerializeField] private Image getTypeIcon;
        [SerializeField] private TMP_Text getTypeText;
        [Space] [SerializeField] private SkinData data;

        private IDisposable _isReady;
        private string _skinButton = "skin_button_";

        public SkinData GetSkinData() => data;

        public void SetSkinData(SkinData value) => data = value;

        [ContextMenu("Tools/Create skin addressable")]
        public void CreateAddresabbleForSkin()
        {
#if UNITY_EDITOR

            string group_name = "Blocks";
            string path_to_object = @"Assets\Prefabs\Skins\Boxes\" + data.SkinAddressableName + @"\";

            List<string> pathes = new List<string>()
            {
                "TapFlowBox",
                "BigBoxTapFlowBox_1_1_2",
                "BigBoxTapFlowBox_1_1_3",
                "BigBoxTapFlowBox_2_1_2",
                "BigBoxTapFlowBox_2_1_3",
                "BigBoxTapFlowBox_2_2_2",
                "BigBoxTapFlowBox_2_2_3",
                "BigBoxTapFlowBox_3_1_3",
                "BigBoxTapFlowBox_3_2_3",
                "BigBoxTapFlowBox_3_3_3"
            };


            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetGroup g = settings.FindGroup(group_name);

            foreach (var VARIABLE in pathes)
            {
                string assetGUID = AssetDatabase.AssetPathToGUID(path_to_object + VARIABLE + ".prefab");
                if (string.IsNullOrEmpty(assetGUID))
                {
                    continue;
                }
                
                var x = settings.CreateAssetReference(assetGUID);

                var entry = settings.FindAssetEntry(x.AssetGUID);
                entry.address = data.SkinAddressableName + "_" + VARIABLE;
                settings.MoveEntry(entry, g);
            }


            AssetDatabase.SaveAssets();
#endif

        }

        public async void Setup()
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);

            getTypeGameObject.SetActive(!data.IsOpen);
            OnSelectionChanged += SetSelector;
            if (data.IsOpen)
            {
                _uunounBg.SetActive(false);
                SetSelector();
                return;
            }

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
                case CurrencyController.Type.RandomSkin:
                    _uunounBg.SetActive(true);
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
                    {
                        return;
                    }

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
                    {
                        return;
                    }

                    Messenger<string>.AddListener(Constants.Events.OnRewardedVideoReward, OnRewardedDone);
                    GameManager.Instance.Mediation.Show(Constants.Ads.Rewarded, _skinButton + data.SkinAddressableName);

                    return;
                case CurrencyController.Type.InterstitialAds:
                    if (!GameManager.Instance.Mediation.IsReady(Constants.Ads.Interstitial))
                    {
                        return;
                    }

                    GameManager.Instance.Mediation.Show(Constants.Ads.Interstitial, _skinButton + data.SkinAddressableName);
                    BuySkin();
                    Setup();
                    await GameManager.Instance.Progress.Save();
                    return;
            }
        }

        private async UniTask ChangeSkin()
        {
            Debug.LogError(data.SkinAddressableName);
            switch (data.Type)
            {
                case CurrencyController.Type.BoxSkin:
                    GameManager.Instance.UIManager.ShowUpToAllPopUp(Constants.PopUps.LoadingPopup);

                    await UniTask.Delay(500);
                    await GameManager.Instance.Progress.ChangeBlock(data.SkinAddressableName);
                    await GameManager.Instance.Progress.Save();
                    GameManager.Instance.UIManager.ClosePopUp(Constants.PopUps.LoadingPopup);

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

            OnSelectionChanged?.Invoke();
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

        public void SetSelector()
        {
            switch (data.Type)
            {
                case CurrencyController.Type.BoxSkin:
                    _selector.enabled = data.SkinAddressableName == GameManager.Instance.Progress.CurrentBoxSkin;
                    break;
                case CurrencyController.Type.BackgroundSkin:
                    _selector.enabled = data.SkinAddressableName == GameManager.Instance.Progress.CurrentBackgroundSkin;
                    break;
                case CurrencyController.Type.TailSkin:
                    _selector.enabled = data.SkinAddressableName == GameManager.Instance.Progress.CurrentTailSkin;
                    break;
                case CurrencyController.Type.TapSkin:
                    _selector.enabled = data.SkinAddressableName == GameManager.Instance.Progress.CurrentTapSkin;
                    break;
            }
        }

        private void OnSiReadyStatusChanged(bool value)
        {
            button.interactable = data.IsOpen || value;
        }

        private async void BuySkin()
        {
            data.IsOpen = true;
            GameManager.Instance.CurrencyController.AddSkin(data.WayToGet, data.Type, data.SkinAddressableName);
            await ChangeSkin();
        }

        private void OnDestroy()
        {
            _isReady?.Dispose();
        }
    }
}