using System;
using System.Collections.Generic;
using LevelCreator;

namespace SaveLoad_progress
{
    [Serializable]
    public class GameProgress
    {
        public List<LevelData> LevelDatas;
    }
}