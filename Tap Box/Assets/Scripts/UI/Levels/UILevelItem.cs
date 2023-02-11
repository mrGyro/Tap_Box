using LevelCreator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevelItem : MonoBehaviour
{
    public Status ButtonType;
    public LevelData Data;

    [SerializeField] private TMP_Text levelNumberText;
    [SerializeField] private TMP_Text status;
    [SerializeField] private Image icon;
    [SerializeField] private Button interactButton;
    [SerializeField] private TMP_Text interactButtonText;

    [SerializeField] private Image coinIcon;
    [SerializeField] private TMP_Text priceText;

    public void Setup(LevelData data)
    {
        // switch (status)
        // {
        //     case Status.None:
        //         break;
        //     case Status.Open:
        //         asset = AddressableUnlockLevelItem;
        //         break;
        //     case Status.Passed:
        //         asset = LevelButtonAddressable;
        //         break;
        //     case Status.Close:
        //         asset = AddressableLockLevelItem;
        //         break;
        // }
        
        if (levelNumberText != null)
            levelNumberText.text = data.ID;
        if (status != null)
            status.text = data.LevelStatus.ToString();
        if (interactButton != null)
        {
            interactButton.onClick.RemoveAllListeners();
            interactButton.onClick.AddListener(OnButtonClick);
        }

        Data = data;
    }

    public void UpdateButton(LevelData data)
    {
        
    }


    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    private void OnButtonClick()
    {
        Game.Instance.GameField.LoadLevelByName("Level_" + Data.ID);
        Game.Instance.GameField.SetActiveLevelPanel(false);
    }
}