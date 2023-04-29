using System.Collections.Generic;
using System.Linq;
using Core.MessengerStatic;
using Currency;
using Cysharp.Threading.Tasks;
using DefaultNamespace.Managers;
using DefaultNamespace.UI.WinWindow;
using Managers;
using UI;
using UI.WinWindow;
using UnityEngine;
using UnityEngine.UI;

public class WinWindow : PopUpBase
{
    [SerializeField] private Image _background;
    [SerializeField] private Slider progress;
    [SerializeField] private Button goNextButton;
    [SerializeField] private Button _getForAds;
    [SerializeField] private GameObject winVFX;
    [SerializeField] private Button _loseButton;
    [SerializeField] private GameObject winCoinVFX;
    [SerializeField] private List<RewardView> rewardViews;
    [SerializeField] private CurrencyCounter _currencyCounter;

    private List<RewardViewSetting> _settings;
    private float _sliderProgressTarget;
    private bool _reset;

    public override void Initialize()
    {
        ID = Constants.PopUps.WinPopUp;
        Priority = 1;
        GameManager.Instance.SkinsManager.AddBackground(_background);
        GameManager.Instance.SkinsManager.SetBackgroundSkinSprite(_background);

        Messenger<string>.AddListener(Constants.Events.OnRewardedVideoReward, OnRewardedAdDone);
    }

    public override async void Show()
    {
        SetActive(true);
    }

    public override void Close()
    {
        rewardViews[GameManager.Instance.Progress.NextRewardIndexWinWindow].SetActiveReward(false);
        SetActive(false);
    }

    private async void SetActive(bool value)
    {
        gameObject.SetActive(value);
        if (!value) return;

        Setup();

        await MakeProgress();
        goNextButton.gameObject.SetActive(true);
    }

    private async UniTask MakeProgress()
    {
        await _currencyCounter.UpdateLayout();
        await UniTask.Delay(1000);

        var nearestPercent = SetNextNearestPercent();

        rewardViews[GameManager.Instance.Progress.NextRewardIndexWinWindow].SetActiveReward(true);
        float duration = 20f;
        float startTime = Time.time;
        while (progress.value < _sliderProgressTarget)
        {
            var t = (Time.time - startTime) / duration;
            await UniTask.WaitForEndOfFrame(this);
            progress.value = Mathf.SmoothStep(progress.value, nearestPercent.Percent + 1, t);
            //progress.value = Mathf.SmoothDamp(progress.value, nearestPercent.Percent + 1, ref yVelocity, 2);
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
        rewardViews[GameManager.Instance.Progress.NextRewardIndexWinWindow].UpdateRewardPercentText(
            nearestPercent.RewardType == CurrencyController.Type.RandomSkin ? "New skin" : nearestPercent.RewardType.ToString());
        rewardViews[GameManager.Instance.Progress.NextRewardIndexWinWindow].SetTokState(true);
        rewardViews[GameManager.Instance.Progress.NextRewardIndexWinWindow].SetActiveVFX(true);
        winVFX.SetActive(true);
        await GiveReward(nearestPercent);
        winVFX.SetActive(false);
    }

    private async UniTask GiveReward(RewardViewSetting settings)
    {
        Debug.LogError(settings.RewardType);
        switch (settings.RewardType)
        {
            case CurrencyController.Type.Coin:
                GameManager.Instance.CurrencyController.AddCurrency(settings.RewardType, settings.RewardCount);
                await UniTask.WaitUntil(_currencyCounter.IsAnimationComplete);
                break;
            case CurrencyController.Type.RandomSkin:
                Debug.LogError(settings.RewardType);

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

        var randomSkin = GameManager.Instance.Progress.SkinDatas.FirstOrDefault(skin => skin.WayToGet == CurrencyController.Type.RandomSkin && !skin.IsOpen);

        if (randomSkin == null)
        {
            GetCoinsRewardInLastSlot();
        }
        else
        {
            _reset = true;
            _getForAds.gameObject.SetActive(true);
            await UniTask.Delay(1000);
            _loseButton.gameObject.SetActive(true);
        }
    }

    private void GetCoinsRewardInLastSlot()
    {
        ProgressToEnd();
        var settings = GameManager.Instance.CurrencyController.GetRewardSettings();
        var max = settings.Max(x => x.RewardCount) * 1.5f;
        var rewardCount = (GameManager.Instance.GameField.GetCountOfReward() / 100) * max;
        GameManager.Instance.CurrencyController.AddCurrency(CurrencyController.Type.Coin, (int)rewardCount);
        goNextButton.gameObject.SetActive(true);
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

        if (GameManager.Instance.Mediation.IsReady(Constants.Ads.Rewarded))
        {
            Debug.LogError("----show r");
            GameManager.Instance.Mediation.Show(Constants.Ads.Rewarded, ID);
            return;
        }

        if (GameManager.Instance.Mediation.IsReady(Constants.Ads.Interstitial))
        {
            Debug.LogError("----show i");
            GameManager.Instance.Mediation.Show(Constants.Ads.Interstitial, ID);
            return;
        }
    }

    private void OnRewardedAdDone(string placeId)
    {
        if (placeId != ID)
        {
            return;
        }

        var randomSkin = GameManager.Instance.Progress.SkinDatas.FirstOrDefault(skin => skin.WayToGet == CurrencyController.Type.RandomSkin && !skin.IsOpen);
        GameManager.Instance.CurrencyController.AddSkin(randomSkin.WayToGet, randomSkin.Type, randomSkin.SkinAddressableName);
        Messenger<CurrencyController.Type, string>.Broadcast(Constants.Events.OnGetRandomSkin, randomSkin.WayToGet, randomSkin.SkinAddressableName);
        ProgressToEnd();
        goNextButton.gameObject.SetActive(true);
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
            Debug.Log("------------" + 0);
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
        Debug.Log("------------" + x);

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

    private async void Setup()
    {
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
}