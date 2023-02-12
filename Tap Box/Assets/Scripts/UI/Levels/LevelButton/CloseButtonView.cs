using LevelCreator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Levels.LevelButton
{
    public class CloseButtonView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image background;
        [SerializeField] private TMP_Text status;
        [SerializeField] private TMP_Text requirements;

        public async void Setup(LevelData data)
        {
            background.color = Color.gray;

            icon.sprite = await AssetProvider.LoadAssetAsync<Sprite>("CloseIcon");
            status.text = "For open";
            requirements.text = "Pass level 1";
        }
        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}