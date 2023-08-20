using System.Collections.Generic;
using System.Linq;
using Ads;
using Analytic;
using Currency;
using IAP;
using Lean.Touch;
using LevelCreator;
using PlayerLevel;
using SaveLoad_progress;
using Sounds;
using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Serialization;
using VFX;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public string environment = "production";

        public GameField GameField;
        public InputController InputController;
        public GameProgress Progress;
        public CurrencyController CurrencyController;
        public UIManager UIManager;
        public PlayerLevelManager PlayerLevelManager;
        public Mediation Mediation;
        public SkinsManager SkinsManager;
        public SoundController SoundManager;
        public AnalyticManager AnalyticManager;
        public TutorialManager TutorialManager;
        public IAPManager IAPManager;
        [FormerlySerializedAs("CheetManager")] public CheatManager _cheatManager;
        [SerializeField] private TapEffectController _tapEffectController;

        private async void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Progress = new GameProgress();
                CurrencyController = new CurrencyController();
                PlayerLevelManager = new PlayerLevelManager();
                SkinsManager = new SkinsManager();
            }

            TutorialManager.Initialize();
            AnalyticManager.Initialize();
            await Progress.Load();

            GameField.Initialize();
            PlayerLevelManager.Initialize();
            
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                var options = new InitializationOptions()
                    .SetEnvironmentName(environment);

                await UnityServices.InitializeAsync(options);
                AnalyticsService.Instance.StartDataCollection();
            }
            
            
            await IAPManager.AwaitInitialization(new List<IapProduct>()
            {
                new IapProduct()
                {
                    Id = Constants.IAP.NoAds,
                    ProductType = ProductType.NonConsumable
                }
            });
            _cheatManager.Initialize();

            Mediation.Initialize();
            UIManager.Initialize();
            SkinsManager.Initialize();

            Progress.LastStartedLevelID =
                string.IsNullOrEmpty(Progress.LastStartedLevelID)
                    ? GetNextLevelId()
                    : Progress.LastStartedLevelID;


            LoadLevelById(Progress.LastStartedLevelID);
            _tapEffectController.Initialize();
        }

        public async void SaveLevel(LevelData level)
        {
            UpdateLevel(level);
            Progress.CheckRequirement();
            await Progress.Save();
        }

        public void LoadNextLevel()
        {
            LoadLevelById(GetNextLevelId());
        }

        public async void LoadLevelById(string id)
        {
            if (PlayerPrefs.HasKey("OpenLevel_" + id))
            {
                AnalyticManager.SendEvent(Constants.AnalyticsEvent.OpenLevel, Constants.AnalyticsEvent.LevelIdParameter, id);
            }
            else
            {
                Debug.LogError("first " + id);

                PlayerPrefs.SetInt("OpenLevel_" + id, 1);
                AnalyticManager.SendEvent(Constants.AnalyticsEvent.FirstOpenLevel, Constants.AnalyticsEvent.LevelIdParameter, id);
            }
            
            LeanTouch.Fingers.Clear();
            Instance.InputController.SetDefaultZoom();
            Progress.LastStartedLevelID = id;

            await GameField.LoadLevelByName(Progress.LastStartedLevelID);
            
            Core.MessengerStatic.Messenger<string>.Broadcast(Constants.Events.OnLevelCreated, Progress.LastStartedLevelID);
            await Progress.Save();
        }

        public float GetWinProgress()
        {
            //return 25;
            return GameField.GetCurrentLevelID() == "1" ? 8 : Random.Range(10, 20);
        }

        public void SetActiveGlobalInput(bool value)
        {
            InputController.SetActiveAllInput(value);
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
}