using System;
using System.Collections.Generic;
using System.Linq;
using Core.MessengerStatic;
using DefaultNamespace.Managers;
using DefaultNamespace.UI.Popup;
using Managers;
using TMPro;
using UI;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, IInitializable
{
    [SerializeField] private Image _background;
    [SerializeField] private CurrencyCounter coinCounter;
    [SerializeField] private PlayerLevelUI playerLevelUI;
    [SerializeField] private TurnsLeftCounter turnsLeftCounter;
    [SerializeField] private TMP_Text _currentLevelText;
    [SerializeField] private BombBoosterUI _bombBoosterUI;
    [SerializeField] private Button _btnNoAds;

    [SerializeField] private List<PopUpBase> popups;
    private List<PopUpBase> _popUpsQueue = new();
    private IDisposable isBanerRedy;
    public void Initialize()
    {
        GameManager.Instance.SkinsManager.AddBackground(_background);
        GameManager.Instance.SkinsManager.SetBackgroundSkinSprite(_background);

        coinCounter.Initialize();
        playerLevelUI.Initialize();
        turnsLeftCounter.Initialize();

        foreach (var variable in popups)
        {
            variable.Initialize();
        }
        Messenger<string>.AddListener(Constants.Events.OnLevelCreated, OnLevelChanged);
        Messenger<string>.AddListener(Constants.IAP.PurchaseSuccess, OnPurchaseSuccess);
        _btnNoAds.onClick.AddListener(BuyNoAds);
        _btnNoAds.gameObject.SetActive(!GameManager.Instance.IAPManager.HasNonConsumableProduct(Constants.IAP.NoAds));

        
        _currentLevelText.text = "Level " + GameManager.Instance.Progress.LastStartedLevelID;
        _bombBoosterUI.Initialize();

        var bannerAd = GameManager.Instance.Mediation.GetAddElement(Constants.Ads.Banner);
        if (bannerAd != null)
        {
            isBanerRedy = bannerAd.IsReady.Subscribe(OnBannerReady);
        }

//        OnBannerReady(false);
    }
    
    private void BuyNoAds()
    {
        Debug.Log("---BuyNoAds");
        GameManager.Instance.IAPManager.BuyProduct(Constants.IAP.NoAds);
    }

    private void OnPurchaseSuccess(string obj)
    {
        Debug.Log("OnPurchaseSuccess");

        if (obj != Constants.IAP.NoAds)
        {
            return;
        }
        Debug.Log("hide after purcase");
        _btnNoAds.gameObject.SetActive(false);
        GameManager.Instance.Mediation.GetAddElement(Constants.Ads.Banner).isEnable = false;
        GameManager.Instance.Mediation.Hide(Constants.Ads.Banner);

    }

    private void OnBannerReady(bool value)
    {
        Debug.Log("HasNoAds = " + GameManager.Instance.IAPManager.HasNonConsumableProduct(Constants.IAP.NoAds) + " value = " + value);
        if (GameManager.Instance.IAPManager.HasNonConsumableProduct(Constants.IAP.NoAds))
        {
            Debug.Log("hide bunner");
            GameManager.Instance.Mediation.GetAddElement(Constants.Ads.Banner).isEnable = false;
            GameManager.Instance.Mediation.Hide(Constants.Ads.Banner);
        }
        else
        {
            Debug.Log("Show bunner");

            GameManager.Instance.Mediation.GetAddElement(Constants.Ads.Banner).isEnable = true;
            GameManager.Instance.Mediation.Show(Constants.Ads.Banner, "UIManager");
        }
    }
    
    public int GetBombCos()
    {
        return _bombBoosterUI.GetPrice();
    }

    public void ClickOnBomb()
    {
        _bombBoosterUI.ClickOnBombButton();
    }

    private void OnLevelChanged(string obj)
    {
        _currentLevelText.text = "Level " + obj;
    }

    public void ShowTurns()
    {
        turnsLeftCounter.SetTurnsText();
    }

    public void ShowPopUp(string id)
    {
        var popup = popups.Find(x => x.ID == id);
        if (popup == null)
            return;
        
        AddToPopUpQueue(popup);
        ShowNext();
    }
    
    public void ShowUpToAllPopUp(string id)
    {
        var popup = popups.Find(x => x.ID == id);
        if (popup == null)
            return;
        
        
        GameManager.Instance.SetActiveGlobalInput(false);
        popup.Show();
        popup.IsShowing = true;
    }

    public void ClosePopUp(string id)
    {
        var popup = popups.Find(x => x.ID == id);
        isBanerRedy?.Dispose();
        if (popup == null)
            return;

        popup.Close();
        popup.IsShowing = false;

        RemoveFromPopUpQueue(popup);
        GameManager.Instance.SetActiveGlobalInput(true);
        ShowNext();
    }

    public PopUpBase GetPopupByID(string id)
    {
        return popups.FirstOrDefault(x => x.ID == id);
    }

    private void AddToPopUpQueue(PopUpBase popUpBase)
    {
        if (_popUpsQueue.Exists(x => x.ID == popUpBase.ID))
            return;
        
        _popUpsQueue.Add(popUpBase);
        _popUpsQueue.Sort(new PopUpComparer());
    }

    private void RemoveFromPopUpQueue(PopUpBase popUpBase)
    {
        _popUpsQueue.Remove(popUpBase);
    }

    private void ShowNext()
    {
        if (_popUpsQueue.Count == 0)
            return;

        if (_popUpsQueue.Find(x => x.IsShowing) != null)
            return;

        GameManager.Instance.SetActiveGlobalInput(false);
        _popUpsQueue[^1].Show();
        _popUpsQueue[^1].IsShowing = true;
    }
}