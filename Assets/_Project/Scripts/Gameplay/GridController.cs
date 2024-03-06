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

            _appActiveHandler.ApplicationClosed += OnApplicationClosed;
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
            _levelLoader.SwitchNextLevel();
            _tableView.AnimationEnded += OnTableAnimationEnded;
        }
        private void OnTableAnimationEnded()
        {
            _tableView.AnimationEnded -= OnTableAnimationEnded;
            _levelIsFinished = false;
            _tableView.InitNewLevel();
        }

        private void OnApplicationClosed()
        {
            SaveLevel();
        }
        private void SaveLevel()
        {
            _appActiveHandler.ApplicationClosed -= OnApplicationClosed;

            var levelData = new LevelData()
            {
                Map = _gridModel.GetMapScreenshot(),
                CurrentLevel = _levelLoader.CurrentLevel
            };

            _dataService.SaveData(LevelLoader.SAVE_FILE_PATH, levelData);
        }
    }
}