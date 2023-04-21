using Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelUI : MonoBehaviour, IInitializable
{
    [SerializeField] private TMP_Text currentLevel;
    [SerializeField] private TMP_Text nextLevel;
    [SerializeField] private Slider progress;

    public void Initialize()
    {
        currentLevel.text = GameManager.Instance.Progress.CurrentPlayerLevel.ToString();
        nextLevel.text = (GameManager.Instance.Progress.CurrentPlayerLevel + 1).ToString();
        progress.value = GameManager.Instance.Progress.CurrentPlayerLevelProgress;

        GameManager.Instance.PlayerLevelManager.OnLevelChanged += LevelChanged;
        GameManager.Instance.PlayerLevelManager.OnLevelProgressChanged += LevelProgressChanged;
    }

    private void LevelProgressChanged(float value)
    {
        progress.value = value;
    }

    private void LevelChanged(int value)
    {
        nextLevel.text = (value + 1).ToString();
        currentLevel.text = value.ToString();
    }
}