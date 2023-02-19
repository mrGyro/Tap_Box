using System.Collections.Generic;
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

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);

        if (!value) return;
        progress.value = Game.Instance.Progress.CurrentWinWindowsProgress;
        progress.value = Game.Instance.Progress.CurrentWinWindowsProgress;
        Setup();
    }

    private void Setup()
    {
        var scrollRect = progress.GetComponent<RectTransform>();

        var settings = Game.Instance.CurrencyController.GetRewardSettings();
        var size = scrollRect.sizeDelta.x / 100;
        
        progress.value = settings[0].Percent;


        for (int i = settings.Count; i < settings.Count; i++)
        {
            rewardViews[i].SetActive(false);
        }
        for (int i = 0; i < settings.Count; i++)
        {
            rewardViews[i].SetActive(true);
            rewardViews[i].Setup(settings[i]);
            rewardViews[i].transform.localPosition = new Vector3(scrollRect.transform.localPosition.x + size * settings[i].Percent, 0, 0);
        }
    }
}