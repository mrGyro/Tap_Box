using System;
using Currency;
using TMPro;
using UI.Skins;
using UnityEngine;
using UnityEngine.UI;

public class SkinsButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private GameObject GetTypeGameObject;
    [SerializeField] private Image icon;
    [SerializeField] private Image getTypeBg;
    [SerializeField] private Image getTypeIcon;
    [SerializeField] private TMP_Text getTypeText;
    [Space] [SerializeField] private SkinData data;

    public void Start()
    {
        Setup();
    }

    private async void Setup()
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);

        if (data.IsOpen)
        {
            GetTypeGameObject.SetActive(false);
            return;
        }

        GetTypeGameObject.SetActive(true);

        switch (data.Type)
        {
            case CurrencyController.Type.Coin:
                getTypeIcon.sprite = await AssetProvider.LoadAssetAsync<Sprite>($"{Constants.Currency.Coins}_icon");
                getTypeText.text = data.Price.ToString();
                break;
            case CurrencyController.Type.Ads:
                getTypeIcon.sprite = await AssetProvider.LoadAssetAsync<Sprite>($"{Constants.Currency.Ads}_icon");
                getTypeText.text = "Open";
                break;
        }
    }

    private async void OnClick()
    {
        if (data.IsOpen)
        {
            await Managers.Instance.Progress.ChangeBlock(data.SkinAddressableName);
            return;
        }
        
        switch (data.Type)
        {
            case CurrencyController.Type.Coin:
                if (!Managers.Instance.Progress.Currencies.ContainsKey(data.Type))
                    return;
                
                if (Managers.Instance.Progress.Currencies[data.Type] >= data.Price)
                {
                    Managers.Instance.Progress.Currencies[data.Type] -= data.Price;
                    Managers.Instance.CurrencyController.RemoveCurrency(data.Type, data.Price);
                    data.IsOpen = true;
                }
                else
                {
                    return;
                }

                break;
            case CurrencyController.Type.Ads:
                return;
        }
        
        Setup();
        await Managers.Instance.Progress.ChangeBlock(data.SkinAddressableName);
    }
}