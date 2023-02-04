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
        public int Price;
        public int BestResult;
        public List<BoxData> Data;
    }
    
    public enum Status
    {
        None,
        Close,
        Open,
        Passed
    }
}