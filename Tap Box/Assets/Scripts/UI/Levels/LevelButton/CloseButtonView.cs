using System;
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
            GetRequiredString(data);
        }
        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        private void GetRequiredString(LevelData data)
        {
            var type = string.Empty;
            var value = string.Empty;

            switch (data.Reqirement.Type)
            {
                case Reqirement.RequirementType.PassedLevel:
                    type = "Complete \n";
                    break;
            }

            status.text = type;
            
            switch (data.Reqirement.Type)
            {
                case Reqirement.RequirementType.PassedLevel:
                    value = $"Level {data.Reqirement.Value}";
                    break;
            }

            requirements.text = value;
        }
    }
}