using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Cysharp.Threading.Tasks;
using LevelCreator;
using UnityEngine;

namespace SaveLoad_progress
{
    public class SaveLoadGameProgress
    {
        private static readonly string Path = Application.dataPath + "/Prefabs/LevelAssets/";
        private static readonly string PathProgress = Application.dataPath + "/Prefabs/GameProgress.txt";

        public static void SaveGameProgress(GameProgress progress)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(PathProgress);

            bf.Serialize(file, progress);
            file.Close();
            Debug.LogError("saved");
        }

        public static async UniTask<GameProgress> LoadGameProgress()
        {
            var loadLevelsFromFile = LoadLevelsName();
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
                Debug.LogError(assetName);
            }

            var gameProgress = await LoadProgressData();

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
            
           
            return defaultLevels;
        }

        public static async UniTask<LevelData> LoadLevelData(string assetName)
        {
            var x = await AssetProvider.LoadAssetAsync<TextAsset>(assetName);
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(x.bytes, 0, x.bytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            return (LevelData)binForm.Deserialize(memStream);
        }

        private static async UniTask<GameProgress> LoadProgressData()
        {
            var x = await AssetProvider.LoadAssetAsync<TextAsset>("GameProgress");
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(x.bytes, 0, x.bytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            return (GameProgress)binForm.Deserialize(memStream);
        }

        private static List<string> LoadLevelsName()
        {
            if (!Directory.Exists(Path))
                Directory.CreateDirectory(Path);

            var files = Directory.GetFiles(Path);
            List<string> result = new List<string>();
            foreach (var variable in files)
            {
                if (variable.Contains(".meta"))
                    continue;

                var x = variable.Remove(0, variable.LastIndexOf('/') + 1);
                var index = x.LastIndexOf('.');
                var y = x.Remove(index, x.Length - index);
                result.Add(y);
            }

            return result;
        }

        private static GameProgress LoadGameProgressFromFile()
        {
            BinaryFormatter bf = new BinaryFormatter();

            string path = PathProgress;
            if (File.Exists(path))
            {
                FileStream file = File.Open(path, FileMode.Open);
                try
                {
                    GameProgress wayData = (GameProgress)bf.Deserialize(file);
                    file.Close();

                    return wayData;
                }
                catch (Exception)
                {
                    file.Close();
                    return null;
                }
            }

            return null;
        }
    }
}