using System;

namespace UI.Skins
{
    [Serializable]
    public class SkinData
    {
        public enum GetType
        {
            None,
            Coins,
            RewardedAds
        }

        public GetType Type;
        public int Price;
        public string SkinAddressableName;
        public bool IsOpen;
    }
}