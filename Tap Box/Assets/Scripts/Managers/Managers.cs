using System.Linq;
using Boxes;
using Currency;
using Cysharp.Threading.Tasks;
using DefaultNamespace.UI.WinWindow;
using LevelCreator;
using PlayerLevel;
using SaveLoad_progress;
using UnityEngine;

public class Managers : MonoBehaviour
{
    public static Managers Instance;

    public GameField GameField;
    public LevelsWindiw LevelsWindiw;
    public InputController InputController;
    public WinWindow WinWindow;
    public GameProgress Progress;
    public CurrencyController CurrencyController;
    public UIManager UIManager;
    public PlayerLevelManager PlayerLevelManager;

    private async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Progress = new GameProgress();
            CurrencyController = new CurrencyController();
            PlayerLevelManager = new PlayerLevelManager();
        }

        await Progress.Load();
        
        LevelsWindiw.Setup();
        GameField.Initialize();
        PlayerLevelManager.Initialize();
        UIManager.Initialize();

        if (string.IsNullOrEmpty(Progress.LastStartedLevelID))
        {
            LoadNextLevel();
        }
        else
        {
            LoadLevelById(Progress.LastStartedLevelID);
        }
        
    }

    private void Handler()
    {
        Debug.LogError(1);
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
        await GameField.LoadLevelByName(Progress.LastStartedLevelID);
        await Progress.Save();
        Core.MessengerStatic.Messenger<string>.Broadcast(Constants.Events.OnLevelCreated, Progress.LastStartedLevelID);
    }

    public async void LoadLevelById(string id)
    {
        Progress.LastStartedLevelID = id;
       await GameField.LoadLevelByName(Progress.LastStartedLevelID);
        await Progress.Save();
        Core.MessengerStatic.Messenger<string>.Broadcast(Constants.Events.OnLevelCreated, Progress.LastStartedLevelID);
    }

    public float GetWinProgress()
    {
        return 20;
    }

    public async UniTask GetReward(RewardViewSetting settings)
    {
        Debug.LogError("--------giv reward "
                       + settings.RewardType
                       + " "
                       + settings.RewardCount
                       + " "
                       + settings.Percent
                       + " "
                       + settings.IsBig);

        CurrencyController.AddCurrency(settings.RewardType, settings.RewardCount);
        await UniTask.Delay(2000);
    }
    
    public void SetActiveLevelPanel(bool value)
    {
        LevelsWindiw.gameObject.SetActive(value);
    }

    private string GetNextLevelId()
    {
        var index = Progress.LevelDatas.FirstOrDefault(x => x.LevelStatus == Status.Open)
                    ?? Progress.LevelDatas.Last(x => x.LevelStatus == Status.Passed);

        return index.ID;
    }
}