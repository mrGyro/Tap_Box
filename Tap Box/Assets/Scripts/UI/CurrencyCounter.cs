using Currency;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CurrencyCounter : MonoBehaviour, IInitializable
    {
        [SerializeField] private RectTransform countRectTransform;
        [SerializeField] private RectTransform textRectTransform;
        [SerializeField] private RectTransform iconTransform;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text count;
        [SerializeField] private CurrencyController.Type type;

        private int _targetValue = 0;
        private int _currentValue;
        private bool _isAnimationCompleate = true;

        public async void Initialize()
        {
            icon.sprite = await AssetProvider.LoadAssetAsync<Sprite>($"{type}_icon");
            _currentValue = Managers.Instance.CurrencyController.GetCurrency(type);
            count.text = _currentValue.ToString();
            Managers.Instance.CurrencyController.OnCurrencyCountChanged -= CurrencyCountChanged;
            Managers.Instance.CurrencyController.OnCurrencyCountChanged += CurrencyCountChanged;
        }

        public bool IsAnimationComplete()
        {
            return _isAnimationCompleate;
        }

        public async UniTask UpdateLayout()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(countRectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(textRectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(iconTransform);
            await UniTask.Yield();
            LayoutRebuilder.ForceRebuildLayoutImmediate(countRectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(textRectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(iconTransform);
        }

        private async void CurrencyCountChanged(CurrencyController.Type typeOfCurrency, int newValue)
        {
            if (typeOfCurrency != type)
            {
                return;
            }

            _isAnimationCompleate = false;

            _targetValue = newValue;
            await IncrementGold();
            _isAnimationCompleate = true;
        }

        private async void OnEnable()
        {
            Initialize();
            await UniTask.Delay(50);
           // UpdateLayout();
        }

        private void OnDisable()
        {
            Managers.Instance.CurrencyController.OnCurrencyCountChanged -= CurrencyCountChanged;
        }

        private async UniTask IncrementGold()
        {
            while (_targetValue >= _currentValue)
            {
                count.text = _currentValue.ToString();
                _currentValue++;
                await UniTask.Delay(50);
               // await UpdateLayout();
            }
        }
    }
}