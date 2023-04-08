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
        public Reqirement Reqirement;
        public List<BoxData> Data;
        public SerializedVector3 CameraPosition;

        public void UpdateData(LevelData data)
        {
            ID = data.ID;
            LevelStatus = data.LevelStatus;
            Reward = data.Reward;
            BestResult = data.BestResult;
            Data = data.Data;
        }
    }

    public enum Status
    {
        None = 0,
        Close = 1,
        Open = 2,
        Passed = 3
    }
}