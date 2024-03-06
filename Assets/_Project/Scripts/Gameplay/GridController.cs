namespace OctanGames.Gameplay
{
    public class GridController
    {
        private readonly TableView _tableView;
        private GridModel _gridModel;

        public GridController(TableView tableView)
        {
            _tableView = tableView;
        }

        public void Init(GridModel model)
        {
            if (_gridModel != null)
            {
                _gridModel.LevelFinished -= _tableView.SwitchNextLevelAfterAnimation;
            }

            _gridModel = model;

            _gridModel.LevelFinished += _tableView.SwitchNextLevelAfterAnimation;
        }
    }
}