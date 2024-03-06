namespace OctanGames.Services
{
    public interface IDataService
    {
        bool SaveData<T>(string relativePath, T data);
        T LoadData<T>(string relativePath);
        bool IsFileExist(string relativePath);
        bool DeleteFile(string relativePath);
    }
}