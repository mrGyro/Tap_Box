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

        public Action<Type, int> OnCurrencyCountChanged;

        public void AddCurrency(Type type, int value)
        {
            if (Managers.Instance.Progress.Currencies == null)
                return;

            if (Managers.Instance.Progress.Currencies.ContainsKey(type))
                Managers.Instance.Progress.Currencies[type] += value;
            else
                Managers.Instance.Progress.Currencies.Add(type, value);

            OnCurrencyCountChanged?.Invoke(type, Managers.Instance.Progress.Currencies[type]);
        }

        public void RemoveCurrency(Type type, int value)
        {
            if (Managers.Instance.Progress.Currencies == null)
                return;

            if (Managers.Instance.Progress.Currencies.ContainsKey(type))
            {
                Managers.Instance.Progress.Currencies[type] -= value;
                if (Managers.Instance.Progress.Currencies[type] < 0)
                {
                    Managers.Instance.Progress.Currencies[type] = 0;
                }
            }
            else
                Managers.Instance.Progress.Currencies.Add(type, 0);

            OnCurrencyCountChanged?.Invoke(type, Managers.Instance.Progress.Currencies[type]);
        }

        public int GetCurrency(Type type)
        {
            if (Managers.Instance.Progress.Currencies == null)
                return 0;
            
            return Managers.Instance.Progress.Currencies.ContainsKey(type) ? Managers.Instance.Progress.Currencies[type] : 0;
        }

        public bool IsInitialized() => Managers.Instance.Progress.Currencies != null;

        public List<RewardViewSetting> GetRewardSettings()
        {
            return new List<RewardViewSetting>()
            {
                new() { IsBig = false, RewardCount = 10, Percent = 10, RewardType = Type.Coin },
                new() { IsBig = false, RewardCount = 20, Percent = 30, RewardType = Type.Coin },
                new() { IsBig = false, RewardCount = 30, Percent = 50, RewardType = Type.Coin },
                new() { IsBig = false, RewardCount = 40, Percent = 70, RewardType = Type.Coin },
                new() { IsBig = false, RewardCount = 1, Percent = 90, RewardType = Type.RandomSkin }
            };
        }
    }
}