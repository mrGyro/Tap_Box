using LevelCreator;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance;

    public GameField GameField;
    public LevelsWindiw LevelsWindiw;
    public InputController InputController;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        LevelsWindiw.Setup();
    }

    public void UpdateLevel(LevelData level)
    {
        LevelsWindiw.UpdateLevel(level);
    }
}
