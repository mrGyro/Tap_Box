using System;
using Currency;
using UnityEngine.Serialization;

namespace UI.Skins
{
    [Serializable]
    public class SkinData
    {
        public CurrencyController.Type WayToGet;
        public CurrencyController.Type Type;
        public int Price;
        public float Size;
        public string SkinAddressableName;
        public bool IsOpen;
    }
}