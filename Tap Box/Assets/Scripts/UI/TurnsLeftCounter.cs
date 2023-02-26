using Boxes;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class TurnsLeftCounter : MonoBehaviour, IInitializable
{
    [SerializeField] private TMP_Text counter;

    public void Initialize()
    {
        Core.MessengerStatic.Messenger<BaseBox>.AddListener(Constants.Events.OnBoxRemoveFromGameField, OnBoxRemoved);
        Core.MessengerStatic.Messenger<string>.AddListener(Constants.Events.OnLevelCreated, OnLevelCreated);
    }

    private void OnLevelCreated(string obj)
    {
        SetTurnsText();
    }

    private void OnBoxRemoved(BaseBox obj)
    {
        SetTurnsText();
    }

    private void SetTurnsText()
    {
        counter.text = $"{Managers.Instance.GameField.GetBoxCount} turns";
    }

    private void OnDestroy()
    {
        Core.MessengerStatic.Messenger<BaseBox>.RemoveListener(Constants.Events.OnBoxRemoveFromGameField, OnBoxRemoved);
        Core.MessengerStatic.Messenger<string>.RemoveListener(Constants.Events.OnLevelCreated, OnLevelCreated);
    }
}