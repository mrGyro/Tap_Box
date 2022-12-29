﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DefaultNamespace;
using UnityEngine;

namespace LevelCreator
{
    public class SaveLoadLevels
    {
        private static string _path = Application.dataPath + "/Prefabs/LevelAssets/";
        public bool LevelFileExist(string fileName)
        {
            return File.Exists(_path + fileName + ".dat");
        }

        public static void SaveLevelToFile(LevelData wayData, string fileName)
        {
            
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(_path + fileName + ".txt");

            bf.Serialize(file, wayData);
            file.Close();
            Debug.LogError("saved");
        }

        public static LevelData LoadLevelFromFile(string fileName)
        {
            BinaryFormatter bf = new BinaryFormatter();

            string path = _path + fileName + ".txt";
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
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            var files = Directory.GetFiles(_path);
            List<string> result = new List<string>();
            foreach (var variable in files)
            {
                if(variable.Contains(".meta"))
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