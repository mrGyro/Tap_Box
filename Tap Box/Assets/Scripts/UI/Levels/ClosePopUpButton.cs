using UnityEngine;
using UnityEngine.UI;

public class ClosePopUpButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private string popupID;

    void Start()
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        Managers.Instance.UIManager.ClosePopUp(popupID);
    }
}
