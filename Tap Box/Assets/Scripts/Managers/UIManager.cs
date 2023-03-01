using System.Collections.Generic;
using DefaultNamespace.Managers;
using DefaultNamespace.UI.Popup;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour, IInitializable
{
    [SerializeField] private CurrensyCounter coinCounter;
    [SerializeField] private PlayerLevelUI playerLevelUI;
    [SerializeField] private TurnsLeftCounter turnsLeftCounter;

    [SerializeField] private List<PopUpBase> popups;
    private List<PopUpBase> _popUpsQueue = new();

    public void Initialize()
    {
        coinCounter.Initialize();
        playerLevelUI.Initialize();
        turnsLeftCounter.Initialize();

        foreach (var variable in popups)
        {
            variable.Initialize();
        }
    }

    public void ShowPopUp(string id)
    {
        var popup = popups.Find(x => x.ID == id);
        if (popup == null)
            return;

        AddToPopUpQueue(popup);
        ShowNext();
    }

    public void AddToPopUpQueue(PopUpBase popUpBase)
    {
        _popUpsQueue.Add(popUpBase);
        popUpBase.OnClose += OnPopUpClose;
        _popUpsQueue.Sort(new PopUpComparer());

        foreach (var VARIABLE in _popUpsQueue)
        {
            Debug.LogError(VARIABLE.Priority);
        }

        Debug.LogError("-----------------------");
    }

    public void RemoveFromPopUpQueue(PopUpBase popUpBase)
    {
        _popUpsQueue.Remove(popUpBase);
    }

    private void OnPopUpClose(PopUpBase popUpBase)
    {
        RemoveFromPopUpQueue(popUpBase);
        ShowNext();
    }

    private void ShowNext()
    {
        if (_popUpsQueue.Count == 0)
            return;

        if (_popUpsQueue.Find(x => x.IsShowing) != null) 
            return;
        
        _popUpsQueue[^1].Show();
    }
}