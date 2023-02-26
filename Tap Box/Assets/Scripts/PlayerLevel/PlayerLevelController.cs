using System;
using Unity.VisualScripting;

namespace PlayerLevel
{
    public class PlayerLevelController : IInitializable
    {
        public Action<int> OnGetNevLevel;
        public Action<float> OnPlayerLevelProgressChanged;
        
        public void Initialize()
        {
        }

        public void AddExperience(float value)
        {
            Game.Instance.Progress.CurrentPlayerLevelProgress += value;

            if (Game.Instance.Progress.CurrentPlayerLevelProgress >= 100)
            {
                AddLevel();
                return;
            }

            OnPlayerLevelProgressChanged?.Invoke(Game.Instance.Progress.CurrentPlayerLevelProgress);
        }

        private void AddLevel()
        {
            Game.Instance.Progress.CurrentPlayerLevel++;
            Game.Instance.Progress.CurrentPlayerLevelProgress = 0;
            OnGetNevLevel?.Invoke(Game.Instance.Progress.CurrentPlayerLevel);
            OnPlayerLevelProgressChanged?.Invoke(Game.Instance.Progress.CurrentPlayerLevelProgress);
        }
    }
}