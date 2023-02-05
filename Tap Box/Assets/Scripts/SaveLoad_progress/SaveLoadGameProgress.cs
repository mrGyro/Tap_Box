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

        public static void SaveGameProgress(GameProgress progress)
        {
        }

        public static async UniTask<GameProgress> LoadGameProgress()
        {
            var loadLevelsFromFile = LoadLevelsFromFile();
            GameProgress progress = new GameProgress
            {
                LevelDatas = new List<LevelData>()
            };

            foreach (var assetName in loadLevelsFromFile)
            {
                var levelData = await LoadLevelData(assetName);

                if (levelData == null) 
                    continue;
                progress.LevelDatas.Add(levelData);
                Debug.LogError(assetName);
            }

            return progress;
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

        public static async UniTask<LevelData> LoadLevelDataText(TextAsset assetText)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(assetText.bytes, 0, assetText.bytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            return (LevelData)binForm.Deserialize(memStream);
        }

        public static void SaveLevelToFile(LevelData wayData, string fileName)
        {
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Path + fileName + ".txt");

            bf.Serialize(file, wayData);
            file.Close();
            Debug.LogError("saved");
        }

        public static LevelData LoadLevelFromFile(string fileName)
        {
            BinaryFormatter bf = new BinaryFormatter();

            string path = Path + fileName + ".txt";
            if (File.Exists(path))
            {
                FileStream file = File.Open(path, FileMode.Open);
                try
                {
                    LevelData wayData = (LevelData)bf.Deserialize(file);
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

        public static List<string> LoadLevelsFromFile()
        {
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }

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
    }
}