using OctanGames.Gameplay.Levels;
using OctanGames.Infrastructure;
using OctanGames.Services;
using UnityEngine;

namespace OctanGames.Gameplay
{
    public class LevelLoader : MonoBehaviour, ILevelLoader
    {
        public const string SAVE_FILE_PATH = "Save.json";

        private ILevelLibrary _levelLibrary;
        private IDataService _dataService;

        public int CurrentLevel { get; private set; }

        private void Start()
        {
            _levelLibrary = ServiceLocator.GetInstance<ILevelLibrary>();
            _dataService = ServiceLocator.GetInstance<IDataService>();
        }

        public int[,] LoadCurrentLevel()
        {
            if (!_dataService.IsFileExist(SAVE_FILE_PATH))
            {
                return _levelLibrary.GetMapByNumber(CurrentLevel).Clone() as int[,];
            }

            var levelData = _dataService.LoadData<LevelData>(SAVE_FILE_PATH);
            _dataService.DeleteFile(SAVE_FILE_PATH);

            return levelData.Map;
        }

        public void SwitchNextLevel()
        {
            CurrentLevel++;
            if (CurrentLevel >= _levelLibrary.Count)
            {
                CurrentLevel = 0;
            }
            Debug.Log($"Next level {CurrentLevel}");
        }
    }
}