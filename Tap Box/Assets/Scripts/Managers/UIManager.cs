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
            variable.Initialize();
    }

    public void ShowPopUp(string id)
    {
        var popup = popups.Find(x => x.ID == id);
        if (popup == null)
            return;
        
        AddToPopUpQueue(popup);
        Debug.LogError("==== add " + popup.ID);
        ShowNext();
    }

    public void ClosePopUp(string id)
    {
        Debug.LogError("==1== close " + id);

        var popup = popups.Find(x => x.ID == id);
        if (popup == null)
            return;

        popup.Close();
        popup.IsShowing = false;
        Debug.LogError("==2== close " + popup.ID);

        RemoveFromPopUpQueue(popup);

        ShowNext();
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

        Debug.LogError("==== show newxt " + _popUpsQueue[^1].ID);

        _popUpsQueue[^1].Show();
        _popUpsQueue[^1].IsShowing = true;

    }
}