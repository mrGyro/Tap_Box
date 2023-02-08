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

    private void OnEnable()
    {
        Setup();
    }

    private async void Setup()
    {
        RemoveAllButtons();
        var x = await SaveLoadGameProgress.LoadGameProgress();
        _levelsData = x.LevelDatas;

        await Addressables.LoadAssetsAsync<TextAsset>("Levels", Callback);
        CreateLevelButtons();
    }

    private void Callback(TextAsset asset)
    {
        // var levelData = await SaveLoadGameProgress.LoadLevelDataText(asset);
        // string level = asset.name.Remove(0, asset.name.LastIndexOf('_') + 1);
        // _levelsData.Add(new LevelButtonData()
        // {
        //     levelNumberText = level,
        //     Data = levelData
        // });
    }

    private async void CreateLevelButtons()
    {
        _levelsData.Sort(Compare);

        foreach (var levelData in _levelsData)
        {
            var level = await levelsPool.Get(levelData.LevelStatus);
            level.Setup(levelData);

           // _levelsDataButtons.Add(g);
        }
    }

    private void RemoveAllButtons()
    {
        // for (int i = _levelsDataButtons.Count - 1; i >= 0; i--)
        // {
        //     Destroy(_levelsDataButtons[i]);
        // }
        //
        // _levelsDataButtons.Clear();
        // _levelsData.Clear();
    }

    private int Compare(LevelData b1, LevelData b2)
    {
        int x, y;
        int.TryParse(b1.ID, out x);
        int.TryParse(b2.ID, out y);
        if (x > y)
            return 1;
        if (x < y)
            return -1;

        return 0;
    }


}