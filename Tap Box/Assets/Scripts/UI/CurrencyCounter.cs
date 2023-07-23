using System.Collections.Generic;
using Currency;
using Cysharp.Threading.Tasks;
using Managers;
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
        [SerializeField] private CoinsCounterObject animationObject;
        [SerializeField] private int animationObjectCount;

        private int _targetValue = 0;
        private int _currentValue;
        private bool _isAnimationCompleate = true;

        public async void Initialize()
        {
            icon.sprite = await AssetProvider.LoadAssetAsync<Sprite>($"{type}_icon");
            _currentValue = GameManager.Instance.CurrencyController.GetCurrency(type);
            count.text = _currentValue.ToString();
            GameManager.Instance.CurrencyController.OnCurrencyCountChanged += CurrencyCountChanged;
            Debug.LogError("--------init");
        }

        public bool IsAnimationComplete()
        {
            return _isAnimationCompleate;
        }

        public async void CoinsAnimation(Transform startPosition)
        {
            List<CoinsCounterObject> objects = new List<CoinsCounterObject>();
            for (int i = 0; i < animationObjectCount; i++)
            {
                CoinsCounterObject x = Instantiate(animationObject, Vector3.zero, Quaternion.identity, startPosition);
                x.transform.localPosition = Vector3.zero;
                x.transform.SetParent(iconTransform);
                x.transform.localScale = Vector3.zero;
                objects.Add(x);
            }

            foreach (var coinsCounterObject in objects)
            {
                await UniTask.Delay(100);
                coinsCounterObject.PlayScaleAnimation();
                coinsCounterObject.PlayAnimation(iconTransform.position);
            }
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
            if (newValue > _currentValue)
            {
                await IncrementGold();
            }
            else
            {
                _currentValue = GameManager.Instance.CurrencyController.GetCurrency(type);
                count.text = _currentValue.ToString();
            }

            _isAnimationCompleate = true;
        }

        private async UniTask IncrementGold()
        {
            int time = 100;
            float differance = _targetValue - _currentValue;
            differance /= 15;

            float currentValue = _currentValue;
            while (_targetValue >= (int)currentValue)
            {
                count.text = ((int)currentValue).ToString();
                currentValue += differance;
                currentValue = Mathf.Clamp(currentValue, 0, _targetValue + 1);

                LayoutRebuilder.ForceRebuildLayoutImmediate(count.transform as RectTransform);
                await UniTask.Delay(time);
            }

            _currentValue = (int)currentValue;
        }
    }
}