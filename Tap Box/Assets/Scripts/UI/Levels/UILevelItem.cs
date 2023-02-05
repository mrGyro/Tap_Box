using LevelCreator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevelItem : MonoBehaviour
{
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
        levelNumberText.text = data.ID;
        status.text = data.LevelStatus.ToString();
        interactButton.onClick.RemoveAllListeners();
        interactButton.onClick.AddListener(OnButtonClick);
        //icon ==
        _data = data;
    }

    private void OnButtonClick()
    {
        GameField.Instance.LoadLevelByName("Level_" + _data.ID);
        GameField.Instance.SetActiveLevelPanel(false);
    }
}