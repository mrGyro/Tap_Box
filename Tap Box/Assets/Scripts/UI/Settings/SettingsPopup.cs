using DefaultNamespace.Managers;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Settings
{
    public class SettingsPopup : PopUpBase
    {
        [SerializeField] private Button _button;
        [SerializeField] private CurrencyCounter _currencyCounter;

        public override void Initialize()
        {
            ID = Constants.PopUps.SettingsPopup;
            Priority = 100;
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener((() =>
            {
                GameManager.Instance.UIManager.ClosePopUp(ID);
            }));
            _currencyCounter.Initialize();
        }
        
        public override async void Show()
        {
            gameObject.SetActive(true);
            await _currencyCounter.UpdateLayout();
        }
        
        public override void Close()
        {
            gameObject.SetActive(false);
        }
    }
}