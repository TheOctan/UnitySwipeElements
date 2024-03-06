using System;

namespace OctanGames.Gameplay
{
    public class GridModel
    {
        public const int EMPTY_CELL = 0;
        
        public event Action LevelFinished;

        private readonly int[,] _map;
        private readonly int _rows;
        private readonly int _columns;

        public GridModel(int[,] map, int rows, int columns)
        {
            _map = map;
            _rows = rows;
            _columns = columns;
        }

        private bool IsLevelFinished()
        {
            for (var i = 0; i < _rows; i++)
            {
                for (var j = 0; j < _columns; j++)
                {
                    if (_map[i, j] != EMPTY_CELL)
                    {
                        return false;
                    }
                }
            }

            LevelFinished?.Invoke();
            return true;
        }
    }
}