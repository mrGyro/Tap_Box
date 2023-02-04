using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.UI.Levels;
using JetBrains.Annotations;
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

    public void Setup(LevelButtonData data)
    {
        levelNumberText.text = data.levelNumberText;
        status.text = data.status;
        //icon ==
    }
}