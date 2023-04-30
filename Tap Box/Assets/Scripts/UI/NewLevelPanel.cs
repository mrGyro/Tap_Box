using Currency;
using DefaultNamespace.Managers;
using Managers;
using Sounds;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NewLevelPanel : PopUpBase
    {
        [SerializeField] private Image _background;
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text _countOfReward;
        [SerializeField] private CurrencyCounter _currencyCounter;

        public override void Initialize()
        {
            ID = Constants.PopUps.NewLevelPopUp;
            Priority = 100;
            GameManager.Instance.PlayerLevelManager.OnLevelChanged += LevelChanged;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener((() => GameManager.Instance.UIManager.ClosePopUp(ID)));
        }

        public override async void Show()
        {
            GameManager.Instance.SoundManager.Play(new ClipDataMessage() { Id = Constants.Sounds.UI.NewLevelWindowShow, SoundType = SoundData.SoundType.UI });
            gameObject.SetActive(true);
            await _currencyCounter.UpdateLayout();
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }

        private void LevelChanged(int obj)
        {
            int count = GameManager.Instance.GameField.GetCountOfReward() / 10;
            _countOfReward.text = $"+{count}";
            GameManager.Instance.CurrencyController.AddCurrency(CurrencyController.Type.Coin, count);
        }

        private void OnDestroy()
        {
            GameManager.Instance.PlayerLevelManager.OnLevelChanged -= LevelChanged;
        }
    }
}