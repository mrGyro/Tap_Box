using System.Collections.Generic;
using LevelCreator;
using UI.Levels;
using UnityEngine;

public class LevelsWindiw : MonoBehaviour
{
    [SerializeField] private LevelsPool levelsPool;
    [SerializeField] private RectTransform root;

    public List<UILevelItem> uiLevelItems = new();

    public void Setup()
    { 
        CreateLevelButtons();
    }

    public void UpdateLevel(LevelData level)
    {
        var updateLevel = uiLevelItems.Find(x => x.Data.ID == level.ID);
        if (updateLevel == null)
            return;

        updateLevel.Data.UpdateData(level);
        UpdateLevelsButton(level);
    }

    public void CheckRequirement()
    {
        foreach (var level in uiLevelItems)
        {
            level.CheckRequirement();
        }
    }

    private async void CreateLevelButtons()
    {
        Game.Instance.Progress.LevelDatas.Sort(Compare);

        foreach (var levelData in Game.Instance.Progress.LevelDatas)
        {
            var level = await levelsPool.Get();
            level.Setup(levelData);
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

    private void UpdateLevelsButton(LevelData levelData)
    {
        var updateLevelButton = uiLevelItems.Find(x => x.Data.ID == levelData.ID);

        if (updateLevelButton == null)
            return;

        updateLevelButton.UpdateButton(levelData);
    }
}