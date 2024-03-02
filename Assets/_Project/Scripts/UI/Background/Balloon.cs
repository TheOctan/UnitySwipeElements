using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OctanGames.UI.Background
{
    public class Balloon : MonoBehaviour
    {
        public enum Side
        {
            Left = 0,
            Right
        }

        [SerializeField] private float _speed;
        [SerializeField] private float _amplitude;
        [SerializeField] private float _heightOffset;

        [SerializeField] private SpriteRenderer _spriteRenderer;

        private Bounds _bounds;
        private Vector3 _spriteBoundsExtents;
        private Side _destinationSide;
        private float _leftBorder;
        private float _rightBorder;

        public void Setup(Bounds bounds)
        {
            _bounds = bounds;
            _spriteBoundsExtents = _spriteRenderer.bounds.extents;
            _leftBorder = _bounds.min.x - _spriteBoundsExtents.x;
            _rightBorder = _bounds.max.x + _spriteBoundsExtents.x;

            GenerateTrajectory();
        }

        private void Update()
        {
            Vector3 deltaPosition = Time.deltaTime * _speed * Vector3.right;

            switch (_destinationSide)
            {
                case Side.Left:
                    transform.position -= deltaPosition;
                    if (transform.position.x <= _leftBorder)
                    {
                        GenerateTrajectory();
                    }
                    break;
                case Side.Right:
                    transform.position += deltaPosition;
                    if (transform.position.x >= _rightBorder)
                    {
                        GenerateTrajectory();
                    }
                    break;
            }

            //float y = Mathf.Sin(Time.time * _speed) * _amplitude;
        }

        private void GenerateTrajectory()
        {
            float randomYPosition = Random.Range(
                _bounds.min.y + _spriteBoundsExtents.y,
                _bounds.max.y - _spriteBoundsExtents.y);

            _destinationSide = GetRandomSide();

            float randomXPosition = _destinationSide == Side.Left ? _rightBorder : _leftBorder;
            transform.position = new Vector3(randomXPosition, randomYPosition, 0);
        }

        private static Side GetRandomSide()
        {
            return (Side)Random.Range((int)Side.Left, (int)Side.Right + 1);
        }
    }
}