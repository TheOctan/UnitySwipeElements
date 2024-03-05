using OctanGames.Gameplay;
using OctanGames.Gameplay.Levels;
using UnityEngine;

namespace OctanGames.Infrastructure
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private CellSettings _cellSettings;
        [SerializeField] private LevelLibraryLibrary _levels;
        [SerializeField] private TableView _tableView;

        private void Awake()
        {
            ServiceLocator.Bind(_cellSettings);
            ServiceLocator.Bind<ILevelLibrary>(_levels);
            ServiceLocator.Bind(_tableView);
        }
    }
}