using System.Collections.Generic;
using System.Linq;
using Core.MessengerStatic;
using Currency;
using DefaultNamespace.Managers;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Skins
{
    public class SkinsPoUp : PopUpBase
    {
        [SerializeField] private Image _background;
        [SerializeField] private CurrencyCounter counter;
        [SerializeField] private List<SkinsButton> _boxesSkinsButtons;
        [SerializeField] private List<SkinsButton> _backgroundsSkinsButtons;
        [SerializeField] private List<SkinsButton> _tailButtons;
        [SerializeField] private List<SkinsButton> _tapButtons;
        [SerializeField] private List<StateButton> _topButtons;

        public override void Initialize()
        {
            Setup();
            Messenger<CurrencyController.Type, string>.AddListener(Constants.Events.OnGetRandomSkin, OnGetRandomSkin);
        }

        private void OnGetRandomSkin(CurrencyController.Type arg1, string arg2)
        {
            Setup();
        }

        private void Setup()
        {
            foreach (var button in _topButtons)
            {
                button.Initialize();
                button.OnClick += OnTopButtonStateChanged;
            }

            SetPage(_boxesSkinsButtons);
            SetPage(_backgroundsSkinsButtons);
            SetPage(_tailButtons);
            SetPage(_tapButtons);
        }

        private void OnTopButtonStateChanged(StateButton arg1, bool arg2)
        {
            foreach (var button in _topButtons)
            {
                button.SetState(button == arg1);
            }
        }

        private void SetPage(List<SkinsButton> buttons)
        {
            foreach (var button in buttons)
            {
                SetSkinData(button);
            }

            foreach (var button in buttons)
            {
                button.Setup();
            }
        }

        private void SetSkinData(SkinsButton button)
        {
            var data = GameManager.Instance.Progress.SkinDatas.FirstOrDefault(x
                => x.SkinAddressableName == button.GetSkinData().SkinAddressableName);
            if (data == null)
            {
                var forCopy = button.GetSkinData();

                GameManager.Instance.Progress.SkinDatas.Add(new SkinData()
                {
                    IsOpen = forCopy.IsOpen,
                    Price = forCopy.Price,
                    SkinAddressableName = forCopy.SkinAddressableName,
                    WayToGet = forCopy.WayToGet,
                    Type = forCopy.Type,
                });
            }
            else
            {
                data = GameManager.Instance.Progress.SkinDatas.FirstOrDefault(x
                    => x.SkinAddressableName == button.GetSkinData().SkinAddressableName);
                button.SetSkinData(data);
            }
        }

        private void Start()
        {
            ID = Constants.PopUps.SkinsPopUp;
            Priority = 1;
        }

        public override async void Show()
        {
            gameObject.SetActive(true);
            await counter.UpdateLayout();
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Messenger<CurrencyController.Type, string>.RemoveListener(Constants.Events.OnGetRandomSkin, OnGetRandomSkin);
        }
    }
}