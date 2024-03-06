namespace OctanGames.Gameplay
{
    public interface ILevelLoader
    {
        int CurrentLevel { get; }
        int[,] LoadCurrentLevel();
        void SwitchNextLevel();
    }
}