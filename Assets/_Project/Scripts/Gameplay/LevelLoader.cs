using OctanGames.Gameplay.Levels;
using OctanGames.Infrastructure;
using UnityEngine;

namespace OctanGames.Gameplay
{
    public class LevelLoader : MonoBehaviour
    {
        private ILevelLibrary _levelLibrary;

        private int _currentLevel;

        private void Start()
        {
            _levelLibrary = ServiceLocator.GetInstance<ILevelLibrary>();
        }

        public int[,] LoadCurrentLevel()
        {
            return _levelLibrary.GetMapByNumber(_currentLevel).Clone() as int[,];
        }

        public void SwitchNextLevel()
        {
            _currentLevel++;
            if (_currentLevel >= _levelLibrary.Count)
            {
                _currentLevel = 0;
            }
            Debug.Log($"Next level {_currentLevel}");
        }
    }
}