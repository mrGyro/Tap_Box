using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Managers;
using UnityEngine;

namespace UI.Skins
{
    public class SkinsPoUp : PopUpBase
    {
        [SerializeField] private CurrencyCounter counter;
        [SerializeField] private List<SkinsButton> skinsButtons;

        public override void Initialize()
        {
            foreach (var data in Managers.Instance.Progress.SkinDatas)
            {
                var button = skinsButtons.FirstOrDefault(x => x.GetSkinData().SkinAddressableName == data.SkinAddressableName);
                if (button == null)
                {
                    continue;
                }

                button.SetSkinData(data);
            }

            foreach (var button in skinsButtons)
            {
                button.Setup();
            }
        }

        private void Start()
        {
            ID = Constants.PopUps.SkinsPopUp;
            Priority = 1;
        }

        public override void Show()
        {
            gameObject.SetActive(true);
            counter.UpdateLayout();
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }
    }
}