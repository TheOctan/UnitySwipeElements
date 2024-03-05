using System;
using System.Collections.Generic;
using DG.Tweening;
using Extensions;
using OctanGames.Extensions;
using OctanGames.Gameplay.Levels;
using OctanGames.Infrastructure;
using OctanGames.Inputs;
using UnityEngine;

namespace OctanGames.Gameplay
{
    public class TableView : MonoBehaviour
    {
        private const int EMPTY_CELL = 0;
        public event Action<Vector2Int, Vector2Int> CellMoved;

        [SerializeField] private float _width;
        [SerializeField] private float _height;

        private int _rows;
        private int _columns;
        private CellView[,] _cellMap;
        private bool _isAnimated;

        private ILevelLibrary _levelLibrary;
        private CellSettings _cellSettings;
        private readonly Vector2Int _invalidPosition = new(-1,-1);

        private void Start()
        {
            _levelLibrary = ServiceLocator.GetInstance<ILevelLibrary>();
            _cellSettings = ServiceLocator.GetInstance<CellSettings>();
        }
        private void SetCell(int[,] map, Vector2Int position, Vector3 leftUpCell, Vector2 cellSize)
        private void SetCell(int[,] map, Vector2Int indexPosition, Vector3 leftUpCell, Vector2 cellSize)
        {
            int cellType = map[indexPosition.x, indexPosition.y];
            if (cellType == EMPTY_CELL)
            {
                return;
            }

            CellView cell = Instantiate(_cellSettings.CellPrefab);
            CellSettings.CellAnimation cellAnimation = _cellSettings.CellAnimations[cellType - 1];
            cell.SetAnimation(cellAnimation);
            SetCellSortingOrder(cell, indexPosition);
            cell.SetSize(cellSize);
            cell.SetPosition(leftUpCell.GetCenter(cellSize));
            cell.Init();
            cell.Swiped += OnCellSwiped;

            _cellMap[indexPosition.x, indexPosition.y] = cell;
        }
        private void SetCellSortingOrder(CellView cell, Vector2Int index)
        {
            cell.SetSortingOrder(index.y * _rows + (_rows - index.x));
        }
        private void OnCellSwiped(CellView cell, SwipeDirection swipeDirection)
        {
            if(_isAnimated)
            {
                return;
            }

            Vector2Int cellPosition = GetCellPosition(cell);
            if (cellPosition == _invalidPosition)
            {
                return;
            }

            Vector2Int nextPosition = GetNextPosition(cellPosition, swipeDirection);
            if (nextPosition == cellPosition)
            {
                return;
            }

            CellMoved?.Invoke(cellPosition, nextPosition);
        }
        private Vector2Int GetCellPosition(CellView cell)
        {
            for (var i = 0; i < _rows; i++)
            {
                for (var j = 0; j < _columns; j++)
                {
                    if (_cellMap[i, j] == cell)
                    {
                        return new Vector2Int(i, j);
                    }
                }
            }

            return _invalidPosition;
        }
        private Vector2Int GetNextPosition(Vector2Int currentPosition, SwipeDirection swipeDirection)
        {
            int x = currentPosition.x;
            int y = currentPosition.y;

            switch (swipeDirection)
            {
                case SwipeDirection.Up:
                    if (x > 0 && _cellMap[x - 1, y] != null)
                    {
                        return new Vector2Int(x - 1, y);
                    }
                    break;
                case SwipeDirection.Down:
                    if (x < _rows - 1)
                    {
                        return new Vector2Int(x + 1, y);
                    }
                    break;
                case SwipeDirection.Right:
                    if (y < _columns - 1)
                    {
                        return new Vector2Int(x, y + 1);
                    }
                    break;
                case SwipeDirection.Left:
                    if (y > 0)
                    {
                        return new Vector2Int(x, y - 1);
                    }
                    break;
            }

            return currentPosition;
        }
    }
}