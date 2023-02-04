using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DefaultNamespace.UI.Levels;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LevelsWindiw : MonoBehaviour
{
    private const string AddressablePassedLevelItem = "PassedLevelItem";
    private const string AddressableUnlockLevelItem = "UnlockableLevel";
    private const string AddressableLockLevelItem = "LockedLevelItem";

    private List<LevelButtonData> _levelsData = new();
    private List<GameObject> _levelsDataButtons = new();

    [SerializeField] private RectTransform _root;

    private void OnEnable()
    {
        Setup();
    }

    private async void Setup()
    {
        RemoveAllButtons();
        await Addressables.LoadAssetsAsync<TextAsset>("Levels", Callback);
        CreateLevelButtons();
    }

    private async void Callback(TextAsset asset)
    {
        var levelData = await GameField.LoadLevelDataText(asset);
        string level = asset.name.Remove(0, asset.name.LastIndexOf('_') + 1);
        _levelsData.Add(new LevelButtonData()
        {
            levelNumberText = level,
            Data = levelData
        });
    }

    private async void CreateLevelButtons()
    {
        _levelsData.Sort(Compare);
        foreach (var VARIABLE in _levelsData)
        {
            GameObject g = await InstantiateAssetAsync(AddressablePassedLevelItem);


            // g = await InstantiateAssetAsync(AddressableUnlockLevelItem);
            // g = await InstantiateAssetAsync(AddressableLockLevelItem);

            g.GetComponent<UILevelItem>().Setup(new LevelButtonData()
            {
                levelNumberText = VARIABLE.levelNumberText,
                status = VARIABLE.levelNumberText
            });

            _levelsDataButtons.Add(g);
        }
    }

    private void RemoveAllButtons()
    {
        for (int i = _levelsDataButtons.Count - 1; i >= 0; i--)
        {
            Destroy(_levelsDataButtons[i]);
        }

        _levelsDataButtons.Clear();
        _levelsData.Clear();
    }

    private int Compare(LevelButtonData b1, LevelButtonData b2)
    {
        int x, y;
        int.TryParse(b1.levelNumberText, out x);
        int.TryParse(b2.levelNumberText, out y);
        if (x > y)
            return 1;
        if (x < y)
            return -1;

        return 0;
    }

    private async UniTask<GameObject> InstantiateAssetAsync(string assetName)
    {
        var x = await AssetProvider.LoadAssetAsync<GameObject>(assetName);
        return x == null ? null : Instantiate(x, _root);
    }
}