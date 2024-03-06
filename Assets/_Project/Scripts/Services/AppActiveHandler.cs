using System;
using UnityEngine;

namespace OctanGames.Services
{
    public class AppActiveHandler : MonoBehaviour, IAppActiveHandler
    {
        public event Action ApplicationPaused;
        public event Action ApplicationResumed;
        public event Action ApplicationClosed;

        private bool _isPaused;

        private void OnApplicationFocus(bool hasFocus)
        {
            CheckPauseStatus(!hasFocus);
        }
        private void OnApplicationPause(bool pauseStatus)
        {
            CheckPauseStatus(pauseStatus);
        }

        private void OnApplicationQuit()
        {
            Debug.Log("Application closed");
            ApplicationClosed?.Invoke();
        }

        private void CheckPauseStatus(bool pauseStatus)
        {
            if (_isPaused == pauseStatus) return;
            NotifyAppStatus(pauseStatus);
        }

        private void NotifyAppStatus(bool status)
        {
            Debug.Log($"{nameof(NotifyAppStatus)}, application pause status = {_isPaused}");

            _isPaused = status;
            if (_isPaused)
            {
                ApplicationPaused?.Invoke();
            }
            else
            {
                ApplicationResumed?.Invoke();
            }
        }
    }
}