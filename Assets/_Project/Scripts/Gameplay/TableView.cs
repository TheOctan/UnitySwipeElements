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

        [SerializeField] private float _tableWidth;
        [SerializeField] private float _tableHeight;

        private CellSettings _cellSettings;
        private LevelLoader _levelLoader;
        private CellView[,] _cellMap;
        private Vector2 _cellSize;
        private Vector3 _finalLeftUpCorner;
        private int _rows;
        private int _columns;

        private bool _isAnimated;

        private readonly Vector2Int _invalidPosition = new(-1,-1);

        private void Start()
        {
            _cellSettings = ServiceLocator.GetInstance<CellSettings>();
            _levelLoader = ServiceLocator.GetInstance<LevelLoader>();
        private void GenerateTable(int[,] map, CornerTuple tableCorners)
        {
            _cellMap = new CellView[_rows, _columns];

            float aspectRatioCell = _cellSettings.CellPrefab.AspectRatio;

            float potentialCellWidth = _tableWidth / _columns;
            float potentialCellHeight = _tableHeight / _rows;

            float minSide = Mathf.Min(potentialCellWidth / aspectRatioCell, potentialCellHeight);
            _cellSize = new Vector2(minSide * aspectRatioCell, minSide );

            float finalTableHeight = _cellSize.y * _rows;
            float finalTableWidth = _cellSize.x * _columns;

            float halfTableWidth = (tableCorners.RightDownCorner.x - tableCorners.LeftDownCorner.x) / 2f;
            float horizontalOffset = halfTableWidth - finalTableWidth / 2;

            _finalLeftUpCorner = new Vector3(
                tableCorners.LeftDownCorner.x + horizontalOffset,
                tableCorners.LeftDownCorner.y + finalTableHeight);

            for (int i = _rows - 1; i >= 0; i--)
            {
                for (var j = 0; j < _columns; j++)
                {
                    var indexPosition = new Vector2Int(i, j);
                    var leftUpCellPosition = new Vector3(
                        _finalLeftUpCorner.x + _cellSize.x * j,
                        _finalLeftUpCorner.y - _cellSize.y * i);

                    Vector3 cellPosition = leftUpCellPosition.GetCenter(_cellSize);

                    SetupCell(map, indexPosition, cellPosition, _cellSize);
                }
            }
        }
        private void SetupCell(int[,] map, Vector2Int indexPosition, Vector3 cellPosition, Vector2 cellSize)
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
            cell.SetPosition(cellPosition);
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