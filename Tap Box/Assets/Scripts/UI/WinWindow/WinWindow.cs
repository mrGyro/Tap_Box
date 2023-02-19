using System.Collections.Generic;
using DefaultNamespace.UI.WinWindow;
using UnityEngine;
using UnityEngine.UI;

public class WinWindow : MonoBehaviour
{
   [SerializeField] private Slider progress;
   [SerializeField] private List<RewardView> rewardViews;

   public void SetActive(bool value)
   {
      gameObject.SetActive(value);

      if (!value) return;
      progress.value = Game.Instance.Progress.CurrentWinWindowsProgress;
      Setup();
   }

   private void Setup()
   {
      var settings = Game.Instance.CurrencyController.GetRewardSettings();
      for (int i = settings.Count; i < settings.Count; i++)
      {
         rewardViews[i].SetActive(false);
      }
      
      for (int i = 0; i < settings.Count; i++)
      {
         rewardViews[i].SetActive(true);
         rewardViews[i].Setup(settings[i]);
      }
   }
}
