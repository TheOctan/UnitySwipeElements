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
            _tableView.SwitchNextLevel();
        }
        public void RestartLevel()
        {
            _tableView.RestartLevel();
        }

        private void SubscribeToModel()
        {
            _tableView.CellMoved += _gridModel.MoveCell;
            _gridModel.CellPositionChanged += _tableView.AnimateCellSwapping;
            _gridModel.CellsFell += _tableView.AnimateCellFalling;
            _gridModel.CellsDestroyed += _tableView.AnimateCellDestroying;
            _gridModel.LevelFinished += _tableView.SwitchNextLevelAfterAnimation;
        }
        private void UnsubscribeFromModel()
        {
            _tableView.CellMoved -= _gridModel.MoveCell;
            _gridModel.CellPositionChanged -= _tableView.AnimateCellSwapping;
            _gridModel.CellsFell -= _tableView.AnimateCellFalling;
            _gridModel.CellsDestroyed -= _tableView.AnimateCellDestroying;
            _gridModel.LevelFinished -= _tableView.SwitchNextLevelAfterAnimation;
        }

        private void OnApplicationClosed()
        {
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
            _appActiveHandler.ApplicationClosed -= OnApplicationClosed;
        }
    }
}