using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OctanGames.Inputs
{
    public class SwipeDetector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event Action<SwipeData> Swiped;

        [SerializeField] private float _minSwipeDistance = 20f;

        private Vector2 _touchUpPosition;
        private Vector2 _touchDownPosition;

        public void OnPointerDown(PointerEventData eventData)
        {
            _touchUpPosition = eventData.position;
            _touchDownPosition = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _touchDownPosition = eventData.position;
            DetectSwipe();
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
                    : SwipeDirection.Left;
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