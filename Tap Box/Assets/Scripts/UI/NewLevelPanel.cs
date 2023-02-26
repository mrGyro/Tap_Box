using Unity.VisualScripting;
using UnityEngine;

public class NewLevelPanel : MonoBehaviour, IInitializable
{
    public void Initialize()
    {
        Managers.Instance.PlayerLevelManager.OnLevelChanged += LevelChanged;
    }

    private void LevelChanged(int obj)
    {
        gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        Managers.Instance.PlayerLevelManager.OnLevelChanged -= LevelChanged;
    }
}
