using System.Collections.Generic;
using OctanGames.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OctanGames.UI.Background
{
    public class BalloonSpawner : MonoBehaviour
    {
        [Header("Properties")]
        [SerializeField, Min(0)] private int _maxBalloons = 3;
        [Space]
        [SerializeField] private Vector2 _horizontalSpeedRange;
        [SerializeField] private Vector2 _verticalSpeedRange;
        [SerializeField] private Vector2 _amplitudeRange;
        [Header("Resources")]
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private List<Balloon> _balloonPrefabs;

        private List<Balloon> _spawnedBalloons = new();

        private void Start()
        {
            SpawnBalloons();
        }

        private void SpawnBalloons()
        {
            if (_balloonPrefabs.Count <= 0)
            {
                return;
            }

            for (var i = 0; i < _maxBalloons; i++)
            {
                int randomIndex = Random.Range(0, _balloonPrefabs.Count);

                Balloon balloonPrefab = _balloonPrefabs[randomIndex];
                Balloon balloon = Instantiate(balloonPrefab, transform);

                Bounds cameraBounds = _mainCamera.OrthographicBounds();
                balloon.Setup(cameraBounds, _horizontalSpeedRange, _verticalSpeedRange, _amplitudeRange);
            }
        }
    }
}