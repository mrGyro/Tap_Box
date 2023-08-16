using Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace UI
{
    public class TurnsLeftCounter : MonoBehaviour, IInitializable
    {
        [SerializeField] private TMP_Text counter;

        private int _countToSave = 20;
        private int _currentCountToSave = 0;
        public void Initialize()
        {
            Core.MessengerStatic.Messenger.AddListener(Constants.Events.OnBoxClicked, OnBoxRemoved);
            Core.MessengerStatic.Messenger<string>.AddListener(Constants.Events.OnLevelCreated, OnLevelCreated);
            Core.MessengerStatic.Messenger.AddListener(Constants.Events.OnGameLoose, OnLoose);
        }

        private void OnLoose()
        {
            GameManager.Instance.UIManager.ShowPopUp(Constants.PopUps.LosePopUp);
        }

        private void OnLevelCreated(string obj)
        {
            _currentCountToSave = 0;
            SetTurnsText();
        }

        private void OnBoxRemoved()
        {
            SetTurnsText();

        }

        public void SetTurnsText()
        {
            GameManager.Instance.Progress.CurrentLevelTurnsLeftValue = GameManager.Instance.GameField.GetTurnsCount;
            counter.text = $"{GameManager.Instance.GameField.GetTurnsCount} turns";
            
            _currentCountToSave++;
            
            if (_currentCountToSave >= _countToSave && GameManager.Instance.GameField.GetTurnsCount > 10)
            {
                Debug.LogError("save");
                _currentCountToSave = 0;
                GameManager.Instance.Progress.Save();
            }
        }

        private void OnDestroy()
        {
            Core.MessengerStatic.Messenger.RemoveListener(Constants.Events.OnBoxClicked, OnBoxRemoved);
            Core.MessengerStatic.Messenger<string>.RemoveListener(Constants.Events.OnLevelCreated, OnLevelCreated);
            Core.MessengerStatic.Messenger.RemoveListener(Constants.Events.OnGameLoose, OnLoose);
        }
    }
}