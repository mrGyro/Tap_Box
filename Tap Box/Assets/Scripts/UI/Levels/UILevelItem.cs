using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.UI.Levels;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
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
    private LevelButtonData _data;

    public void Setup(LevelButtonData data)
    {
        levelNumberText.text = data.levelNumberText;
        status.text = data.status;
        interactButton.onClick.RemoveAllListeners();
        interactButton.onClick.AddListener(OnButtonClick);
        //icon ==
        _data = data;
    }

    private void OnButtonClick()
    {
        GameField.Instance.LoadLevelByName("Level_" + _data.levelNumberText);
        GameField.Instance.SetActiveLevelPanel(false);
    }
}