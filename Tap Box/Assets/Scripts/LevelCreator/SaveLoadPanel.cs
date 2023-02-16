using System.Collections.Generic;
using Boxes;
using TMPro;
using UnityEngine;

namespace LevelCreator
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
        }

        public void Save()
        {
            List<BoxData> data = new List<BoxData>();

            foreach (var variable in levelCreator.Level)
            {
                data.Add(variable.Data);
                Debug.LogError(variable.Data.ArrayPosition.ToVector3() + " " + variable.Data.Rotation.ToVector3());
            }

            int.TryParse(reward.text, out var rewardCount);
            SaveLoadLevels.SaveLevelToFile(new LevelData()
            {
                Data = data,
                ID = id.text,
                LevelStatus = _status,
                Reward = rewardCount,
                Reqirement = new Reqirement()
                {
                    Type = _requirement,
                    Value = requirementValue.text,
                }
            }, fileName.text);
        }

        public void StatusChanged()
        {
            Debug.LogError(statusDropDown.value);
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

        private void LoadFiles()
        {
            foreach (Transform child in filesRoot)
            {
                Destroy(child.gameObject);
            }

            var list = SaveLoadLevels.LoadLevelsFromFile();
            if (list == null || list.Count == 0)
                return;

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