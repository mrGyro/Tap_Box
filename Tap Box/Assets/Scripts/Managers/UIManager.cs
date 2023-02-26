using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour, IInitializable
{
    [SerializeField] private CurrensyCounter coinCounter;
    [SerializeField] private PlayerLevelUI playerLevelUI;
    [SerializeField] private NewLevelPanel newLevelPanel;
    [SerializeField] private TurnsLeftCounter turnsLeftCounter;
    
    public void Initialize()
    {
        coinCounter.Initialize();
        playerLevelUI.Initialize();
        newLevelPanel.Initialize();
        turnsLeftCounter.Initialize();
    }
}
