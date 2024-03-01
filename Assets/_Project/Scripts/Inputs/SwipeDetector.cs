using System;
using UnityEngine;

namespace OctanGames.Inputs
{
    public class SwipeDetector : MonoBehaviour
    {
        public event Action<SwipeData> Swiped;

        [SerializeField] private float _minSwipeDistance = 20f;

        private Vector2 _touchUpPosition;
        private Vector2 _touchDownPosition;

        private void Update()
        {
            if (Input.touches.Length <= 0)
            {
                return;
            }

            Touch touch = Input.touches[0];

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _touchUpPosition = touch.position;
                    _touchDownPosition = touch.position;
                    break;
                case TouchPhase.Ended:
                    _touchDownPosition = touch.position;
                    DetectSwipe();
                    break;
            }
        }

        private void DetectSwipe()
        {
            bool isSwiped = (_touchDownPosition - _touchUpPosition).sqrMagnitude
                            > _minSwipeDistance * _minSwipeDistance;

            if (!isSwiped)
            {
                return;
            }

            SwipeDirection direction = GetSwipeDirection();
            InvokeSwipe(direction);

            Debug.Log(direction);

            _touchUpPosition = _touchDownPosition;
        }

        private SwipeDirection GetSwipeDirection()
        {
            bool isVerticalSwipe = IsVerticalSwipe();
            SwipeDirection direction;
            if (isVerticalSwipe)
            {
                direction = _touchDownPosition.y - _touchUpPosition.y > 0
                    ? SwipeDirection.Up
                    : SwipeDirection.Down;
            }
            else
            {
                direction = _touchDownPosition.x - _touchUpPosition.x > 0
                    ? SwipeDirection.Right
                    : SwipeDirection.Down;
            }

            return direction;
        }

        private bool IsVerticalSwipe()
        {
            float verticalDistance = Mathf.Abs(_touchDownPosition.y - _touchUpPosition.y);
            float horizontalDistance = Mathf.Abs(_touchDownPosition.x - _touchUpPosition.x);

            return verticalDistance > horizontalDistance;
        }

        private void InvokeSwipe(SwipeDirection direction)
        {
            Swiped?.Invoke(new SwipeData()
            {
                Direction = direction,
                StartPosition = _touchDownPosition,
                EndPosition = _touchUpPosition
            });
        }
    }
}