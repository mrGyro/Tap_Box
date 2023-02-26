using System;
using Boxes;
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
            AddExperience(10);
        }

        public void AddExperience(float value)
        {
            Managers.Instance.Progress.CurrentPlayerLevelProgress += value;

            if (Managers.Instance.Progress.CurrentPlayerLevelProgress >= 100)
            {
                AddLevel();
                return;
            }

            OnLevelProgressChanged?.Invoke(Managers.Instance.Progress.CurrentPlayerLevelProgress);
        }

        private void AddLevel()
        {
            Managers.Instance.Progress.CurrentPlayerLevel++;
            Managers.Instance.Progress.CurrentPlayerLevelProgress = 0;
            OnLevelChanged?.Invoke(Managers.Instance.Progress.CurrentPlayerLevel);
            OnLevelProgressChanged?.Invoke(Managers.Instance.Progress.CurrentPlayerLevelProgress);
        }
    }
}