using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Currency;
using Cysharp.Threading.Tasks;
using LevelCreator;
using UnityEngine;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
using UnityEditor;

#endif

namespace SaveLoad_progress
{
    [Serializable]
    public class GameProgress
    {
        public string LastStartedLevelID;
        public float CurrentWinWindowsProgress;
        public int NextRewardIndexWinWindow;
        public int CurrentPlayerLevel;
        public float CurrentPlayerLevelProgress;
        public string CurrentSkin;


        public Dictionary<CurrencyController.Type, int> Currencies;

        public List<LevelData> LevelDatas;

#if UNITY_EDITOR
        [MenuItem("Tools/GiroGame/RemoveSaves")]
        public static void RemoveFile()
        {
            File.Delete(Application.persistentDataPath + "/GameProgress.dat");
        }
#endif
        public void SetValues(GameProgress progress)
        {
            LevelDatas = progress.LevelDatas;
            LastStartedLevelID = progress.LastStartedLevelID;
            CurrentWinWindowsProgress = progress.CurrentWinWindowsProgress;
            NextRewardIndexWinWindow = progress.NextRewardIndexWinWindow;
            Currencies = progress.Currencies;
            CurrentPlayerLevel = progress.CurrentPlayerLevel;
            CurrentPlayerLevelProgress = progress.CurrentPlayerLevelProgress;
            CurrentSkin = string.IsNullOrEmpty(CurrentSkin) ? "Default" : progress.CurrentSkin;
        }

        public async UniTask SaveGameProgress(GameProgress progress)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/GameProgress.dat");

            bf.Serialize(file, progress);
            file.Close();
            await UniTask.Yield();
        }

        public async UniTask Save()
        {
            await SaveGameProgress(this);
        }

        public async UniTask Load()
        {
            await LoadGameProgress();
        }

        public void CheckRequirement()
        {
            foreach (var levelData in LevelDatas)
            {
                if (levelData.Reqirement.CheckForDone() && levelData.LevelStatus == Status.Close)
                    levelData.LevelStatus = Status.Open;
            }
        }

        private async UniTask LoadGameProgress()
        {
            var loadGameProgress = new GameProgress
            {
                LevelDatas = new List<LevelData>()
            };
            
            var loadLevelsFromFile = await LoadLevelsName();
            foreach (var assetName in loadLevelsFromFile)
            {
                var levelData = await LoadLevelData(assetName);

                if (levelData == null)
                    continue;
                
                loadGameProgress.LevelDatas.Add(levelData);
            }

            var gameProgress = LoadGameProgressFromFile();
            if (gameProgress.LevelDatas != null)
            {
                foreach (var levelData in gameProgress.LevelDatas)
                {
                    var level = loadGameProgress.LevelDatas.Find(x => x.ID == levelData.ID);

                    if (level == null)
                        continue;

                    level.LevelStatus = levelData.LevelStatus;
                    level.BestResult = levelData.BestResult;
                }
            }
            else
            {
                gameProgress.LevelDatas = loadGameProgress.LevelDatas;
            }
            
            SetValues(gameProgress);
        }

        public async UniTask<LevelData> LoadLevelData(string assetName)
        {
            var x = await AssetProvider.LoadAssetAsync<TextAsset>(assetName);
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(x.bytes, 0, x.bytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            return (LevelData)binForm.Deserialize(memStream);
        }

        public async UniTask ChangeBlock(string name)
        {
            if (CurrentSkin == name)
                return;
            CurrentSkin = name;
            await Managers.Instance.GameField.ChangeSkin();
        }

        private async UniTask<List<string>> LoadLevelsName()
        {
            List<string> result = new List<string>();
            await Addressables.LoadAssetsAsync<TextAsset>("Levels", asset =>
            {
                result.Add(asset.name);
            });
            return result;
        }

        private GameProgress LoadGameProgressFromFile()
        {
            BinaryFormatter bf = new BinaryFormatter();

            string path = Application.persistentDataPath + "/GameProgress.dat";
            Debug.LogError(path);
            if (File.Exists(path))
            {
                FileStream file = File.Open(path, FileMode.Open);
                try
                {
                    GameProgress progress = (GameProgress)bf.Deserialize(file);
                    file.Close();

                    if (progress.LevelDatas == null)
                    {
                        LevelDatas = new List<LevelData>();
                    }

                    return progress;
                }
                catch (Exception)
                {
                    file.Close();
                    return new GameProgress()
                    {
                        LevelDatas = new List<LevelData>()
                    };
                }
            }

            return new GameProgress()
            {
                LevelDatas = new List<LevelData>()
            };
        }
    }
}