using System;
using System.Collections.Generic;
using System.Linq;
using OctanGames.Extensions;
using UnityEngine;

namespace OctanGames.Gameplay
{
    public class GridModel
    {
        public const int COUNT_MATCHES = 3;
        public const int EMPTY_CELL = 0;

        public event Action<Vector2Int, Vector2Int> CellPositionChanged;
        public event Action<List<FallData>> CellsFell;
        public event Action<List<Vector2Int>> CellsDestroyed;
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

        public void MoveCell(Vector2Int startPosition, Vector2Int endPosition)
        {
            SwapCells(startPosition, endPosition);

            CellPositionChanged?.Invoke(startPosition, endPosition);

            bool isLevelFinished;
            bool isCellsDestroying;

            do
            {
                bool isCellsFalling;
                do
                {
                    isCellsFalling = CheckFallingCells();
                } while (isCellsFalling);

                isCellsDestroying = CheckDestroyingCells();
                isLevelFinished = IsLevelFinished();

            } while (isCellsDestroying && !isLevelFinished);
        }
        private void SwapCells(Vector2Int startPosition, Vector2Int endPosition)
        {
            int firstElement = _map.GetElementByIndex(startPosition);
            int secondElement = _map.GetElementByIndex(endPosition);

            _map.SetElementByIndex(startPosition, secondElement);
            _map.SetElementByIndex(endPosition, firstElement);
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

        private bool CheckDestroyingCells()
        {
            var destroyIndexes = new List<Vector2Int>();

            CheckVerticalMatches(destroyIndexes);
            CheckHorizontalMatches(destroyIndexes);

            destroyIndexes = destroyIndexes.Distinct().ToList();

            ResetCells(destroyIndexes);

            bool isDestroy = destroyIndexes.Count > 0;
            if (isDestroy)
            {
                CellsDestroyed?.Invoke(destroyIndexes);
            }

            return isDestroy;
        }
        private void CheckVerticalMatches(List<Vector2Int> destroyIndexes)
        {
            for (var i = 0; i < _rows; i++)
            {
                var columnIndexes = new List<Vector2Int>();
                for (var j = 1; j < _columns; j++)
                {
                    if (_map[i, j] != EMPTY_CELL && _map[i, j] == _map[i, j - 1])
                    {
                        if (columnIndexes.Count == 0)
                        {
                            columnIndexes.Add(new Vector2Int(i, j - 1));
                        }

                        columnIndexes.Add(new Vector2Int(i, j));
                    }
                    else
                    {
                        if (columnIndexes.Count >= COUNT_MATCHES)
                        {
                            destroyIndexes.AddRange(columnIndexes);
                        }

                        columnIndexes.Clear();
                    }
                }

                if (columnIndexes.Count >= COUNT_MATCHES)
                {
                    destroyIndexes.AddRange(columnIndexes);
                }
            }
        }
        private void CheckHorizontalMatches(List<Vector2Int> destroyIndexes)
        {
            for (var j = 0; j < _columns; j++)
            {
                var rowIndexes = new List<Vector2Int>();
                for (var i = 1; i < _rows; i++)
                {
                    if (_map[i, j] != EMPTY_CELL && _map[i, j] == _map[i - 1, j])
                    {
                        if (rowIndexes.Count == 0)
                        {
                            rowIndexes.Add(new Vector2Int(i - 1, j));
                        }

                        rowIndexes.Add(new Vector2Int(i, j));
                    }
                    else
                    {
                        if (rowIndexes.Count >= COUNT_MATCHES)
                        {
                            destroyIndexes.AddRange(rowIndexes);
                        }

                        rowIndexes.Clear();
                    }
                }

                if (rowIndexes.Count >= COUNT_MATCHES)
                {
                    destroyIndexes.AddRange(rowIndexes);
                }
            }
        }
        private void ResetCells(List<Vector2Int> destroyIndexes)
        {
            foreach (Vector2Int index in destroyIndexes)
            {
                _map.SetElementByIndex(index, EMPTY_CELL);
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