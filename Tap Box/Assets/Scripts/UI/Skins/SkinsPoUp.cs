using System.Collections.Generic;
using System.Linq;
using Currency;
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
            Setup();
            Core.MessengerStatic.Messenger<CurrencyController.Type, string>.AddListener(Constants.Events.OnGetRandomSkin, OnGetRandomSkin);

        }

        private void OnGetRandomSkin(CurrencyController.Type arg1, string arg2)
        {
            Setup();
        }

        private void Setup()
        {
            var buffer = new List<SkinData>();

            foreach (var button in skinsButtons)
            {
                var data = Managers.Instance.Progress.SkinDatas.FirstOrDefault(x
                    => x.SkinAddressableName == button.GetSkinData().SkinAddressableName);
                if (data == null)
                {
                    var forCopy = button.GetSkinData();
                    buffer.Add(new SkinData()
                    {
                        IsOpen = forCopy.IsOpen,
                        Price = forCopy.Price,
                        SkinAddressableName = forCopy.SkinAddressableName,
                        Type = forCopy.Type,
                        IsRandom = forCopy.IsRandom
                    });

                    continue;
                }

                button.SetSkinData(data);
            }

            Managers.Instance.Progress.SkinDatas.AddRange(buffer);

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

        private void OnDestroy()
        {
            Core.MessengerStatic.Messenger<CurrencyController.Type, string>.RemoveListener(Constants.Events.OnGetRandomSkin, OnGetRandomSkin);

        }
    }
}