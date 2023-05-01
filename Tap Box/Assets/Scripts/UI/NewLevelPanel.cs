using Currency;
using Cysharp.Threading.Tasks;
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
        [SerializeField] private GameObject _reward;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private CurrencyCounter _currencyCounter;
        [SerializeField] private Transform _image;


        public override void Initialize()
        {
            ID = Constants.PopUps.NewLevelPopUp;
            Priority = 100;
            GameManager.Instance.PlayerLevelManager.OnLevelChanged += LevelChanged;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener((() => GameManager.Instance.UIManager.ClosePopUp(ID)));
            _currencyCounter.Initialize();

        }

        public override async void Show()
        {
            button.gameObject.SetActive(false);
            GameManager.Instance.SoundManager.Play(new ClipDataMessage() { Id = Constants.Sounds.UI.NewLevelWindowShow, SoundType = SoundData.SoundType.UI });
            gameObject.SetActive(true);
            await _currencyCounter.UpdateLayout();
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }

        private async void LevelChanged(int obj)
        {
            int count = GameManager.Instance.GameField.GetCountOfReward() / 10;
            _reward.SetActive(true);
            _countOfReward.text = $"+{count}";
            _levelText.text = $"You get {obj} level";
            await UniTask.Delay(1000);
             _currencyCounter.CoinsAnimation(_image.transform);
             await UniTask.Delay(500);
             GameManager.Instance.CurrencyController.AddCurrency(CurrencyController.Type.Coin, count);
             await UniTask.Delay(500);
             button.gameObject.SetActive(true);
             await GameManager.Instance.Progress.Save();
        }

        private void OnDestroy()
        {
            GameManager.Instance.PlayerLevelManager.OnLevelChanged -= LevelChanged;
        }
    }
}