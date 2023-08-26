using LevelCreator;
using TMPro;
using UnityEngine;

namespace UI.Levels.LevelButton
{
    public class CloseButtonView : MonoBehaviour
    {
        [SerializeField] private TMP_Text levelNumber;
        [SerializeField] private TMP_Text requirements;

        public void Setup(LevelData data)
        {
            levelNumber.text = data.ID;
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

            levelNumber.text = data.ID;
            switch (data.Reqirement.Type)
            {
                case Reqirement.RequirementType.PassedLevel:
                    type = "Complete \n";
                    break;
            }

            switch (data.Reqirement.Type)
            {
                case Reqirement.RequirementType.PassedLevel:
                    value = $"{type}Level {data.Reqirement.Value}";
                    break;
            }

            requirements.text = value;
        }
    }
}