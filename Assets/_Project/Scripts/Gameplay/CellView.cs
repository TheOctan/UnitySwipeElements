﻿using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Extensions;
using OctanGames.Extensions;
using OctanGames.Inputs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OctanGames.Gameplay
{
    public class CellView : MonoBehaviour
    {
        private const float MIN_ANIMATION_DELAY = 1f;
        private const float MAX_ANIMATION_DELAY = 3f;
        private const float IDLE_ANIMATION_DURATION = 1.5f;
        private const int ENDLESS_LOOP = -1;

        public event Action<CellView, SwipeDirection> Moved;

        [Header("Properties")]
        [SerializeField] private float _up;
        [SerializeField] private float _down;
        [SerializeField] private float _left;
        [SerializeField] private float _right;
        [Header("Components")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private SwipeDetector _swipeDetector;

        private Sequence _animationLoop;
        private Sprite[] _idleAnimation;
        private Sprite[] _destroyAnimation;
        private int _idleAnimationFrame;
        private int _destroyAnimationFrame;

        public float AspectRatio => Width / Height;
        public int BoomIndexMax => _destroyAnimation.Length - 1;
        private float Width => _right + _left;
        private float Height => _up + _down;
        public int DestroyAnimationFrame
        {
            get => _destroyAnimationFrame;
            set
            {
                if (_animationLoop != null)
                {
                    _animationLoop.Kill();
                    _animationLoop = null;
                }

                _destroyAnimationFrame = value;
                SetSprite(_destroyAnimation[value]);
            }
        }
        private int IdleAnimationFrame
        {
            get => _idleAnimationFrame;
            set
            {
                _idleAnimationFrame = value;
                SetSprite(_idleAnimation[value]);
            }
        }

        private void Start()
        {
            _swipeDetector.Swiped += OnSwiped;
        }

        private void OnSwiped(SwipeData swipeData)
        {
            Moved?.Invoke(this, swipeData.Direction);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Vector3 lossyScale = transform.lossyScale;

            var corner = transform.position.GetCornersFromSides(
                _up * lossyScale.y,
                _down * lossyScale.y,
                _left * lossyScale.x,
                _right * lossyScale.x);

            GizmosWrapper.DrawGizmosRect(
                corner.leftUpCorner,
                corner.rightUpCorner,
                corner.leftDownCorner,
                corner.rightDownCorner);
        }

        public void Init()
        {
            Animate();
        }

        private void Animate()
        {
            float delay = Random.Range(MIN_ANIMATION_DELAY, MAX_ANIMATION_DELAY);

            _animationLoop?.Kill();

            TweenerCore<int, int, NoOptions> animationSequence = DOTween.To(
                () => IdleAnimationFrame, 
                x => IdleAnimationFrame = x, 
                _idleAnimation.Length - 1,
                IDLE_ANIMATION_DURATION);

            _animationLoop = DOTween.Sequence()
                .Append(animationSequence)
                .SetEase(Ease.Linear)
                .AppendInterval(delay)
                .AppendCallback(() =>
                {
                    IdleAnimationFrame = 0;
                    Animate();
                })
                .SetLoops(ENDLESS_LOOP, LoopType.Restart);
        }

        public void SetSize(float width, float height)
        {
            Transform parent = transform.parent;
            transform.parent = null;

            transform.localScale = new Vector3(
                width / (Width * transform.lossyScale.x),
                height / (Height * transform.localScale.y),
                transform.localScale.z);

            transform.parent = parent;
        }

        public void SetPosition(Vector3 position)
        {
            Vector3 localScale = transform.localScale;

            position.x -= (_up - _down) / 2f * localScale.x;
            position.y -= (_right - _left) / 2f * localScale.y;

            transform.position = position;
        }

        public void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }

        public void SetAnimation(Sprite[] idleAnimation, Sprite[] destroyAnimation)
        {
            _idleAnimation = idleAnimation;
            _destroyAnimation = destroyAnimation;

            _idleAnimationFrame = 0;
            _destroyAnimationFrame = 0;
        }

        public void SetSortingOrder(int order)
        {
            _spriteRenderer.sortingOrder = order;
        }

        public void Destroy()
        {
            _animationLoop?.Kill();
            Destroy(gameObject);
        }
    }
}