using System;
using System.Collections.Generic;
using DefaultNamespace.UI.WinWindow;

namespace Currency
{
    public class CurrencyController
    {
        public enum Type
        {
            Coin,
            BoxSkin,
            RandomSkin,
            BackgroundSkin,
            FlowSkin,
            VFXSkin,
        }

        public Action<string, int> OnCurrencyCountChanged;

        public void AddCurrency(Type type, int value)
        {
            var currencyType = type.ToString();
            
            if (Game.Instance.Progress.Currencies.ContainsKey(currencyType))
                Game.Instance.Progress.Currencies[currencyType] += value;
            else
                Game.Instance.Progress.Currencies.Add(currencyType, value);
            
            OnCurrencyCountChanged?.Invoke(currencyType, Game.Instance.Progress.Currencies[currencyType]);
        }
        
        public void RemoveCurrency(Type type, int value)
        {
            var currencyType = type.ToString();

            if (Game.Instance.Progress.Currencies.ContainsKey(currencyType))
            {
                Game.Instance.Progress.Currencies[currencyType] -= value;
                if (Game.Instance.Progress.Currencies[currencyType] < 0)
                {
                    Game.Instance.Progress.Currencies[currencyType] = 0;
                }
            }
            else
                Game.Instance.Progress.Currencies.Add(currencyType, 0);
            
            OnCurrencyCountChanged?.Invoke(currencyType, Game.Instance.Progress.Currencies[currencyType]);
        }

        public List<RewardViewSetting> GetRewardSettings()
        {
            return new List<RewardViewSetting>();
        }
    }
}