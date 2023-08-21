using System;
using System.Collections;
using System.Collections.Generic;
using Boxes;
using Core.MessengerStatic;
using Cysharp.Threading.Tasks;
using Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameLevelUI : MonoBehaviour, IInitializable
{
    [SerializeField] private TMP_Text currentLevel;
    [SerializeField] private TMP_Text nextLevel;
    [SerializeField] private Slider progress;

    public void Initialize()
    {
        Messenger<string>.AddListener(Constants.Events.OnLevelCreated, OnLevelChanged);
        Messenger<BaseBox>.AddListener(Constants.Events.OnBoxRemoveFromGameField, OnBoxRemove);
    }

    private void OnLevelChanged(string obj)
    {
        int.TryParse(GameManager.Instance.Progress.LastStartedLevelID, out var level);
        currentLevel.text = level.ToString();
        nextLevel.text = (level + 1).ToString();
        progress.maxValue = GameManager.Instance.GameField.GetBoxCountOnStart();
        OnBoxRemove(null);
    }
    
    private void OnBoxRemove(BaseBox obj)
    {
        progress.value = GameManager.Instance.GameField.GetBoxCountOnStart() - GameManager.Instance.GameField.GetCurrentBoxCount();

        if (Math.Abs(progress.value - GameManager.Instance.GameField.GetBoxCountOnStart()) == 0)
        {
            ProgressToZero();
        }
    }

    private async UniTaskVoid ProgressToZero()
    {
        float decresePercent = progress.maxValue / 50;
        while (progress.value > 0)
        {
            await UniTask.WaitForFixedUpdate();
            progress.value -= decresePercent;
        }
    }

    private void OnDestroy()
    {
        Messenger<string>.RemoveListener(Constants.Events.OnLevelCreated, OnLevelChanged);
        Messenger<BaseBox>.RemoveListener(Constants.Events.OnBoxRemoveFromGameField, OnBoxRemove);
    }
}