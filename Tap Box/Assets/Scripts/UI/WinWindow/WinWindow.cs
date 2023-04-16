using System.Collections.Generic;
using System.Linq;
using Currency;
using Cysharp.Threading.Tasks;
using DefaultNamespace.Managers;
using DefaultNamespace.UI.WinWindow;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class WinWindow : PopUpBase
{
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
    }

    public override async void Show()
    {
        SetActive(true);

    }

    public override void Close()
    {
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

        rewardViews[Managers.Instance.Progress.NextRewardIndexWinWindow].SetActiveReward(true);

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

            rewardViews[Managers.Instance.Progress.NextRewardIndexWinWindow].UpdateRewardPercentText(((int)GetPercents()).ToString());
        }

        Managers.Instance.Progress.CurrentWinWindowsProgress = progress.value;
        await Managers.Instance.Progress.Save();
    }

    private async UniTask GetRewardFromSettings(RewardViewSetting nearestPercent)
    {
        rewardViews[Managers.Instance.Progress.NextRewardIndexWinWindow].UpdateRewardPercentText("100");
        rewardViews[Managers.Instance.Progress.NextRewardIndexWinWindow].SetTokState(true);
        rewardViews[Managers.Instance.Progress.NextRewardIndexWinWindow].SetActiveVFX(true);
        winVFX.SetActive(true);
        await GiveReward(nearestPercent);
        winVFX.SetActive(false);
    }

    private async UniTask GiveReward(RewardViewSetting settings)
    {
        switch (settings.RewardType)
        {
            case CurrencyController.Type.Coin:
                Managers.Instance.CurrencyController.AddCurrency(settings.RewardType, settings.RewardCount);
                await UniTask.WaitUntil(_currencyCounter.IsAnimationComplete);
                break;
            case CurrencyController.Type.RandomSkin:
                GetSkinRandomSkin();
                break;
        }
    }

    private void GetSkinRandomSkin()
    {
        var randomSkin = Managers.Instance.Progress.SkinDatas.FirstOrDefault(skin => skin.IsRandom && !skin.IsOpen);
        if (randomSkin == null)
        {
            var settings = Managers.Instance.CurrencyController.GetRewardSettings();
            var max = settings.Max(x => x.RewardCount) * 1.5f;
            var rewardCount = (Managers.Instance.GameField.GetCountOfReward() / 100) * max;
            Managers.Instance.CurrencyController.AddCurrency(CurrencyController.Type.Coin, (int)rewardCount);
            return;
        }

        Managers.Instance.CurrencyController.AddSkin(randomSkin.Type, randomSkin.SkinAddressableName);
        Core.MessengerStatic.Messenger<CurrencyController.Type, string>.Broadcast(Constants.Events.OnGetRandomSkin, randomSkin.Type, randomSkin.SkinAddressableName);
    }

    private float GetPercents()
    {
        if (Managers.Instance.Progress.NextRewardIndexWinWindow == 0)
            return (progress.value / _settings[0].Percent) * 100;

        var previousIndex = Managers.Instance.Progress.NextRewardIndexWinWindow - 1;
        var x = (progress.value - _settings[previousIndex].Percent) / (_settings[Managers.Instance.Progress.NextRewardIndexWinWindow].Percent - _settings[previousIndex].Percent);

        return x * 100;
    }

    private RewardViewSetting SetNextNearestPercent()
    {
        if (progress.value >= progress.maxValue)
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

        var setting = _settings.FirstOrDefault(x => x.Percent >= progress.value);
        if (setting == null)
        {
            progress.value = progress.minValue;
            _sliderProgressTarget -= progress.maxValue;
            SetNextIndex(0);

            for (var i = 0; i < _settings.Count; i++)
                rewardViews[i].SetTokState(progress.value >= _settings[i].Percent);

            return _settings[0];
        }

        var x = _settings.IndexOf(setting);
        SetNextIndex(x);

        return _settings[x];
    }

    private void SetNextIndex(int index)
    {
        rewardViews[Managers.Instance.Progress.NextRewardIndexWinWindow].SetActiveReward(false);
        rewardViews[Managers.Instance.Progress.NextRewardIndexWinWindow].SetActiveVFX(false);
        Managers.Instance.Progress.NextRewardIndexWinWindow = index;
        rewardViews[Managers.Instance.Progress.NextRewardIndexWinWindow].SetActiveReward(true);
    }

    private async void Setup()
    {
        goNextButton.interactable = false;
        goNextButton.onClick.RemoveAllListeners();
        goNextButton.onClick.AddListener(() =>
        {
            Managers.Instance.LoadNextLevel();
            Managers.Instance.UIManager.ClosePopUp(ID);
        });

        var scrollRect = progress.GetComponent<RectTransform>();

        _settings = Managers.Instance.CurrencyController.GetRewardSettings();
        var size = scrollRect.sizeDelta.x / 100;

        progress.value = Managers.Instance.Progress.CurrentWinWindowsProgress;
        _sliderProgressTarget = Managers.Instance.Progress.CurrentWinWindowsProgress;
        _sliderProgressTarget += Managers.Instance.GetWinProgress();

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