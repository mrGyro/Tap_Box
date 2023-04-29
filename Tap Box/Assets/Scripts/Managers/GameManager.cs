using System.Collections.Generic;
using System.Linq;
using Ads;
using Currency;
using LevelCreator;
using PlayerLevel;
using SaveLoad_progress;
using Unity.VisualScripting;
using UnityEngine;
using VFX;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public GameField GameField;
        public InputController InputController;
        public GameProgress Progress;
        public CurrencyController CurrencyController;
        public UIManager UIManager;
        public PlayerLevelManager PlayerLevelManager;
        public Mediation Mediation;
        public SkinsManager SkinsManager;
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

            await Progress.Load();
            SkinsManager.Initialize();
            UIManager.Initialize();

            GameField.Initialize();
            PlayerLevelManager.Initialize();

            Progress.LastStartedLevelID =
                string.IsNullOrEmpty(Progress.LastStartedLevelID)
                    ? GetNextLevelId()
                    : Progress.LastStartedLevelID;
            

            LoadLevelById(Progress.LastStartedLevelID);
            Mediation.Initialize();
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
            Progress.LastStartedLevelID = id;

            await GameField.LoadLevelByName(Progress.LastStartedLevelID);

            await Progress.Save();
            Core.MessengerStatic.Messenger<string>.Broadcast(Constants.Events.OnLevelCreated, Progress.LastStartedLevelID);
        }

        public float GetWinProgress()
        {
            return 25;
            return GameField.GetCurrentLevelID() == "1" ? 8 : Random.Range(10, 20);
        }

        public void SetActiveGlobalInput(bool value)
        {
            InputController.SetActiveInput(value);
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