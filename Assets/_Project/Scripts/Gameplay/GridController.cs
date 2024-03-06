using OctanGames.Gameplay.Levels;
using OctanGames.Infrastructure;
using OctanGames.Services;

namespace OctanGames.Gameplay
{
    public class GridController
    {
        private readonly IAppActiveHandler _appActiveHandler;
        private readonly IDataService _dataService;
        private readonly ILevelLoader _levelLoader;
        private readonly TableView _tableView;
        private GridModel _gridModel;

        private bool _levelIsFinished;

        public GridController(TableView tableView)
        {
            _tableView = tableView;
            _dataService = ServiceLocator.GetInstance<IDataService>();
            _appActiveHandler = ServiceLocator.GetInstance<IAppActiveHandler>();
            _levelLoader = ServiceLocator.GetInstance<ILevelLoader>();

#if UNITY_EDITOR
            _appActiveHandler.ApplicationClosed += OnApplicationClosed;
#else
            _appActiveHandler.ApplicationPaused += OnApplicationPaused;
#endif
        }

        public void Init(GridModel model)
        {
            if (_gridModel != null)
            {
                UnsubscribeFromModel();
            }

            _gridModel = model;
            SubscribeToModel();
        }
        public void Dispose()
        {
            UnsubscribeFromModel();
        }

        public void SwitchNextLevel()
        {
            if (_levelIsFinished)
            {
                return;
            }

            _levelLoader.SwitchNextLevel();
            _tableView.DestroyTable();
            _tableView.InitNewLevel();
        }
        public void RestartLevel()
        {
            if (_levelIsFinished)
            {
                return;
            }

            _tableView.DestroyTable();
            _tableView.InitNewLevel();
        }

        private void SubscribeToModel()
        {
            _tableView.CellMoved += _gridModel.MoveCell;
            _gridModel.CellPositionChanged += _tableView.AnimateCellSwapping;
            _gridModel.CellsFell += _tableView.AnimateCellFalling;
            _gridModel.CellsDestroyed += _tableView.AnimateCellDestroying;
            _gridModel.LevelFinished += OnLevelFinished;
        }
        private void UnsubscribeFromModel()
        {
            _tableView.CellMoved -= _gridModel.MoveCell;
            _gridModel.CellPositionChanged -= _tableView.AnimateCellSwapping;
            _gridModel.CellsFell -= _tableView.AnimateCellFalling;
            _gridModel.CellsDestroyed -= _tableView.AnimateCellDestroying;
            _gridModel.LevelFinished -= OnLevelFinished;
        }

        private void OnLevelFinished()
        {
            _levelIsFinished = true;
            _tableView.AnimationEnded += OnTableAnimationEnded;
        }
        private void OnTableAnimationEnded()
        {
            _tableView.AnimationEnded -= OnTableAnimationEnded;
            _levelIsFinished = false;

            _levelLoader.SwitchNextLevel();
            _tableView.InitNewLevel();
        }

        private void OnApplicationClosed()
        {
            _appActiveHandler.ApplicationClosed -= OnApplicationClosed;

            SaveLevel();
        }
        private void OnApplicationPaused()
        {
            _appActiveHandler.ApplicationPaused -= OnApplicationPaused;

            SaveLevel();
        }

        private void SaveLevel()
        {
            var levelData = new LevelData()
            {
                Map = _gridModel.GetMapScreenshot(),
                CurrentLevel = _levelLoader.CurrentLevel
            };

            _dataService.SaveData(LevelLoader.SAVE_FILE_PATH, levelData);
        }
    }
}