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
        public const int EMPTY_CELL = 0;

        public event Action<Vector2Int, Vector2Int> CellMoved;

        [Header("Properties")]
        [SerializeField] private float _tableWidth;
        [SerializeField] private float _tableHeight;
        [Header("Settings")]
        [SerializeField, Min(0.1f)] private float _movementDuration = 0.5f;
        [SerializeField, Min(0.1f)] private float _fallDuration = 0.5f;
        [SerializeField, Min(0.1f)] private float _destroyAnimationDuration = 1.5f;
        [SerializeField, Min(0.1f)] private float _idleAnimationDuration = 1.5f;

        private CellSettings _cellSettings;
        private LevelLoader _levelLoader;
        private GridController _gridController;

        private CellView[,] _cellMap;
        private Vector2 _cellSize;
        private Vector3 _finalLeftUpCorner;
        private int _rows;
        private int _columns;

        private bool _isAnimated;
        private Action _callBackAfterAnimation;

        private readonly Stack<Sequence> _callStackSequences = new();
        private readonly List<CellView> _allCells = new();
        private readonly Queue<List<Sequence>> _animationQueue = new();

        private readonly Vector2Int _invalidPosition = new(-1,-1);

        private void Start()
        {
            _cellSettings = ServiceLocator.GetInstance<CellSettings>();
            _levelLoader = ServiceLocator.GetInstance<LevelLoader>();
            _gridController = new GridController(this);

            DOTween.defaultAutoPlay = AutoPlay.None;

            InitNewLevel();
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            CornerTuple corner = transform.position.GetCornersFromCenter(_tableHeight, _tableWidth);
            GizmosWrapper.DrawGizmosRect(
                corner.LeftUpCorner,
                corner.RightUpCorner,
                corner.LeftDownCorner,
                corner.RightDownCorner);
        }

        public void AnimateCellSwapping(Vector2Int startPosition, Vector2Int endPosition)
        {
            var animationList = new List<Sequence>();

            CellView firstCell = GetCellByIndex(startPosition);
            Sequence sequence = GetCellMovementSequence(firstCell, endPosition, _movementDuration);
            animationList.Add(sequence);

            CellView secondCell = GetCellByIndex(endPosition);
            if (secondCell != null)
            {
                sequence = GetCellMovementSequence(secondCell, startPosition, _movementDuration);
                animationList.Add(sequence);
            }

            if (animationList.Count > 0)
            {
                AddAnimation(animationList);
            }

            SetCellByIndex(startPosition, secondCell);
            SetCellByIndex(endPosition, firstCell);
        }
        public void AnimateCellFalling(List<FallData> cellModelFalls)
        {
            var animationList = new List<Sequence>();
            foreach (FallData modelFall in cellModelFalls)
            {
                CellView cell = _cellMap[modelFall.StartIndex.x, modelFall.StartIndex.y];

                Vector2Int targetIndex = modelFall.StartIndex;
                targetIndex.x += modelFall.FallStep;

                float duration = _fallDuration * modelFall.FallStep;
                Sequence sequence = GetCellMovementSequence(cell, targetIndex, duration);
                animationList.Add(sequence);

                SetCellByIndex(targetIndex, cell);
                SetCellByIndex(modelFall.StartIndex, null);
            }

            if (animationList.Count > 0)
            {
                AddAnimation(animationList);
            }
        }
        public void AnimateCellDestroying(List<Vector2Int> indexes)
        {
            var animationList = new List<Sequence>();
            foreach (Vector2Int index in indexes)
            {
                CellView cell = GetCellByIndex(index);

                Sequence sequence = DOTween.Sequence()
                    .AppendCallback(() =>
                    {
                        if (cell != null)
                        {
                            cell.AnimateDestruction();
                        }
                    })
                    .AppendInterval(_destroyAnimationDuration)
                    .SetLink(cell.gameObject);

                animationList.Add(sequence);

                SetCellByIndex(index, null);
            }

            if (animationList.Count > 0)
            {
                AddAnimation(animationList);
            }
        }

        public void SwitchNextLevel()
        {
            _levelLoader.SwitchNextLevel();
            DestroyTable();
            InitNewLevel();
        }
        public void SwitchNextLevelAfterAnimation()
        {
            _levelLoader.SwitchNextLevel();
            _callBackAfterAnimation = InitNewLevel;
        }
        private void InitNewLevel()
        {
            int[,] map = _levelLoader.LoadCurrentLevel();
            _rows = map.GetLength(0);
            _columns = map.GetLength(1);

            CornerTuple tableCorners = transform.position.GetCornersFromCenter(_tableHeight, _tableWidth);

            GenerateTable(map, tableCorners);
            _gridController.Init(new GridModel(map, _rows, _columns));
            _callBackAfterAnimation = null;

            Debug.Log("Init new level");
        }
        private void DestroyTable()
        {
            while (_callStackSequences.Count > 0)
            {
                Sequence sequence = _callStackSequences.Pop();
                sequence.Kill();
            }

            _animationQueue.Clear();

            foreach (CellView cell in _allCells)
            {
                if (cell != null)
                {
                    cell.Destroy();
                }
            }
            _allCells.Clear();

            _isAnimated = false;
        }

        private Sequence GetCellMovementSequence(CellView cell, Vector2Int targetIndex, float duration)
        {
            Vector3 newPosition = IndexToPosition(targetIndex);

            Sequence sequence = DOTween.Sequence()
                .AppendCallback(() => SetCellSortingOrder(cell, targetIndex))
                .Append(DOTween.To(() => cell.transform.position, cell.SetPosition, newPosition, duration))
                .SetLink(cell.gameObject);

            return sequence;
        }
        private void AddAnimation(List<Sequence> listAnimations)
        {
            foreach (Sequence sequence in listAnimations)
            {
                sequence.Pause();
            }
            _animationQueue.Enqueue(listAnimations);

            if (!_isAnimated)
            {
                PlayAnimations();
            }
        }
        private void PlayAnimations()
        {
            if (_animationQueue.Count > 0)
            {
                _isAnimated = true;
                List<Sequence> list = _animationQueue.Dequeue();

                for (var i = 0; i < list.Count; i++)
                {
                    Sequence sequence = list[i];
                    _callStackSequences.Push(sequence);

                    if (i == 0)
                    {
                        sequence.AppendCallback(PlayAnimations);
                    }

                    sequence.Play();
                }
            }
            else
            {
                _isAnimated = false;
                _callStackSequences.Clear();
                _callBackAfterAnimation?.Invoke();
            }
        }

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
                    var leftUpCellCorner = new Vector3(
                        _finalLeftUpCorner.x + _cellSize.x * j,
                        _finalLeftUpCorner.y - _cellSize.y * i);

                    Vector3 cellPosition = leftUpCellCorner.GetCenter(_cellSize);

                    SetupCell(map, indexPosition, cellPosition, _cellSize);
                }
            }
        }
        private void SetupCell(int[,] map, Vector2Int index, Vector3 cellPosition, Vector2 cellSize)
        {
            int cellType = map[index.x, index.y];
            if (cellType == EMPTY_CELL)
            {
                return;
            }

            CellView cell = Instantiate(_cellSettings.CellPrefab);
            CellSettings.CellAnimation cellAnimation = _cellSettings.CellAnimations[cellType - 1];
            cell.SetAnimations(cellAnimation);
            SetCellSortingOrder(cell, index);
            cell.SetSize(cellSize);
            cell.SetPosition(cellPosition);
            cell.Init(_idleAnimationDuration, _destroyAnimationDuration);
            cell.Swiped += OnCellSwiped;

            _cellMap[index.x, index.y] = cell;
            _allCells.Add(cell);
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

            Vector2Int cellPosition = GetCellIndex(cell);
            if (cellPosition == _invalidPosition)
            {
                return;
            }

            Vector2Int nextPosition = GetNextIndex(cellPosition, swipeDirection);
            if (nextPosition == cellPosition)
            {
                return;
            }

            CellMoved?.Invoke(cellPosition, nextPosition);
        }
        private Vector2Int GetNextIndex(Vector2Int currentPosition, SwipeDirection swipeDirection)
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

        private Vector3 IndexToPosition(Vector2Int index)
        {
            var leftUpCellCorner = new Vector3(
                _finalLeftUpCorner.x + _cellSize.x * index.y,
                _finalLeftUpCorner.y - _cellSize.y * index.x);

            return leftUpCellCorner.GetCenter(_cellSize);
        }
        private Vector2Int GetCellIndex(CellView cell)
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
        private CellView GetCellByIndex(Vector2Int index)
        {
            return _cellMap[index.x, index.y];
        }
        private void SetCellByIndex(Vector2Int index, CellView cell)
        {
            _cellMap[index.x, index.y] = cell;
        }
    }
}