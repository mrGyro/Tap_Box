using Boxes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TurnsLeftCounter : MonoBehaviour, IInitializable
{
    [SerializeField] private TMP_Text counter;

    public void Initialize()
    {
        Core.MessengerStatic.Messenger.AddListener(Constants.Events.OnBoxClicked, OnBoxRemoved);
        Core.MessengerStatic.Messenger<string>.AddListener(Constants.Events.OnLevelCreated, OnLevelCreated);
        Core.MessengerStatic.Messenger.AddListener(Constants.Events.OnGameLoose, OnLoose);
    }

    private void OnLoose()
    {
        Managers.Instance.UIManager.ShowPopUp(Constants.PopUps.LosePopUp);
    }

    private void OnLevelCreated(string obj)
    {
        SetTurnsText();
    }

    private void OnBoxRemoved()
    {
        SetTurnsText();
    }

    private void SetTurnsText()
    {
        counter.text = $"{Managers.Instance.GameField.GetBoxCount} turns";
    }

    private void OnDestroy()
    {
        Core.MessengerStatic.Messenger.RemoveListener(Constants.Events.OnBoxClicked, OnBoxRemoved);
        Core.MessengerStatic.Messenger<string>.RemoveListener(Constants.Events.OnLevelCreated, OnLevelCreated);
        Core.MessengerStatic.Messenger.RemoveListener(Constants.Events.OnGameLoose, OnLoose);
    }
}