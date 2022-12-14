using System;
using TMPro;
using UnityEngine;

namespace LevelCreator
{
    public class FilePrefab : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        private Action<string> _onClick;
        public void Setup(string filename, Action<string> onClick)
        {
            _text.text = filename;
            _onClick = onClick;
        }

        public void OnClick()
        {
            _onClick?.Invoke(_text.text);
        }
    }
}