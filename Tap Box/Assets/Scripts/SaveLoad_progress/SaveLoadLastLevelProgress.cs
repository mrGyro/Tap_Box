using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LevelCreator;
using UnityEngine;

namespace SaveLoad_progress
{
    public static class SaveLoadLastLevelProgress
    {
        private static readonly string Path = Application.dataPath + "/Prefabs/LevelAssets/";

        public static void SaveLevel(LevelData levelForsave)
        {
            
        }

        public static LevelData LoadLevel()
        {
            return null;
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
                Debug.LogError(variable);
                var x = variable.Remove(0, variable.LastIndexOf('/') + 1);
                var index = x.LastIndexOf('.');
                var y = x.Remove(index, x.Length - index);
                result.Add(y);
            }

            return result;
        }
    }
}