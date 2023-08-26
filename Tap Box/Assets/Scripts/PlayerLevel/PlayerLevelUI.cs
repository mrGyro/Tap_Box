using Managers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelUI : MonoBehaviour, IInitializable
{
    [SerializeField] private Slider progress;

    public void Initialize()
    {
        progress.value = GameManager.Instance.Progress.CurrentPlayerLevelProgress;
        GameManager.Instance.PlayerLevelManager.OnLevelProgressChanged += LevelProgressChanged;
    }

    private void LevelProgressChanged(float value)
    {
        progress.value = value;
    }
}