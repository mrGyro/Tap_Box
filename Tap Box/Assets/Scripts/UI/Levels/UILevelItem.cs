using LevelCreator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevelItem : MonoBehaviour
{
    public Status ButtonType;
    [SerializeField] private TMP_Text levelNumberText;
    [SerializeField] private TMP_Text status;
    [SerializeField] private Image icon;
    [SerializeField] private Button interactButton;
    [SerializeField] private TMP_Text interactButtonText;

    [SerializeField] private Image coinIcon;
    [SerializeField] private TMP_Text priceText;
    private LevelData _data;

    public void Setup(LevelData data)
    {
        if (levelNumberText != null)
            levelNumberText.text = data.ID;
        if (status != null)
            status.text = data.LevelStatus.ToString();
        if (interactButton != null)
        {
            interactButton.onClick.RemoveAllListeners();
            interactButton.onClick.AddListener(OnButtonClick);
        }

        _data = data;
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    private void OnButtonClick()
    {
        GameField.Instance.LoadLevelByName("Level_" + _data.ID);
        GameField.Instance.SetActiveLevelPanel(false);
    }
}