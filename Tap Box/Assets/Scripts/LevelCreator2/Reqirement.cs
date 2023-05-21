using System;
using System.Linq;
using LevelCreator;
using Managers;
using UnityEngine;

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
                var level = GameManager.Instance.Progress.LevelDatas.FirstOrDefault(x => x.ID == Value);
                if (level == null)
                    return false;

                return level.LevelStatus == Status.Passed;
        }

        return false;
    }


}