using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using DefaultNamespace;
using UnityEngine;

namespace LevelCreator
{
    public class SaveLoadLevels
    {
        public bool LevelFileExist(string fileName)
        {
            return File.Exists(Application.persistentDataPath + "/Levels/" + fileName + ".dat");
        }

        public static void SaveLevelToFile(LevelData wayData, string fileName)
        {
            string path = Application.persistentDataPath + "/Levels/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/Levels/" + fileName + ".dat");

            bf.Serialize(file, wayData);
            file.Close();
            Debug.LogError("saved");
        }

        public static LevelData LoadLevelFromFile(string fileName)
        {
            BinaryFormatter bf = new BinaryFormatter();

            string path = Application.persistentDataPath + "/Levels/" + fileName + ".dat";
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
            var path = Application.persistentDataPath + "/Levels/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var files = Directory.GetFiles(path);
            List<string> result = new List<string>();
            foreach (var variable in files)
            {
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