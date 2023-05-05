using DefaultNamespace.Managers;
using Managers;
using UI.Skins;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Settings
{
    public class SettingsPopup : PopUpBase
    {
        [SerializeField] private Button _button;
        [SerializeField] private CurrencyCounter _currencyCounter;
        [SerializeField] private StateButton _soundButton;
        [SerializeField] private StateButton _vibrationButton;
        [SerializeField] private Button _restoreButton;
        [SerializeField] private Button _privacyButton;

        public override void Initialize()
        {
            ID = Constants.PopUps.SettingsPopup;
            Priority = 100;
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener((Call));
            _currencyCounter.Initialize();
            
            _soundButton.OnClick += OnSoundClick;
            _vibrationButton.OnClick += OnVibrationClick;
            _restoreButton.onClick.AddListener(OnRestoreClick);
            _privacyButton.onClick.AddListener(OnPrivacyClick);
            
            _soundButton.SetState(GameManager.Instance.Progress.CurrentSoundSetting);
            _vibrationButton.SetState(GameManager.Instance.Progress.CurrentVibroSetting);
        }

        private async void Call()
        {
            await GameManager.Instance.Progress.Save();
            GameManager.Instance.UIManager.ClosePopUp(ID);
        }

        private void OnSoundClick(StateButton arg1, bool arg2)
        {
            GameManager.Instance.Progress.CurrentSoundSetting = !GameManager.Instance.Progress.CurrentSoundSetting;
            _soundButton.SetState(GameManager.Instance.Progress.CurrentSoundSetting);
        }
        
        private void OnVibrationClick(StateButton arg1, bool arg2)
        {
            GameManager.Instance.Progress.CurrentVibroSetting = !GameManager.Instance.Progress.CurrentVibroSetting;
            _vibrationButton.SetState(GameManager.Instance.Progress.CurrentVibroSetting);
        }
        
        private void OnRestoreClick()
        {
        }
        
        private void OnPrivacyClick()
        {
            Application.OpenURL("https://www.mindom.com.ua/privacy-policy/");
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