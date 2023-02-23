using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DefaultNamespace.UI.WinWindow;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinWindow : MonoBehaviour
{
    [SerializeField] private Slider progress;

    [Header("CurrentReward")] [SerializeField]
    private GameObject currentRewardGameObject;

    [SerializeField] private Image currentRewardImage;
    [SerializeField] private TMP_Text currentRewardText;
    [Space] [SerializeField] private List<RewardView> rewardViews;

    private List<RewardViewSetting> _settings;

    public async void SetActive(bool value)
    {
        gameObject.SetActive(value);
        if (!value) return;

        Setup();
        await MakeProgress();
    }

    private async UniTask MakeProgress()
    {
        await UniTask.Delay(1000);

        var nearestPercent = SetNextNearestPercent();
        while (progress.value < Game.Instance.Progress.NextWinWindowsProgress)
        {
            await UniTask.Yield();
            progress.value += 0.2f;
            
            if (progress.value >= 100)
            {
                ResetProgress();
            }
            
            if (progress.value >= nearestPercent.Percent)
            {
                GetReward(nearestPercent);
                nearestPercent = SetNextNearestPercent();
            }
        }

        Game.Instance.Progress.CurrentWinWindowsProgress = progress.value;
    }

    private void GetReward(RewardViewSetting settings)
    {
        Debug.LogError("--------giv reward " + settings.RewardType + " " + settings.RewardCount + " " + settings.Percent + " " + settings.IsBig);
    }

    private RewardViewSetting SetNextNearestPercent()
    {
        var x = _settings.FirstOrDefault(x => x.Percent >= progress.value);

        if (x != null) 
            return x;
        
        return _settings[0];
    }

    private void ResetProgress()
    {
        Game.Instance.Progress.NextWinWindowsProgress =  Game.Instance.Progress.NextWinWindowsProgress - 100;
        progress.value = 0;
    }

    private void Setup()
    {
        var scrollRect = progress.GetComponent<RectTransform>();

        _settings = Game.Instance.CurrencyController.GetRewardSettings();
        var size = scrollRect.sizeDelta.x / 100;

        progress.value = Game.Instance.Progress.CurrentWinWindowsProgress;
        Game.Instance.Progress.NextWinWindowsProgress += 30;


        for (int i = _settings.Count; i < _settings.Count; i++)
        {
            rewardViews[i].SetActive(false);
        }

        for (int i = 0; i < _settings.Count; i++)
        {
            rewardViews[i].SetActive(true);
            rewardViews[i].Setup(_settings[i]);
            rewardViews[i].transform.localPosition = new Vector3(scrollRect.transform.localPosition.x + size * _settings[i].Percent, 0, 0);
        }
    }
}