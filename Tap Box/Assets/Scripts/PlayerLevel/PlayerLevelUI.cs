using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text currentLevel;
    [SerializeField] private TMP_Text nextLevel;
    [SerializeField] private Slider progress;

    private void Start()
    {
        currentLevel.text = Game.Instance.Progress.CurrentPlayerLevel.ToString();
        nextLevel.text = (Game.Instance.Progress.CurrentPlayerLevel + 1).ToString();
        progress.value = Game.Instance.Progress.CurrentPlayerLevelProgress;
    }
}
