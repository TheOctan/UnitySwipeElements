namespace OctanGames.Gameplay.Levels
{
    public interface ILevelLibrary
    {
        int[,] GetMapByNumber(int index);
        int Count { get; }
    }
}