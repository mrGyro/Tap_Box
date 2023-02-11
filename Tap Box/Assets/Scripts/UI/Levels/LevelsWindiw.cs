using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LevelCreator;
using SaveLoad_progress;
using UI.Levels;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LevelsWindiw : MonoBehaviour
{
    [SerializeField] private LevelsPool levelsPool;
    [SerializeField] private RectTransform root;

    private List<LevelData> _levelsData = new();
    private List<UILevelItem> _uiLevelItems = new();

    public async void Setup()
    {
        var progress = await SaveLoadGameProgress.LoadGameProgress();
        _levelsData = progress.LevelDatas;

        await Addressables.LoadAssetsAsync<TextAsset>("Levels", Callback);
        CreateLevelButtons();
    }

    public void UpdateLevel(LevelData level)
    {
        var updateLevel = _uiLevelItems.Find(x => x.Data.ID == level.ID);
        if (updateLevel == null)
            return;

        updateLevel.Data.UpdateData(level);
        UpdateLevelsButton(level);
    }

    private void Callback(TextAsset asset)
    {
    }

    private async void CreateLevelButtons()
    {
        _levelsData.Sort(Compare);

        foreach (var levelData in _levelsData)
        {
            var level = await levelsPool.Get();
            level.Setup(levelData);
            _uiLevelItems.Add(level);
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
        var updateLevelButton = _uiLevelItems.Find(x => x.Data.ID == levelData.ID);

        if (updateLevelButton == null)
            return;

        updateLevelButton.UpdateButton(levelData);
    }
}