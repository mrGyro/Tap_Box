using System;
using System.Collections.Generic;
using System.Linq;
using Core.MessengerStatic;
using Currency;
using DefaultNamespace.Managers;
using Managers;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Skins
{
    public class SkinsPoUp : PopUpBase
    {
        [SerializeField] private Image _background;
        [SerializeField] private CurrencyCounter counter;
        [SerializeField] private Button _getRandomSkinByCoins;
        [SerializeField] private TMP_Text _getRandomSkinByCoinsText;
        [SerializeField] private Button _getCoinsByRewardedAd;
        [SerializeField] private TMP_Text _getCoinsByRewardedAdText;

        [SerializeField] private List<SkinsButton> _boxesSkinsButtons;
        [SerializeField] private List<SkinsButton> _backgroundsSkinsButtons;
        [SerializeField] private List<SkinsButton> _tailButtons;
        [SerializeField] private List<SkinsButton> _tapButtons;
        [SerializeField] private List<StateButton> _topButtons;

        private const int _coinsCountForWatchAd = 500;
        private CurrencyController.Type _selectedType;
        private IDisposable _isReady;

        public override void Initialize()
        {
            Setup();
            Messenger<CurrencyController.Type, string>.AddListener(Constants.Events.OnGetRandomSkin, OnGetRandomSkin);
            GameManager.Instance.PlayerLevelManager.OnLevelChanged += OnGetSkinByLevel;
            Messenger<string>.AddListener(Constants.Events.OnRewardedVideoReward, OnRewardedAdDone);

            _getCoinsByRewardedAd.onClick.RemoveAllListeners();
            _getCoinsByRewardedAd.onClick.AddListener(GetCoinsForAd);

            _getRandomSkinByCoins.onClick.RemoveAllListeners();
            _getRandomSkinByCoins.onClick.AddListener(BuyRandomSkin);

            _getCoinsByRewardedAdText.text = $"+{_coinsCountForWatchAd}";
            _getRandomSkinByCoinsText.text = GetPriceForRandomSkin().ToString();
            _selectedType = CurrencyController.Type.BoxSkin;
            
            var rewardedAd = GameManager.Instance.Mediation.GetAddElement(Constants.Ads.Rewarded);
            if (rewardedAd != null)
            {
                _getCoinsByRewardedAd.interactable = rewardedAd.IsReady.Value;
                _isReady = rewardedAd.IsReady.Subscribe(OnSiReadyStatusChanged);
            }
            
#if UNITY_EDITOR
            _getCoinsByRewardedAd.interactable = true;
#endif
        }

        public void SetCurrentSize()
        {
            var skinButton = _boxesSkinsButtons.FirstOrDefault(x => x.GetSkinData().SkinAddressableName == GameManager.Instance.Progress.CurrentBoxSkin);
            if (skinButton)
            {
                GameField.Size = skinButton.GetSkinData().Size;
            }
        }
        private void OnSiReadyStatusChanged(bool value)
        {
            _getCoinsByRewardedAd.interactable = value;
        }
        private void OnEnable()
        {
            SetBottomButtons(_selectedType);
        }

        public void OnBoxClicked()
        {
            SetBottomButtons(CurrencyController.Type.BoxSkin);
        }

        public void OnTailClicked()
        {
            SetBottomButtons(CurrencyController.Type.TailSkin);
        }

        public void OnTapClicked()
        {
            SetBottomButtons(CurrencyController.Type.TapSkin);
        }

        public void OnBackgroundClicked()
        {
            SetBottomButtons(CurrencyController.Type.BackgroundSkin);
        }

        private async void BuyRandomSkin()
        {
            var randomSkin = GameManager.Instance.Progress.SkinDatas.FirstOrDefault(skin => skin.WayToGet == CurrencyController.Type.RandomSkin && !skin.IsOpen && skin.Type == _selectedType);
            if (randomSkin != null && GameManager.Instance.CurrencyController.GetCurrency(CurrencyController.Type.Coin) >= GetPriceForRandomSkin())
            {
                GameManager.Instance.CurrencyController.AddSkin(randomSkin.WayToGet, randomSkin.Type, randomSkin.SkinAddressableName);
                Messenger<CurrencyController.Type, string>.Broadcast(Constants.Events.OnGetRandomSkin, randomSkin.WayToGet, randomSkin.SkinAddressableName);
                GameManager.Instance.CurrencyController.RemoveCurrency(CurrencyController.Type.Coin, GetPriceForRandomSkin());
                SetBottomButtons(randomSkin.Type);
                await GameManager.Instance.Progress.Save();

            }
        }

        private async void OnRewardedAdDone(string placeId)
        {
            if (placeId != ID)
            {
                return;
            }

            GameManager.Instance.CurrencyController.AddCurrency(CurrencyController.Type.Coin, _coinsCountForWatchAd);
            await GameManager.Instance.Progress.Save();
            SetBottomButtons(_selectedType);
        }

        private int GetPriceForRandomSkin()
        {
            return 400;
        }

        private void SetBottomButtons(CurrencyController.Type type)
        {
            _selectedType = type;
            if (!GameManager.Instance.Mediation.IsReady(Constants.Ads.Rewarded) && !GameManager.Instance.Mediation.IsReady(Constants.Ads.Interstitial))
            {
                _getCoinsByRewardedAd.interactable = false;
                return;
            }

            var randomSkin = GameManager.Instance.Progress.SkinDatas.FirstOrDefault(skin => skin.WayToGet == CurrencyController.Type.RandomSkin && !skin.IsOpen && skin.Type == _selectedType);
            _getRandomSkinByCoins.gameObject.SetActive(randomSkin != null);
            _getRandomSkinByCoins.interactable = GameManager.Instance.CurrencyController.GetCurrency(CurrencyController.Type.Coin) >= GetPriceForRandomSkin();
            _getRandomSkinByCoinsText.text = GetPriceForRandomSkin().ToString();
        }

        private async void GetCoinsForAd()
        {
#if UNITY_EDITOR
            GameManager.Instance.CurrencyController.AddCurrency(CurrencyController.Type.Coin, _coinsCountForWatchAd);
            await GameManager.Instance.Progress.Save();
            SetBottomButtons(_selectedType);

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

        private void OnGetRandomSkin(CurrencyController.Type arg1, string arg2)
        {
            Setup();
        }
        
        private void OnGetSkinByLevel(int level)
        {
            Setup();
        }

        private void Setup()
        {
            foreach (var button in _topButtons)
            {
                button.Initialize();
                button.OnClick = null;
                button.OnClick += OnTopButtonStateChanged;
            }

            SetPage(_boxesSkinsButtons);
            SetPage(_backgroundsSkinsButtons);
            SetPage(_tailButtons);
            SetPage(_tapButtons);
        }

        private void OnTopButtonStateChanged(StateButton arg1, bool arg2)
        {
            foreach (var button in _topButtons)
            {
                button.SetState(button == arg1);
            }
        }

        private void SetPage(List<SkinsButton> buttons)
        {
            foreach (var button in buttons)
            {
                SetSkinData(button);
            }

            foreach (var button in buttons)
            {
                button.Setup();
            }
        }

        private void SetSkinData(SkinsButton button)
        {
            var data = GameManager.Instance.Progress.SkinDatas.FirstOrDefault(x
                => x.SkinAddressableName == button.GetSkinData().SkinAddressableName);
            
            if (data == null)
            {
                var forCopy = button.GetSkinData();

                GameManager.Instance.Progress.SkinDatas.Add(new SkinData()
                {
                    IsOpen = forCopy.IsOpen,
                    Price = forCopy.Price,
                    Size = forCopy.Size,
                    SkinAddressableName = forCopy.SkinAddressableName,
                    WayToGet = forCopy.WayToGet,
                    Type = forCopy.Type,
                });
            }
            else
            {
                var forCopy = button.GetSkinData();
                data.Price = forCopy.Price;
                data.Size = forCopy.Size;
                data.SkinAddressableName = forCopy.SkinAddressableName;
                data.WayToGet = forCopy.WayToGet;
                data.Type = forCopy.Type;
                button.SetSkinData(data);
            }
        }

        private void Start()
        {
            ID = Constants.PopUps.SkinsPopUp;
            Priority = 1;
        }

        public override async void Show()
        {
            gameObject.SetActive(true);
            await counter.UpdateLayout();
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Messenger<CurrencyController.Type, string>.RemoveListener(Constants.Events.OnGetRandomSkin, OnGetRandomSkin);
            GameManager.Instance.PlayerLevelManager.OnLevelChanged -= OnGetSkinByLevel;

            _isReady?.Dispose();
        }
    }
}