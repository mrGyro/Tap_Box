using TMPro;
using UI.Skins;
using UnityEngine;
using UnityEngine.UI;

public class SkinsButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private Image getTypeBg;
    [SerializeField] private Image getTypeIcon;
    [SerializeField] private TMP_Text getTypeText;
    [Space]
    [SerializeField] private SkinData data;

    public void Start()
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private async void OnClick()
    {
        await Managers.Instance.Progress.ChangeBlock(data.SkinAddressableName);
    }
}
