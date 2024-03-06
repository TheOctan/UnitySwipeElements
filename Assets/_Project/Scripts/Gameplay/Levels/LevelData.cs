using System;

namespace OctanGames.Gameplay.Levels
{
    [Serializable]
    public struct LevelData
    {
        public int[,] Map;
        public int CurrentLevel;
    }
}