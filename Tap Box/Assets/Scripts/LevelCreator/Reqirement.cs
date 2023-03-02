using System;
using System.Linq;
using LevelCreator;

[Serializable]
public class Reqirement
{
    public enum RequirementType
    {
        PassedLevel = 0,
    }
    
    public bool CheckForDone()
    {
        switch (Type)
        {
            case RequirementType.PassedLevel:
                var level = Managers.Instance.Progress.LevelDatas.FirstOrDefault(x => x.ID == Value);
                if (level != null && level.LevelStatus == Status.Passed)
                {
                    return true;
                }

                break;
        }

        return false;
    }

    public RequirementType Type;
    public string Value;
}