using System;
using Boxes;
using Managers;
using Unity.VisualScripting;

namespace PlayerLevel
{
    public class PlayerLevelManager : IInitializable
    {
        public Action<int> OnLevelChanged;
        public Action<float> OnLevelProgressChanged;
        
        public void Initialize()
        {
            Core.MessengerStatic.Messenger<BaseBox>.AddListener(Constants.Events.OnBoxRemoveFromGameField, Handler);
        }
        
        private void Handler(BaseBox obj)
        {
            AddExperience(1);
        }

        public void AddExperience(float value)
        {
            GameManager.Instance.Progress.CurrentPlayerLevelProgress += value;
            if (GameManager.Instance.Progress.CurrentPlayerLevelProgress >= 100)
            {
                AddLevel();
                return;
            }

            OnLevelProgressChanged?.Invoke(GameManager.Instance.Progress.CurrentPlayerLevelProgress);
        }

        private void AddLevel()
        {
            GameManager.Instance.Progress.CurrentPlayerLevel++;
            GameManager.Instance.Progress.CurrentPlayerLevelProgress = 0;
            OnLevelChanged?.Invoke(GameManager.Instance.Progress.CurrentPlayerLevel);
            OnLevelProgressChanged?.Invoke(GameManager.Instance.Progress.CurrentPlayerLevelProgress);
            GameManager.Instance.UIManager.ShowPopUp(Constants.PopUps.NewLevelPopUp);
        }
    }
}