using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private GameObject winVFX;
    [SerializeField] private GameObject winCoinVFX;
    [SerializeField] private List<RewardView> rewardViews;
    [SerializeField] private CurrencyCounter _currencyCounter;

    private List<RewardViewSetting> _settings;
    private float _sliderProgressTarget;

    public override void Initialize()
    {
        ID = Constants.PopUps.WinPopUp;
        Priority = 1;
        GameManager.Instance.SkinsManager.AddBackground(_background);
        GameManager.Instance.SkinsManager.SetBackgroundSkinSprite(_background);
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
        goNextButton.interactable = true;
    }

    private async UniTask MakeProgress()
    {
        await _currencyCounter.UpdateLayout();
        await UniTask.Delay(1000);

        var nearestPercent = SetNextNearestPercent();

        rewardViews[GameManager.Instance.Progress.NextRewardIndexWinWindow].SetActiveReward(true);

        var yVelocity = 0f;
        while (progress.value < _sliderProgressTarget)
        {
            await UniTask.WaitForEndOfFrame(this);
            progress.value = Mathf.SmoothDamp(progress.value, nearestPercent.Percent + 1, ref yVelocity, 2);

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
        switch (settings.RewardType)
        {
            case CurrencyController.Type.Coin:
                GameManager.Instance.CurrencyController.AddCurrency(settings.RewardType, settings.RewardCount);
                await UniTask.WaitUntil(_currencyCounter.IsAnimationComplete);
                break;
            case CurrencyController.Type.RandomSkin:
                GetSkinRandomSkin();
                break;
        }
    }

    private void GetSkinRandomSkin()
    {
        var randomSkin = GameManager.Instance.Progress.SkinDatas.FirstOrDefault(skin => skin.IsRandom && !skin.IsOpen);
        var settings = GameManager.Instance.CurrencyController.GetRewardSettings();
        
        if (randomSkin == null) 
        {
            var max = settings.Max(x => x.RewardCount) * 1.5f;
            var rewardCount = (GameManager.Instance.GameField.GetCountOfReward() / 100) * max;
            GameManager.Instance.CurrencyController.AddCurrency(CurrencyController.Type.Coin, (int)rewardCount);
            return;
        }

        GameManager.Instance.CurrencyController.AddSkin(randomSkin.WayToGet, randomSkin.Type, randomSkin.SkinAddressableName);
        Core.MessengerStatic.Messenger<CurrencyController.Type, string>.Broadcast(Constants.Events.OnGetRandomSkin, randomSkin.WayToGet, randomSkin.SkinAddressableName);
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

        // var setting = _settings.FirstOrDefault(x => x.Percent >= progress.value);
        // if (setting == null)
        // {
        //     progress.value = progress.minValue;
        //     _sliderProgressTarget -= progress.maxValue;
        //     SetNextIndex(0);
        //
        //     for (var i = 0; i < _settings.Count; i++)
        //     {
        //         rewardViews[i].SetTokState(progress.value >= _settings[i].Percent);
        //     }
        //
        //     return _settings[0];
        // }

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

    private async void Setup()
    {
        goNextButton.interactable = false;
        goNextButton.onClick.RemoveAllListeners();
        goNextButton.onClick.AddListener(() =>
        {
            GameManager.Instance.LoadNextLevel();
            GameManager.Instance.UIManager.ClosePopUp(ID);
        });

        var scrollRect = progress.GetComponent<RectTransform>();

        _settings = GameManager.Instance.CurrencyController.GetRewardSettings();
        var size = scrollRect.sizeDelta.x / 100;

        progress.value = GameManager.Instance.Progress.CurrentWinWindowsProgress;
        _sliderProgressTarget = GameManager.Instance.Progress.CurrentWinWindowsProgress;
        _sliderProgressTarget += GameManager.Instance.GetWinProgress();

        for (var i = _settings.Count; i < _settings.Count; i++)
        {
            rewardViews[i].SetActiveObject(false);
            rewardViews[i].SetActiveVFX(false);
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