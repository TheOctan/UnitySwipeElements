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

        [SerializeField] private SpriteRenderer _spriteRenderer;

        private Bounds _bounds;
        private Vector3 _spriteBoundsExtents;
        private Vector3 _startPosition;

        private Vector2 _horizontalSpeedRange;
        private Vector2 _verticalSpeedRange;
        private Vector2 _amplitudeRange;
        
        private float _horizontalSpeed;
        private float _verticalSpeed;
        private float _amplitude;

        private Side _destinationSide;
        private float _leftBorder;
        private float _rightBorder;

        public void Setup(Bounds bounds,
            Vector2 horizontalSpeedRange,
            Vector2 verticalSpeedRange,
            Vector2 amplitudeRange)
        {
            _bounds = bounds;
            _amplitudeRange = amplitudeRange;
            _verticalSpeedRange = verticalSpeedRange;
            _horizontalSpeedRange = horizontalSpeedRange;

            _spriteBoundsExtents = _spriteRenderer.bounds.extents;
            _leftBorder = _bounds.min.x - _spriteBoundsExtents.x;
            _rightBorder = _bounds.max.x + _spriteBoundsExtents.x;

            GenerateTrajectory();
        }

        private void Update()
        {
            float deltaPosition = Time.deltaTime * _horizontalSpeed;
            float yPosition = Mathf.Sin(Time.time * _verticalSpeed) * _amplitude;

            switch (_destinationSide)
            {
                case Side.Left:
                    transform.position = new Vector3(transform.position.x - deltaPosition,
                        _startPosition.y + yPosition, 0);

                    if (transform.position.x <= _leftBorder)
                    {
                        GenerateTrajectory();
                    }
                    break;
                case Side.Right:
                    transform.position = new Vector3(transform.position.x + deltaPosition,
                        _startPosition.y + yPosition, 0);

                    if (transform.position.x >= _rightBorder)
                    {
                        GenerateTrajectory();
                    }
                    break;
            }
        }

        private void GenerateTrajectory()
        {
            float randomYPosition = Random.Range(
                _bounds.min.y + _spriteBoundsExtents.y,
                _bounds.max.y - _spriteBoundsExtents.y);

            _destinationSide = GetRandomSide();
            float randomXPosition = _destinationSide == Side.Left ? _rightBorder : _leftBorder;

            _startPosition = new Vector3(randomXPosition, randomYPosition, 0);
            transform.position = _startPosition;

            _horizontalSpeed = Random.Range(_horizontalSpeedRange.x, _horizontalSpeedRange.y);
            _verticalSpeed = Random.Range(_verticalSpeedRange.x, _verticalSpeedRange.y);
            _amplitude = Random.Range(_amplitudeRange.x, _amplitudeRange.y);
        }

        private static Side GetRandomSide()
        {
            return (Side)Random.Range((int)Side.Left, (int)Side.Right + 1);
        }
    }
}