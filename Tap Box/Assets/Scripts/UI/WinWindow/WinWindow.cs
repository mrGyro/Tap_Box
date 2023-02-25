using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DefaultNamespace.UI.WinWindow;
using UnityEngine;
using UnityEngine.UI;

public class WinWindow : MonoBehaviour
{
    [SerializeField] private Slider progress;
    [SerializeField] private Button goNextButton;
    [SerializeField] private GameObject winVFX;
    [SerializeField] private GameObject winCoinVFX;
    [SerializeField] private List<RewardView> rewardViews;

    private List<RewardViewSetting> _settings;
    private float _sliderProgressTarget;

    public async void SetActive(bool value)
    {
        gameObject.SetActive(value);
        if (!value) return;

        Setup();
        await MakeProgress();
        goNextButton.interactable = true;
    }

    private async UniTask MakeProgress()
    {
        await UniTask.Delay(1000);

        var nearestPercent = SetNextNearestPercent();
        rewardViews[Game.Instance.Progress.NextRewardIndexWinWindow].SetActiveReward(true);

        var yVelocity = 0f;
        while (progress.value < _sliderProgressTarget)
        {
            await UniTask.WaitForEndOfFrame(this);
            progress.value = Mathf.SmoothDamp(progress.value, nearestPercent.Percent + 1, ref yVelocity, 2);

            if (progress.value > nearestPercent.Percent)
            {
                await GetReward(nearestPercent);
                nearestPercent = SetNextNearestPercent();
            }

            rewardViews[Game.Instance.Progress.NextRewardIndexWinWindow].UpdateRewardText(((int)GetPercents()).ToString());
        }

        Game.Instance.Progress.CurrentWinWindowsProgress = progress.value;
        await Game.Instance.Progress.Save();
    }

    private async UniTask GetReward(RewardViewSetting nearestPercent)
    {
        rewardViews[Game.Instance.Progress.NextRewardIndexWinWindow].UpdateRewardText("100");
        rewardViews[Game.Instance.Progress.NextRewardIndexWinWindow].SetTokState(true);
        rewardViews[Game.Instance.Progress.NextRewardIndexWinWindow].SetActiveVFX(true);
        winVFX.SetActive(true);
        await Game.Instance.GetReward(nearestPercent);
        winVFX.SetActive(false);
    }

    private float GetPercents()
    {
        if (Game.Instance.Progress.NextRewardIndexWinWindow == 0)
            return (progress.value / _settings[0].Percent) * 100;

        var previousIndex = Game.Instance.Progress.NextRewardIndexWinWindow - 1;
        var x = (progress.value - _settings[previousIndex].Percent) / (_settings[Game.Instance.Progress.NextRewardIndexWinWindow].Percent - _settings[previousIndex].Percent);
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
                rewardViews[i].SetTokState(progress.value >= _settings[i].Percent);
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
        rewardViews[Game.Instance.Progress.NextRewardIndexWinWindow].SetActiveReward(false);
        rewardViews[Game.Instance.Progress.NextRewardIndexWinWindow].SetActiveVFX(false);
        Game.Instance.Progress.NextRewardIndexWinWindow = index;
        rewardViews[Game.Instance.Progress.NextRewardIndexWinWindow].SetActiveReward(true);
    }

    private async void Setup()
    {
        goNextButton.interactable = false;
        goNextButton.onClick.RemoveAllListeners();
        goNextButton.onClick.AddListener(() =>
        {
            Game.Instance.LoadNextLevel();
            gameObject.SetActive(false);
        });
        
        var scrollRect = progress.GetComponent<RectTransform>();

        _settings = Game.Instance.CurrencyController.GetRewardSettings();
        var size = scrollRect.sizeDelta.x / 100;

        progress.value = Game.Instance.Progress.CurrentWinWindowsProgress;
        _sliderProgressTarget = Game.Instance.Progress.CurrentWinWindowsProgress;
        _sliderProgressTarget += Game.Instance.GetWinProgress();

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