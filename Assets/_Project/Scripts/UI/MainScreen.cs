using OctanGames.Gameplay;
using OctanGames.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace OctanGames.UI
{
    public class MainScreen : MonoBehaviour
    {
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _nextLevelButton;

        private GridController _gridController;

        private void Start()
        {
            _gridController = ServiceLocator.GetInstance<GridController>();

            _restartButton.onClick.AddListener(OnRestartButtonClicked);
            _nextLevelButton.onClick.AddListener(OnNextLevelButtonClicked);
        }

        private void OnDestroy()
        {
            _restartButton.onClick.RemoveListener(OnRestartButtonClicked);
            _nextLevelButton.onClick.RemoveListener(OnNextLevelButtonClicked);

        }

        private void OnRestartButtonClicked()
        {
            _gridController.RestartLevel();
        }

        private void OnNextLevelButtonClicked()
        {
            _gridController.SwitchNextLevel();
        }
    }
}