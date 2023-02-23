using System.Linq;
using Currency;
using LevelCreator;
using SaveLoad_progress;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance;

    public GameField GameField;
    public LevelsWindiw LevelsWindiw;
    public InputController InputController;
    public WinWindow WinWindow;
    public GameProgress Progress;
    public CurrencyController CurrencyController;

    private async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Progress = new GameProgress();
            CurrencyController = new CurrencyController();
        }

        await Progress.Load();
        LevelsWindiw.Setup();

        if (string.IsNullOrEmpty(Progress.LastStartedLevelID))
        {
            LoadNextLevel();
        }
        else
        {
            LoadLevelById(Progress.LastStartedLevelID);
        }
    }

    public async void SaveLevel(LevelData level)
    {
        LevelsWindiw.UpdateLevel(level);
        Updateevel(level);
        LevelsWindiw.CheckRequirement();

        await Progress.Save();
    }

    public void Updateevel(LevelData level)
    {
        var updateLevelButton = Instance.Progress.LevelDatas.FindIndex(x => x.ID == level.ID);

        if (updateLevelButton == -1)
            return;

        Progress.LevelDatas[updateLevelButton].LevelStatus = level.LevelStatus;
        Progress.LevelDatas[updateLevelButton].BestResult = level.BestResult;
    }

    public async void LoadNextLevel()
    {
        Progress.LastStartedLevelID = GetNextLevelId();
        GameField.LoadLevelByName(Progress.LastStartedLevelID);
        await Progress.Save();
    }

    public async void LoadLevelById(string id)
    {
        Progress.LastStartedLevelID = id;
        GameField.LoadLevelByName(Progress.LastStartedLevelID);
        await Progress.Save();
    }

    private string GetNextLevelId()
    {
        var index = Progress.LevelDatas.FirstOrDefault(x => x.LevelStatus == Status.Open) 
                    ?? Progress.LevelDatas.Last(x => x.LevelStatus == Status.Passed);

        return index.ID;
    }
    // private async void OnDisable()
    // {
    //     Debug.LogError("Save disable");
    //
    //     await Progress.Save();
    // }
    //
    // private async void OnDestroy()
    // {
    //     Debug.LogError("Save destroy");
    //     await Progress.Save();
    // }
}