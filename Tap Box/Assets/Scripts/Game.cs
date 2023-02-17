using System;
using LevelCreator;
using SaveLoad_progress;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance;

    public GameField GameField;
    public LevelsWindiw LevelsWindiw;
    public InputController InputController;
    public GameProgress Progress;
    
    private async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Progress = new GameProgress();
        }

        Progress.Path = Application.dataPath + "/Prefabs/LevelAssets/";
        Progress.PathProgress = Application.dataPath + "/Prefabs/GameProgress.txt";
        await Progress.Load();

        LevelsWindiw.Setup();
    }

    public async void UpdateLevel(LevelData level)
    {
        LevelsWindiw.UpdateLevel(level);
        UpdateProgress(level);
        await Progress.Save();
    }
    
    private void UpdateProgress(LevelData level)
    {
        var updateLevelButton = Game.Instance.Progress.LevelDatas.Find(x => x.ID == level.ID);

        if (updateLevelButton == null)
            return;

        updateLevelButton.LevelStatus = level.LevelStatus;
        updateLevelButton.BestResult = level.BestResult;
    }

    private async void OnDisable()
    {
        Debug.LogError("Save disable");

        await Progress.Save();
    }

    private async void OnDestroy()
    {
        Debug.LogError("Save destroy");
        await Progress.Save();
    }
}
