using System;
using LevelCreator;

[Serializable]
public class Reqirement
{
    public enum RequirementType
    {
        PassedLevel = 0,
    }

    public RequirementType Type;
    public string Value;

    public bool CheckForDone()
    {
        switch (Type)
        {
            case RequirementType.PassedLevel:
                var level = Managers.Instance.LevelsWindiw.uiLevelItems.Find(x => x.Data.ID == Value);
                if (level.Data.LevelStatus == Status.Passed)
                {
                    return true;
                }

                break;
        }

        return false;
    }
}