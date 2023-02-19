using System;
using Currency;

namespace DefaultNamespace.UI.WinWindow
{
    [Serializable]
    public class RewardViewSetting
    {
        public string Sprite;
        public bool IsBig;
        public int Percent;
        public CurrencyController.Type RewardType;
        public int RewardCount;
    }
}