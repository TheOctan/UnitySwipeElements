namespace OctanGames.Gameplay
{
    public interface ILevelLoader
    {
        int CurrentLevel { get; }
        int[,] LoadCurrentLevel();
        void SaveLevel(int[,] map);
        void DeleteSavedLevel();
        void SwitchNextLevel();
    }
}