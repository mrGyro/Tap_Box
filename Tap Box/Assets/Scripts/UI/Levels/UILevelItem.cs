using LevelCreator;
using UI.Levels.LevelButton;
using UnityEngine;

namespace UI.Levels
{
    public class UILevelItem : MonoBehaviour
    {
        public LevelData Data;

        [SerializeField] private CloseButtonView closeButtonView;
        [SerializeField] private OpenButtonView openButtonView;
        [SerializeField] private PassedButtonView passedButton;

        public void Setup(LevelData data)
        {
            switch (data.LevelStatus)
            {
                case Status.None:
                    break;
                case Status.Open:
                    passedButton.SetActive(false);
                    closeButtonView.SetActive(false);
                    openButtonView.SetActive(true);
                    openButtonView.Setup(data);
                    break;
                case Status.Passed:
                    openButtonView.SetActive(false);
                    closeButtonView.SetActive(false);
                    passedButton.SetActive(true);
                    passedButton.Setup(data);
                    break;
                case Status.Close:
                    passedButton.SetActive(false);
                    openButtonView.SetActive(false);
                    closeButtonView.SetActive(true);
                    closeButtonView.Setup(data);
                    break;
            }

            Data = data;
        }

        public void UpdateButton(LevelData data)
        {
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }


    }
}