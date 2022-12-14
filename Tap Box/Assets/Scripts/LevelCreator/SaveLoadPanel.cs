using System.Collections.Generic;
using Boxes;
using DefaultNamespace;
using TMPro;
using UnityEngine;

namespace LevelCreator
{
    public class SaveLoadPanel : MonoBehaviour
    {
        [SerializeField] private Transform _filesRoot;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private FilePrefab _filePrefab;
        [SerializeField] private LevelCreator _levelCreator;

        private string _currentSelected;

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

            _levelCreator.RemoveAllBoxes();

            foreach (var VARIABLE in levelData.Data)
            {
                _levelCreator.CreateBox(VARIABLE);
            }
        }

        public void Save()
        {
            List<BoxData> data = new List<BoxData>();

            foreach (var VARIABLE in _levelCreator.Level)
            {
                data.Add(VARIABLE.Data);
                Debug.LogError(VARIABLE.Data.ArrayPosition.ToVector3() + " " + VARIABLE.Data.Rotation.ToVector3());
            }

            SaveLoadLevels.SaveLevelToFile(new LevelData() { Data = data }, _inputField.text);
        }

        private void LoadFiles()
        {
            foreach (Transform child in _filesRoot)
            {
                Destroy(child.gameObject);
            }

            var list = SaveLoadLevels.LoadLevelsFromFile();
            if (list == null || list.Count == 0)
                return;

            foreach (var button in list)
            {
                var fileButton = Instantiate(_filePrefab, _filesRoot);
                fileButton.Setup(button, SetSelectedFile);
            }
        }

        private void SetSelectedFile(string fileName)
        {
            _currentSelected = fileName;
            _inputField.text = fileName;
        }
    }
}