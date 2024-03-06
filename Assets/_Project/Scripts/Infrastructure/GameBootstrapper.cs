using System;
using OctanGames.Gameplay;
using OctanGames.Gameplay.Levels;
using OctanGames.Services;
using UnityEngine;

namespace OctanGames.Infrastructure
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private CellSettings _cellSettings;
        [SerializeField] private LevelLibraryLibrary _levels;
        [SerializeField] private TableView _tableView;
        [SerializeField] private LevelLoader _levelLoader;
        [SerializeField] private AppActiveHandler _appActiveHandler;

        private GridController _gridController;

        private void Awake()
        {
            ServiceLocator.Bind(_cellSettings);
            ServiceLocator.Bind<ILevelLibrary>(_levels);
            ServiceLocator.Bind(_tableView);
            ServiceLocator.Bind<ILevelLoader>(_levelLoader);
            ServiceLocator.Bind<IDataService>(new JsonDataService());
            ServiceLocator.Bind<IAppActiveHandler>(_appActiveHandler);

            _gridController = new GridController(_tableView);
            ServiceLocator.Bind(_gridController);
        }

        private void OnDestroy()
        {
            _gridController.Dispose();
        }
    }
}