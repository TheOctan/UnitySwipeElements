using System;
using UnityEngine;

namespace OctanGames.UI.Background
{
    public class Balloon : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _amplitude;

        private Bounds _bounds;
        private int _currentDirection = 1;

        public void Setup(Bounds bounds)
        {
            _bounds = bounds;
        }

        private void Update()
        {
            float y = Mathf.Sin(Time.time * _speed) * _amplitude;
        }
    }
}