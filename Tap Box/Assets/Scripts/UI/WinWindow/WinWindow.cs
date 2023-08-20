using System.Collections.Generic;
using System.Linq;
using Core.MessengerStatic;
using Currency;
using Cysharp.Threading.Tasks;
using DefaultNamespace.Managers;
using DefaultNamespace.UI.WinWindow;
using Managers;
using Sounds;
using UI;
using UI.Skins;
using UI.WinWindow;
using UnityEngine;
using UnityEngine.UI;

public class WinWindow : PopUpBase
{
    [SerializeField] private WinWindowSmileAnimation _smile;
    [SerializeField] private Slider progress;
    [SerializeField] private Button goNextButton;
    [SerializeField] private Button _getForAds;
    [SerializeField] private GameObject _getForAdsAdsIcon;
    [SerializeField] private GameObject winVFX;
    [SerializeField] private Button _loseButton;
    [SerializeField] private List<RewardView> rewardViews;
    [SerializeField] private CurrencyCounter _currencyCounter;

    private List<RewardViewSetting> _settings;
    private float _sliderProgressTarget;
    private bool _reset;
    private SkinData _randomSkin;
    public override void Initialize()
    {
        ID = Constants.PopUps.WinPopUp;
        Priority = 1;
        Messenger<string>.AddListener(Constants.Events.OnRewardedVideoReward, OnRewardedAdDone);
        _currencyCounter.Initialize();
    }

    public override void Show()
    {
        _randomSkin = null;
        SetActive(true);
        _smile.Play();
    }

    public override void Close()
    {
        _smile.Stop();
        rewardViews[GameManager.Instance.Progress.NextRewardIndexWinWindow].SetActiveReward(false);
        SetActive(false);
    }

    private async void Setup()
    {
        GameManager.Instance.SoundManager.Play(new ClipDataMessage() { Id = Constants.Sounds.UI.WinWindowShow, SoundType = SoundData.SoundType.UI });
        goNextButton.gameObject.SetActive(false);
        goNextButton.onClick.RemoveAllListeners();
        goNextButton.onClick.AddListener(OnClose);

        _getForAds.onClick.RemoveAllListeners();
        _getForAds.onClick.AddListener(OnRewardAdClick);
        _getForAds.gameObject.SetActive(false);

        _loseButton.onClick.RemoveAllListeners();
        _loseButton.onClick.AddListener(OnClose);
        _loseButton.gameObject.SetActive(false);

        var scrollRect = progress.GetComponent<RectTransform>();

        _settings = GameManager.Instance.CurrencyController.GetRewardSettings();
        //For test last reward uncoment 
        //GameManager.Instance.Progress.NextRewardIndexWinWindow = _settings.Count - 1;
        //GameManager.Instance.Progress.CurrentWinWindowsProgress = _settings[GameManager.Instance.Progress.NextRewardIndexWinWindow].Percent - 10;
        //---------------------
        
        var size = scrollRect.sizeDelta.x / 100;

        progress.value = GameManager.Instance.Progress.CurrentWinWindowsProgress;
        _sliderProgressTarget = GameManager.Instance.Progress.CurrentWinWindowsProgress;
        _sliderProgressTarget += GameManager.Instance.GetWinProgress();

        for (var i = 0; i < _settings.Count; i++)
        {
            rewardViews[i].SetActiveObject(false);
            rewardViews[i].SetActiveVFX(false);
            rewardViews[i].SetActiveReward(false);
        }

        for (var i = 0; i < _settings.Count; i++)
        {
            rewardViews[i].SetActiveObject(true);
            await rewardViews[i].Setup(_settings[i]);
            rewardViews[i].SetTokState(progress.value >= _settings[i].Percent);
            rewardViews[i].transform.localPosition = new Vector3(scrollRect.transform.localPosition.x + size * _settings[i].Percent, 0, 0);
        }
    }

    private void ResetToDefault()
    {
        progress.value = 0;
        for (var i = 0; i < _settings.Count; i++)
        {
            rewardViews[i].SetTokState(false);
            rewardViews[i].SetActiveReward(false);
        }

        GameManager.Instance.Progress.CurrentWinWindowsProgress = 0;
        GameManager.Instance.Progress.NextRewardIndexWinWindow = 0;
    }

    private async void SetActive(bool value)
    {
        gameObject.SetActive(value);
        if (!value)
        {
            return;
        }

        Setup();
        
        _getForAdsAdsIcon.SetActive(!GameManager.Instance.IAPManager.HasNonConsumableProduct(Constants.IAP.NoAds));

        await MakeProgress();
        goNextButton.gameObject.SetActive(true);
    }

    private async UniTask MakeProgress()
    {
        //await UniTask.Delay(500);
        var nearestPercent = SetNextNearestPercent();
        rewardViews[GameManager.Instance.Progress.NextRewardIndexWinWindow].SetActiveReward(true);
        rewardViews[GameManager.Instance.Progress.NextRewardIndexWinWindow].UpdateRewardPercentText(((int)GetPercents()) + "%");

        float duration = 10f;
        float startTime = Time.time;
        while (progress.value < _sliderProgressTarget)
        {
            var t = (Time.time - startTime) / duration;
            await UniTask.WaitForEndOfFrame(this);
            progress.value = Mathf.SmoothStep(progress.value, nearestPercent.Percent + 1, t);
            if (progress.value > nearestPercent.Percent)
            {
                await GetRewardFromSettings(nearestPercent);
                break;
            }

            rewardViews[GameManager.Instance.Progress.NextRewardIndexWinWindow].UpdateRewardPercentText(((int)GetPercents()) + "%");
        }

        GameManager.Instance.Progress.CurrentWinWindowsProgress = progress.value;
        await GameManager.Instance.Progress.Save();
    }

    private async UniTask GetRewardFromSettings(RewardViewSetting nearestPercent)
    {
        _randomSkin = GameManager.Instance.Progress.SkinDatas.FirstOrDefault(skin => skin.WayToGet == CurrencyController.Type.RandomSkin && !skin.IsOpen);

        rewardViews[GameManager.Instance.Progress.NextRewardIndexWinWindow].UpdateRewardPercentText(
            nearestPercent.RewardType == CurrencyController.Type.RandomSkin
                ? _randomSkin == null ? "+" + GetCountOfLastRewardInCoins() : "New skin"
                : $"+{nearestPercent.RewardCount}");

        rewardViews[GameManager.Instance.Progress.NextRewardIndexWinWindow].SetTokState(true);
        rewardViews[GameManager.Instance.Progress.NextRewardIndexWinWindow].SetActiveVFX(true);
        winVFX.SetActive(true);

        await GiveReward(nearestPercent);
        winVFX.SetActive(false);
    }

    private async UniTask GiveReward(RewardViewSetting settings)
    {
        switch (settings.RewardType)
        {
            case CurrencyController.Type.Coin:
                GameManager.Instance.SoundManager.Play(new ClipDataMessage() { Id = Constants.Sounds.UI.WinWindowGetReward, SoundType = SoundData.SoundType.UI });

                _currencyCounter.CoinsAnimation(rewardViews[GameManager.Instance.Progress.NextRewardIndexWinWindow].GetCurrencyRoot());
                await UniTask.Delay(1000);
                GameManager.Instance.CurrencyController.AddCurrency(settings.RewardType, settings.RewardCount);

                await UniTask.Delay(500);
                await UniTask.WaitUntil(_currencyCounter.IsAnimationComplete);
                break;
            case CurrencyController.Type.RandomSkin:
                GameManager.Instance.SoundManager.Play(new ClipDataMessage() { Id = Constants.Sounds.UI.WinWindowGetSkinReward, SoundType = SoundData.SoundType.UI });

                GetSkinRandomSkin();
                break;
        }
    }

    private async void GetSkinRandomSkin()
    {
        if (!GameManager.Instance.Mediation.IsReady(Constants.Ads.Rewarded) && !GameManager.Instance.Mediation.IsReady(Constants.Ads.Interstitial))
        {
            _getForAds.gameObject.SetActive(false);
            return;
        }

        _reset = true;
        _getForAds.gameObject.SetActive(true);
        await UniTask.Delay(1000);
        _loseButton.gameObject.SetActive(true);
    }

    private async UniTask GetCoinsRewardInLastSlot()
    {
        ProgressToEnd();

        _currencyCounter.CoinsAnimation(rewardViews[GameManager.Instance.Progress.NextRewardIndexWinWindow].GetCurrencyRoot());
        await UniTask.Delay(1000);
        GameManager.Instance.CurrencyController.AddCurrency(CurrencyController.Type.Coin, GetCountOfLastRewardInCoins());


        await UniTask.Delay(500);
        await UniTask.WaitUntil(_currencyCounter.IsAnimationComplete);
        goNextButton.gameObject.SetActive(true);
    }

    private int GetCountOfLastRewardInCoins()
    {
        var settings = GameManager.Instance.CurrencyController.GetRewardSettings();
        var max = settings.Max(x => x.RewardCount) * 2f;
        return (int)Random.Range(max / 1.5f, max);
    }

    private async void ProgressToEnd()
    {
        float duration = 10f;
        float startTime = Time.time;
        while (progress.value < progress.maxValue)
        {
            var t = (Time.time - startTime) / duration;
            await UniTask.WaitForEndOfFrame(this);
            progress.value = Mathf.SmoothStep(progress.value, progress.maxValue, t);
        }
    }

    private void OnRewardAdClick()
    {
        _getForAds.gameObject.SetActive(false);
        _loseButton.gameObject.SetActive(false);
        ProgressToEnd();

        if (GameManager.Instance.IAPManager.HasNonConsumableProduct(Constants.IAP.NoAds))
        {
            GetLastReward();
            return;
        }
        
#if UNITY_EDITOR
        GetLastReward();
#endif

        if (GameManager.Instance.Mediation.IsReady(Constants.Ads.Rewarded))
        {
            GameManager.Instance.Mediation.Show(Constants.Ads.Rewarded, ID);
            return;
        }

        if (GameManager.Instance.Mediation.IsReady(Constants.Ads.Interstitial))
        {
            GameManager.Instance.Mediation.Show(Constants.Ads.Interstitial, ID);
            GetLastReward();
            return;
        }
    }

    private void OnRewardedAdDone(string placeId)
    {
        if (placeId != ID)
        {
            return;
        }

        GetLastReward();
    }

    private async void GetLastReward()
    {
        var randomSkin = GameManager.Instance.Progress.SkinDatas.Where(skin => skin.WayToGet == CurrencyController.Type.RandomSkin && !skin.IsOpen).ToList();
        if (randomSkin.Count > 0)
        {
            _randomSkin = randomSkin[Random.Range(0, randomSkin.Count)];
        }
        
        if (_randomSkin == null)
        {
            await GetCoinsRewardInLastSlot();
            return;
        }

        GameManager.Instance.CurrencyController.AddSkin(_randomSkin.WayToGet, _randomSkin.Type, _randomSkin.SkinAddressableName);
        Messenger<CurrencyController.Type, string>.Broadcast(Constants.Events.OnGetRandomSkin, _randomSkin.WayToGet, _randomSkin.SkinAddressableName);
        Messenger<CurrencyController.Type, string>.Broadcast(Constants.Events.OnGetNewSkin, _randomSkin.Type, _randomSkin.SkinAddressableName);
        ProgressToEnd();
        goNextButton.gameObject.SetActive(true);

        var sprite = await AssetProvider.LoadAssetAsync<Sprite>(_randomSkin.SkinAddressableName + "_icon");
        rewardViews[^1].SetRewardSprite(sprite);
        
        GameManager.Instance.UIManager.ShowUpToAllPopUp(Constants.PopUps.NewSkinPopUp);
        
    }

    private float GetPercents()
    {
        if (GameManager.Instance.Progress.NextRewardIndexWinWindow == 0)
            return (progress.value / _settings[0].Percent) * 100;

        var previousIndex = GameManager.Instance.Progress.NextRewardIndexWinWindow - 1;
        var x = (progress.value - _settings[previousIndex].Percent) / (_settings[GameManager.Instance.Progress.NextRewardIndexWinWindow].Percent - _settings[previousIndex].Percent);

        return x * 100;
    }

    private RewardViewSetting SetNextNearestPercent()
    {
        var setting = _settings.FirstOrDefault(x => x.Percent >= progress.value);
        if (progress.value >= progress.maxValue || setting == null)
        {
            SetNextIndex(0);
            progress.value = progress.minValue;
            _sliderProgressTarget -= progress.maxValue;

            for (var i = 0; i < _settings.Count; i++)
            {
                rewardViews[i].SetTokState(progress.value >= _settings[i].Percent);
            }

            return _settings[0];
        }

        var x = _settings.IndexOf(setting);
        SetNextIndex(x);

        return _settings[x];
    }

    private void SetNextIndex(int index)
    {
        rewardViews[GameManager.Instance.Progress.NextRewardIndexWinWindow].SetActiveReward(false);
        rewardViews[GameManager.Instance.Progress.NextRewardIndexWinWindow].SetActiveVFX(false);
        GameManager.Instance.Progress.NextRewardIndexWinWindow = index;
        rewardViews[GameManager.Instance.Progress.NextRewardIndexWinWindow].SetActiveReward(true);
    }

    private void OnClose()
    {
        GameManager.Instance.LoadNextLevel();
        GameManager.Instance.UIManager.ClosePopUp(ID);

        if (_reset)
        {
            ResetToDefault();
            _reset = false;
        }
    }
}