using System;

namespace OctanGames.Gameplay
{
    public class GridModel
    {
        public const int EMPTY_CELL = 0;
        
        public event Action<List<FallData>> CellsFell;
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

        private bool CheckFallingCells()
        {
            List<FallData> fallIndexes = CalculateFallingCells();
            MoveFallingCells(fallIndexes);

            bool isFall = fallIndexes.Count > 0;

            if (isFall)
            {
                CellsFell?.Invoke(fallIndexes.OrderByDescending(c => c.FallStep).ToList());
            }

            return isFall;
        }
        private List<FallData> CalculateFallingCells()
        {
            var fallIndexes = new List<FallData>();

            for (var j = 0; j < _columns; j++)
            {
                var hasEmptyColumn = false;
                var countFallSteps = 0;

                for (int i = _rows - 1; i >= 0; i--)
                {
                    if (_map[i, j] == EMPTY_CELL)
                    {
                        hasEmptyColumn = true;
                        countFallSteps++;
                    }
                    else if (hasEmptyColumn)
                    {
                        fallIndexes.Add(new FallData()
                        {
                            StartIndex = new Vector2Int(i, j),
                            FallStep = countFallSteps
                        });
                    }
                }
            }

            return fallIndexes;
        }
        private void MoveFallingCells(List<FallData> fallIndexes)
        {
            foreach (FallData cellFall in fallIndexes)
            {
                Vector2Int startIndex = cellFall.StartIndex;
                Vector2Int targetIndex = cellFall.StartIndex + Vector2Int.right * cellFall.FallStep;

                int startElement = _map.GetElementByIndex(cellFall.StartIndex);
                _map.SetElementByIndex(targetIndex, startElement);
                _map.SetElementByIndex(startIndex, EMPTY_CELL);
            }
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