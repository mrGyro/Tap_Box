using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.UI.WinWindow;
using UI.Skins;

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
            RewardedAds,
            InterstitialAds
        }

        public Action<Type, int> OnCurrencyCountChanged;

        public void AddCurrency(Type type, int value)
        {
            Managers.Instance.Progress.Currencies ??= new Dictionary<Type, int>();
            
            if (Managers.Instance.Progress.Currencies.ContainsKey(type))
                Managers.Instance.Progress.Currencies[type] += value;
            else
                Managers.Instance.Progress.Currencies.Add(type, value);

            OnCurrencyCountChanged?.Invoke(type, Managers.Instance.Progress.Currencies[type]);
        }

        public void AddSkin(Type type, string value)
        {
            var skinData = Managers.Instance.Progress.SkinDatas.FirstOrDefault(x => x.SkinAddressableName == value);
           
            if (skinData != null)
            {
                skinData.IsOpen = true;
                return;
            }
            
            Managers.Instance.Progress.SkinDatas.Add(new SkinData()
            {
                IsOpen = true,
                Price = 0,
                SkinAddressableName = value,
                Type = type,
                IsRandom = false
            });
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