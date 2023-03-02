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
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text count;
        [SerializeField] private CurrencyController.Type type;

        public async void Initialize()
        {
            icon.sprite = await AssetProvider.LoadAssetAsync<Sprite>(type.ToString());
            count.text = Managers.Instance.CurrencyController.GetCurrency(type).ToString();
            Managers.Instance.CurrencyController.OnCurrencyCountChanged += CurrencyCountChanged;
            await UpdateLayout();
        }

        private async void CurrencyCountChanged(CurrencyController.Type arg1, int arg2)
        {
            if (arg1 != type) return;
            count.text = arg2.ToString();
            await UpdateLayout();
        }

        private void OnDestroy()
        {
            Managers.Instance.CurrencyController.OnCurrencyCountChanged -= CurrencyCountChanged;
        }

        private async UniTask UpdateLayout()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(countRectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(textRectTransform);
            await UniTask.Yield();
            LayoutRebuilder.ForceRebuildLayoutImmediate(countRectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(textRectTransform);
        }

    
    }
}