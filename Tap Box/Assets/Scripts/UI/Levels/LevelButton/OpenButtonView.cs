using LevelCreator;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Levels.LevelButton
{
    public class OpenButtonView : MonoBehaviour
    {
        [SerializeField] private TMP_Text levelNumber;
        [SerializeField] private TMP_Text rewardTitle;
        [SerializeField] private Image rewardIcon;

        [SerializeField] private TMP_Text rewardNumber;
        [SerializeField] private Button actionButton;

        private LevelData _data;

        public async void Setup(LevelData data, int levelNum)
        {
            _data = data;
            levelNumber.text = (levelNum + 1).ToString();

            rewardTitle.text = "Reward";
            rewardIcon.sprite = await AssetProvider.LoadAssetAsync<Sprite>($"{Constants.Currency.Coins}_icon");
            rewardNumber.text = _data.Reward.ToString();

            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(OnButtonClick);
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        private void OnButtonClick()
        {
            GameManager.Instance.LoadLevelById(levelNumber.text);
            GameManager.Instance.UIManager.ClosePopUp(Constants.PopUps.LevelListPopUp);
        }
    }
}