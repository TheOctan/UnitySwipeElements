using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Extensions;
using OctanGames.Extensions;
using OctanGames.Gameplay.Levels;
using OctanGames.Inputs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OctanGames.Gameplay
{
    public class CellView : MonoBehaviour
    {
        private const int ENDLESS_LOOP = -1;
        private const float MIN_ANIMATION_DELAY = 1f;
        private const float MAX_ANIMATION_DELAY = 3f;

        public event Action<CellView, SwipeDirection> Swiped;

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
        private float _destroyAnimationDuration;
        private float _idleAnimationDuration;

        public float AspectRatio => Width / Height;
        private float Width => _right + _left;
        private float Height => _up + _down;
        private int DestroyAnimationFrame
        {
            get => _destroyAnimationFrame;
            set
            {
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
        private void OnDestroy()
        {
            _swipeDetector.Swiped -= OnSwiped;
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Vector3 lossyScale = transform.lossyScale;

            CornerTuple corner = transform.position.GetCornersFromSides(
                _up * lossyScale.y,
                _down * lossyScale.y,
                _left * lossyScale.x,
                _right * lossyScale.x);

            GizmosWrapper.DrawGizmosRect(
                corner.LeftUpCorner,
                corner.RightUpCorner,
                corner.LeftDownCorner,
                corner.RightDownCorner);
        }

        public void Init(float idleDuration, float destroyDuration)
        {
            _idleAnimationDuration = idleDuration;
            _destroyAnimationDuration = destroyDuration;

            AnimateIdle();
        }
        public void SetSize(Vector2 size)
        {
            Transform parent = transform.parent;
            transform.parent = null;

            transform.localScale = new Vector3(
                size.x / (Width * transform.localScale.x),
                size.y / (Height * transform.localScale.y),
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
        public void SetAnimations(CellSettings.CellAnimation cellAnimation)
        {
            _idleAnimation = cellAnimation.IdleAnimation;
            _destroyAnimation = cellAnimation.DestroyAnimation;

            _idleAnimationFrame = 0;
            _destroyAnimationFrame = 0;

            SetSprite(cellAnimation.Sprite);
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
        public void AnimateDestruction()
        {
            _animationLoop?.Kill();

            TweenerCore<int,int,NoOptions> animationSequence = DOTween.To(
                () => DestroyAnimationFrame, 
                x => DestroyAnimationFrame = x, 
                _destroyAnimation.Length - 1,
                _destroyAnimationDuration);

            _animationLoop = DOTween.Sequence()
                .Append(animationSequence)
                .SetEase(Ease.Linear)
                .AppendCallback(Destroy);
        }

        private void AnimateIdle()
        {
            float delay = Random.Range(MIN_ANIMATION_DELAY, MAX_ANIMATION_DELAY);

            _animationLoop?.Kill();

            TweenerCore<int, int, NoOptions> animationSequence = DOTween.To(
                () => IdleAnimationFrame, 
                x => IdleAnimationFrame = x, 
                _idleAnimation.Length - 1,
                _idleAnimationDuration);

            _animationLoop = DOTween.Sequence()
                .Append(animationSequence)
                .SetEase(Ease.Linear)
                .AppendInterval(delay)
                .AppendCallback(() =>
                {
                    IdleAnimationFrame = 0;
                    AnimateIdle();
                })
                .SetLoops(ENDLESS_LOOP, LoopType.Restart);
        }
        private void OnSwiped(SwipeData swipeData)
        {
            Swiped?.Invoke(this, swipeData.Direction);
        }
        private void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }
    }
}