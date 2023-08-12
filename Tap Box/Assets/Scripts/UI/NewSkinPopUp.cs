using Core.MessengerStatic;
using Currency;
using DefaultNamespace.Managers;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NewSkinPopUp : PopUpBase
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Button _closePopup;
        [SerializeField] private Button _equipButton;

        private CurrencyController.Type _skinType;
        private string _skinAddressabpeId;
        public override void Initialize()
        {
            ID = Constants.PopUps.NewSkinPopUp;
            Priority = 100;
            _closePopup.onClick.RemoveAllListeners();
            _closePopup.onClick.AddListener(() =>
            {
                GameManager.Instance.UIManager.ClosePopUp(ID);
            });
            
            _equipButton.onClick.RemoveAllListeners();
            _equipButton.onClick.AddListener(EquipSkin);
            
            Messenger<CurrencyController.Type, string>.AddListener(Constants.Events.OnGetNewSkin, NewRandomSkin);
        }

        private void EquipSkin()
        {
            Messenger<CurrencyController.Type, string>.Broadcast(Constants.Events.OnEquipSkin, _skinType, _skinAddressabpeId);
            _closePopup.onClick.Invoke();
        }

        private async void NewRandomSkin(CurrencyController.Type arg1, string arg2)
        {
            _skinType = arg1;
            _skinAddressabpeId = arg2;
            var sprite = await AssetProvider.LoadAssetAsync<Sprite>(arg2 + "_icon");
            _icon.sprite = sprite;
        }

        public override void Show()
        {
            gameObject.SetActive(true);
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }
    }
}