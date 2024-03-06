using System;

namespace OctanGames.Services
{
    public interface IAppActiveHandler
    {
        event Action ApplicationPaused;
        event Action ApplicationResumed;
    }
}