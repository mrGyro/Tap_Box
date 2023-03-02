using System;
using Currency;

namespace UI.Skins
{
    [Serializable]
    public class SkinData
    {
        

        public CurrencyController.Type Type;
        public int Price;
        public string SkinAddressableName;
        public bool IsOpen;
    }
}