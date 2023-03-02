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
    public InputController InputController;
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

        GameField.Initialize();
        PlayerLevelManager.Initialize();
        UIManager.Initialize();

        Progress.LastStartedLevelID = 
            string.IsNullOrEmpty(Progress.LastStartedLevelID) 
            ? GetNextLevelId() 
            : Progress.LastStartedLevelID;
        
        LoadLevelById(Progress.LastStartedLevelID);
    }

    public async void SaveLevel(LevelData level)
    {
        Progress.CheckRequirement();
        UpdateLevel(level);
        await Progress.Save();
    }

    public void LoadNextLevel()
    {
         LoadLevelById(GetNextLevelId());
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
        CurrencyController.AddCurrency(settings.RewardType, settings.RewardCount);
        await UniTask.Delay(2000);
    }
    
    private void UpdateLevel(LevelData level)
    {
        var updateLevelButton = Instance.Progress.LevelDatas.FindIndex(x => x.ID == level.ID);

        if (updateLevelButton == -1)
            return;

        Progress.LevelDatas[updateLevelButton].LevelStatus = level.LevelStatus;
        Progress.LevelDatas[updateLevelButton].BestResult = level.BestResult;
    }

    private string GetNextLevelId()
    {

        var index = Progress.LevelDatas.FirstOrDefault(x => x.LevelStatus == Status.Open)
                    ?? Progress.LevelDatas.Last(x => x.LevelStatus == Status.Passed);

        return index.ID;
    }
}