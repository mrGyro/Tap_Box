using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
        public List<LevelData> LevelDatas;

#if UNITY_EDITOR
        [MenuItem("Tools/GiroGame/RemoveSaves")]
        public static void RemoveFile()
        {
            File.Delete(Application.persistentDataPath + "/GameProgress.dat");
        }
#endif

        public async UniTask SaveGameProgress(GameProgress progress)
        {
            Debug.LogError("---------" + progress.LastStartedLevelID);

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/GameProgress.dat");

            bf.Serialize(file, progress);
            file.Close();
            await UniTask.Yield();
        }

        public async UniTask Save()
        {
            Debug.LogError("Save");

            await SaveGameProgress(this);
        }

        public async UniTask Load()
        {
            Debug.LogError("Load");
            var progress = await LoadGameProgress();
            LevelDatas = progress.LevelDatas;
            LastStartedLevelID = progress.LastStartedLevelID;
        }

        private async UniTask<GameProgress> LoadGameProgress()
        {
            var loadLevelsFromFile = await LoadLevelsName();
            GameProgress defaultLevels = new GameProgress
            {
                LevelDatas = new List<LevelData>()
            };

            foreach (var assetName in loadLevelsFromFile)
            {
                var levelData = await LoadLevelData(assetName);

                if (levelData == null)
                    continue;
                defaultLevels.LevelDatas.Add(levelData);
            }

            var gameProgress = LoadGameProgressFromFile();
            if (gameProgress.LevelDatas != null)
            {
                foreach (var levelData in gameProgress.LevelDatas)
                {
                    var level = defaultLevels.LevelDatas.Find(x => x.ID == levelData.ID);

                    if (level == null)
                        continue;

                    level.LevelStatus = levelData.LevelStatus;
                    level.BestResult = levelData.BestResult;
                }
            }
            else
            {
                gameProgress.LevelDatas = defaultLevels.LevelDatas;
            }

            defaultLevels.LastStartedLevelID = gameProgress.LastStartedLevelID;

            await UniTask.Yield();
            return defaultLevels;
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

        // private static async UniTask<GameProgress> LoadProgressData()
        // {
        //     var x = await AssetProvider.LoadAssetAsync<TextAsset>("GameProgress");
        //     MemoryStream memStream = new MemoryStream();
        //     BinaryFormatter binForm = new BinaryFormatter();
        //     memStream.Write(x.bytes, 0, x.bytes.Length);
        //     memStream.Seek(0, SeekOrigin.Begin);
        //
        //     try
        //     {
        //         return (GameProgress)binForm.Deserialize(memStream);
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.LogError("FistStart");
        //         return new GameProgress();
        //     }
        // }

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