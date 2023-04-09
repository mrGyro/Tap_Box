using LevelCreator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Levels.LevelButton
{
    public class PassedButtonView : MonoBehaviour
    {
        [SerializeField] private TMP_Text levelNumber;
        [SerializeField] private TMP_Text bestScoreCount;
        [SerializeField] private Button actionButton;
        
        private LevelData _data;
        public async void Setup(LevelData data)
        {
            _data = data;
            levelNumber.text = _data.ID;
            bestScoreCount.text = _data.BestResult.ToString();
            
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(OnButtonClick);
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }
        
        private void OnButtonClick()
        {
            Managers.Instance.LoadLevelById(_data.ID);
            Managers.Instance.UIManager.ClosePopUp(Constants.PopUps.LevelListPopUp);
        }
    }
}