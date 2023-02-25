using Currency;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrensyCounter : MonoBehaviour
{
    [SerializeField] private RectTransform countRectTransform;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text count;
    [SerializeField] private CurrencyController.Type type;

    private async void Start()
    {
        await UniTask.WaitUntil(() => Game.Instance.CurrencyController.IsInitialized());
        Debug.LogError(Game.Instance.CurrencyController.IsInitialized());
        icon.sprite = await AssetProvider.LoadAssetAsync<Sprite>(type.ToString());
        count.text = Game.Instance.CurrencyController.GetCurrency(type).ToString();
        Game.Instance.CurrencyController.OnCurrencyCountChanged += CurrencyCountChanged;
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
        Game.Instance.CurrencyController.OnCurrencyCountChanged -= CurrencyCountChanged;
    }

    private async UniTask UpdateLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(countRectTransform);
        await UniTask.Yield();
        LayoutRebuilder.ForceRebuildLayoutImmediate(countRectTransform);
    }
}