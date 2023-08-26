using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Managers;
using LevelCreator;
using Managers;
using UI.Levels;
using UnityEngine;
using UnityEngine.UI;

public class LevelsWindiw : PopUpBase
{
    [SerializeField] private Image _background;
    [SerializeField] private LevelsPool levelsPool;
    public List<UILevelItem> uiLevelItems = new();

    public override void Initialize()
    {
        ID = Constants.PopUps.LevelListPopUp;
        Priority = 1;
        CreateLevelButtons();
    }

    public override void Close()
    {
        gameObject.SetActive(false);
    }

    public override void Show()
    {
        UpdateLevel();
        gameObject.SetActive(true);
    }

    private void UpdateLevel()
    {
        for (var index = 0; index < GameManager.Instance.Progress.LevelDatas.Count; index++)
        {
            var data = GameManager.Instance.Progress.LevelDatas[index];
            var level = uiLevelItems.FirstOrDefault(x => x.Data.ID == data.ID);
            if (level == null)
                continue;

            level.Data.UpdateData(level.Data);
            UpdateLevelsButton(level.Data, index);

            if (!level.Data.Reqirement.CheckForDone())
                continue;

            level.Setup(data, index);
        }
    }

    private async void CreateLevelButtons()
    {
        GameManager.Instance.Progress.LevelDatas.Sort(Compare);

        for (var index = 0; index < GameManager.Instance.Progress.LevelDatas.Count; index++)
        {
            var levelData = GameManager.Instance.Progress.LevelDatas[index];
            var level = await levelsPool.Get();
            level.Setup(levelData, index);
            uiLevelItems.Add(level);
        }
    }

    private int Compare(LevelData b1, LevelData b2)
    {
        int.TryParse(b1.ID, out var x);
        int.TryParse(b2.ID, out var y);

        if (x > y) return 1;
        if (x < y) return -1;

        return 0;
    }

    private void UpdateLevelsButton(LevelData levelData, int index)
    {
        var updateLevelButton = uiLevelItems.Find(x => x.Data.ID == levelData.ID);

        if (updateLevelButton == null)
            return;

        updateLevelButton.Setup(levelData, index);
    }

    // private bool CheckForDone(Reqirement requirement)
    // {
    //     switch (requirement.Type)
    //     {
    //         case Reqirement.RequirementType.PassedLevel:
    //             var level = uiLevelItems.Find(x => x.Data.ID == requirement.Value);
    //             Debug.LogError("----");
    //             if (level != null && level.Data.LevelStatus == Status.Passed)
    //             {
    //                 return true;
    //             }
    //
    //             break;
    //     }
    //
    //     return false;
    // }
}