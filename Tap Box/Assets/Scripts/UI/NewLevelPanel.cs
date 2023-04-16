using Currency;
using DefaultNamespace.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NewLevelPanel : PopUpBase
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text _countOfReward;

        public override void Initialize()
        {
            ID = Constants.PopUps.NewLevelPopUp;
            Priority = 100;
            Managers.Instance.PlayerLevelManager.OnLevelChanged += LevelChanged;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener((() => Managers.Instance.UIManager.ClosePopUp(ID)));
        }

        public override void Show()
        {
            gameObject.SetActive(true);
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }

        private void LevelChanged(int obj)
        {
            int count = Managers.Instance.GameField.GetCountOfReward() / 10;
            _countOfReward.text = $"+{count}";
            Managers.Instance.CurrencyController.AddCurrency(CurrencyController.Type.Coin, count);
        }

        private void OnDestroy()
        {
            Managers.Instance.PlayerLevelManager.OnLevelChanged -= LevelChanged;
        }
    }
}