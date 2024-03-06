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
                _tableView.CellMoved -= _gridModel.MoveCell;
                _gridModel.CellPositionChanged -= _tableView.AnimateCellSwapping;
                _gridModel.CellsFell -= _tableView.AnimateCellFalling;
                _gridModel.CellsDestroyed -= _tableView.AnimateCellDestroying;
                _gridModel.LevelFinished -= _tableView.SwitchNextLevelAfterAnimation;
            }

            _gridModel = model;

            _tableView.CellMoved += _gridModel.MoveCell;
            _gridModel.CellPositionChanged += _tableView.AnimateCellSwapping;
            _gridModel.CellsFell += _tableView.AnimateCellFalling;
            _gridModel.CellsDestroyed += _tableView.AnimateCellDestroying;
            _gridModel.LevelFinished += _tableView.SwitchNextLevelAfterAnimation;
        }
    }
}