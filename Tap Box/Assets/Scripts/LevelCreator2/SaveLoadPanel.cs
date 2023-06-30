using System.Collections.Generic;
using System.Linq;
using Boxes;
using LevelCreator;
using TMPro;
using UnityEngine;

namespace LevelCreator2
{
    public class SaveLoadPanel : MonoBehaviour
    {
        [SerializeField] private Transform filesRoot;
        [SerializeField] private TMP_InputField fileName;
        [SerializeField] private TMP_InputField id;
        [SerializeField] private TMP_InputField reward;
        [SerializeField] private TMP_Dropdown statusDropDown;
        [SerializeField] private TMP_Dropdown requirementDropDown;
        [SerializeField] private TMP_InputField requirementValue;
        [SerializeField] private FilePrefab filePrefab;
        [SerializeField] private LevelCreator levelCreator;
        [SerializeField] private Transform _camera;

        private string _currentSelected;
        private Status _status;
        private Reqirement.RequirementType _requirement;

        private void OnEnable()
        {
            LoadFiles();
        }

        public void Load()
        {
            if (_currentSelected == null)
            {
                return;
            }

            var levelData = SaveLoadLevels.LoadLevelFromFile(_currentSelected);

            if (levelData == null)
                return;

            levelCreator.RemoveAllBoxes();

            foreach (var variable in levelData.Data)
            {
                levelCreator.CreateBox(variable);
            }

            _camera.position = levelData.CameraPosition;
            _camera.transform.LookAt(Vector3.zero);
        }

        public void Save()
        {
            List<BoxData> data = new List<BoxData>();

            foreach (var variable in levelCreator.Level)
            {
                data.Add(variable.Data);
            }

            int.TryParse(reward.text, out var rewardCount);
            SaveLoadLevels.SaveLevelToFile(new LevelData()
            {
                Data = data,
                ID = id.text,
                LevelStatus = _status,
                Reward = rewardCount,
                CameraPosition = _camera.position,
                Reqirement = new Reqirement()
                {
                    Type = _requirement,
                    Value = requirementValue.text,
                }
            }, fileName.text);
        }
        
        public void Save(LevelData levelData, string fileName)
        {
            int.TryParse(reward.text, out var rewardCount);
            SaveLoadLevels.SaveLevelToFile(new LevelData()
            {
                Data = levelData.Data,
                ID = levelData.ID,
                LevelStatus = levelData.LevelStatus,
                Reward = levelData.Reward,
                CameraPosition = levelData.CameraPosition,
                Reqirement = new Reqirement()
                {
                    Type = levelData.Reqirement.Type,
                    Value = levelData.Reqirement.Value,
                }
            }, fileName);
        }

        public void StatusChanged()
        {
            switch (statusDropDown.value)
            {
                case 0:
                    _status = Status.None;
                    break;
                case 1:
                    _status = Status.Close;
                    break;
                case 2:
                    _status = Status.Open;
                    break;
                case 3:
                    _status = Status.Passed;
                    break;
            }
        }        
        
        public void RequirementChanged()
        {
            Debug.LogError(requirementDropDown.value);
            switch (requirementDropDown.value)
            {
                case 0:
                    _requirement = Reqirement.RequirementType.PassedLevel;
                    break;
      
            }
        }
        
        [ContextMenu("Load Levels Data From File")]
        public List<LevelData> LoadLevelsDataFromFile()
        {
            var levelsNames = SaveLoadLevels.LoadLevelsFromFile();
            List<LevelData> result = new List<LevelData>();
            foreach (var fileName in levelsNames)
            {
                var level = SaveLoadLevels.LoadLevelFromFile(fileName);
                if (level != null)
                {
                    result.Add(level);
                }
            }

            result.Sort((x, y) =>
            {
                
                if (x.Data.Count > y.Data.Count)
                    return 1;
                if (x.Data.Count < y.Data.Count)
                    return -1;

                return 0;
            });

            for (int i = 0; i < result.Count; i++)
            {
                Debug.LogError(result[i].Data.Count);
                Save(result[i], (i + 1).ToString());
            }


            return result;
        }

        private void LoadFiles()
        {
            foreach (Transform child in filesRoot)
            {
                Destroy(child.gameObject);
            }

            var list = SaveLoadLevels.LoadLevelsFromFile();
            if (list == null || list.Count == 0)
                return;
            
            list.Sort((x, y) =>
            {
                int ix, iy;
                return int.TryParse(x, out ix) && int.TryParse(y, out iy)
                    ? ix.CompareTo(iy) : string.Compare(x, y);
            });

            foreach (var button in list)
            {
                var fileButton = Instantiate(filePrefab, filesRoot);
                fileButton.Setup(button, SetSelectedFile);
            }
        }

        private void SetSelectedFile(string selectedFileName)
        {
            var levelData = SaveLoadLevels.LoadLevelFromFile(selectedFileName);
            if (levelData == null)
            {
                Debug.LogError("Level nit fount:" + selectedFileName);
                return;
            }
            
            _currentSelected = selectedFileName;
            fileName.text = selectedFileName;
            id.text = levelData.ID;
            reward.text = levelData.Reward.ToString();
            statusDropDown.value = (int)levelData.LevelStatus;
            requirementDropDown.value = (int)levelData.Reqirement.Type;
            requirementValue.text = levelData.Reqirement.Value;
        }
    }
}