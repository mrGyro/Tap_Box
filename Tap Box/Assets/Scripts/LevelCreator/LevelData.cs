using System;
using System.Collections.Generic;
using Boxes;

namespace LevelCreator
{
    [Serializable]
    public class LevelData
    {
        public string ID;
        public Status LevelStatus;
        public int Reward;
        public int BestResult;
        public List<BoxData> Data;
    }

    public enum Status
    {
        None = 0,
        Close = 1,
        Open = 2,
        Passed = 3
    }
}