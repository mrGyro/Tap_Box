using System;
using TMPro;
using UnityEngine;

namespace LevelCreator2
{
    public class FilePrefab : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        private Action<string> _onClick;

        public void Setup(string filename, Action<string> onClick)
        {
            var levelData = SaveLoadLevels.LoadLevelFromFile(filename);

            _text.text = $"id = {filename} size = {levelData.Data.Count}";
            _onClick = onClick;
        }

        public void OnClick()
        {
            _onClick?.Invoke(_text.text);
        }
    }
}