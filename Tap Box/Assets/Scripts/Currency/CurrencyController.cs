using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.UI.WinWindow;
using Managers;
using UI.Skins;

namespace Currency
{
    public class CurrencyController
    {
        [Serializable]
        public enum Type
        {
            Coin,
            BoxSkin,
            RandomSkin,
            BackgroundSkin,
            TailSkin,
            TapSkin,
            RewardedAds,
            InterstitialAds,
            Level
        }

        public Action<Type, int> OnCurrencyCountChanged;

        public void AddCurrency(Type type, int value)
        {
            GameManager.Instance.Progress.Currencies ??= new Dictionary<Type, int>();

            if (GameManager.Instance.Progress.Currencies.ContainsKey(type))
            {
                GameManager.Instance.Progress.Currencies[type] += value;
            }
            else
            {
                GameManager.Instance.Progress.Currencies.Add(type, value);
            }

            OnCurrencyCountChanged?.Invoke(type, GameManager.Instance.Progress.Currencies[type]);
        }

        public void AddSkin(Type wayToGet,Type type, string value)
        {
            var skinData = GameManager.Instance.Progress.SkinDatas.FirstOrDefault(x => x.SkinAddressableName == value);
           
            if (skinData != null)
            {
                skinData.IsOpen = true;
                return;
            }
            
            GameManager.Instance.Progress.SkinDatas.Add(new SkinData()
            {
                IsOpen = true,
                Price = 0,
                SkinAddressableName = value,
                WayToGet = wayToGet,
                Type = type,
            });
        } 

        public void RemoveCurrency(Type type, int value)
        {
            if (GameManager.Instance.Progress.Currencies == null)
                return;

            if (GameManager.Instance.Progress.Currencies.ContainsKey(type))
            {
                GameManager.Instance.Progress.Currencies[type] -= value;
                if (GameManager.Instance.Progress.Currencies[type] < 0)
                {
                    GameManager.Instance.Progress.Currencies[type] = 0;
                }
            }
            else
                GameManager.Instance.Progress.Currencies.Add(type, 0);

            OnCurrencyCountChanged?.Invoke(type, GameManager.Instance.Progress.Currencies[type]);
        }

        public int GetCurrency(Type type)
        {
            if (GameManager.Instance.Progress.Currencies == null)
                return 0;
            
            return GameManager.Instance.Progress.Currencies.ContainsKey(type) ? GameManager.Instance.Progress.Currencies[type] : 0;
        }

        public List<RewardViewSetting> GetRewardSettings()
        {
            return new List<RewardViewSetting>()
            {
                new() { IsBig = false, RewardCount = 30, Percent = 10, RewardType = Type.Coin },
                new() { IsBig = false, RewardCount = 40, Percent = 30, RewardType = Type.Coin },
                new() { IsBig = false, RewardCount = 50, Percent = 50, RewardType = Type.Coin },
                new() { IsBig = false, RewardCount = 60, Percent = 70, RewardType = Type.Coin },
                new() { IsBig = false, RewardCount = 1, Percent = 90, RewardType = Type.RandomSkin }
            };
        }
    }
}