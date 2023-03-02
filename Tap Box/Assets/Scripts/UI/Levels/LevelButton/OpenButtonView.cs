using LevelCreator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Levels.LevelButton
{
    public class OpenButtonView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image background;
        [SerializeField] private TMP_Text levelNumber;
        [SerializeField] private TMP_Text rewardTitle;
        [SerializeField] private Image rewardIcon;

        [SerializeField] private TMP_Text rewardNumber;
        [SerializeField] private Button actionButton;

        private LevelData _data;

        public async void Setup(LevelData data)
        {
            _data = data;
            background.color = Color.gray;
            levelNumber.text = _data.ID;
            icon.sprite = await AssetProvider.LoadAssetAsync<Sprite>("OpenIcon");

            rewardTitle.text = "Reward";
            rewardIcon.sprite = await AssetProvider.LoadAssetAsync<Sprite>("Coins");
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
            Managers.Instance.LoadLevelById(_data.ID);
            Managers.Instance.UIManager.ClosePopUp(Constants.PopUps.LevelListPopUp);
        }
    }
}