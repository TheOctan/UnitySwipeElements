using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace OctanGames.Services
{
    public class JsonDataService : IDataService
    {
        public bool SaveData<T>(string relativePath, T data)
        {
            string path = GetFilePath(relativePath);

            try
            {
                if (File.Exists(path))
                {
                    Debug.Log("Data exists. Deleting old file and writing a new one!");
                    File.Delete(path);
                }

                using FileStream stream = File.Create(path);
                stream.Close();
                File.WriteAllText(path, JsonConvert.SerializeObject(data));

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to save data to: {e.Message} {e.StackTrace}");
                return false;
            }
        }

        public T LoadData<T>(string relativePath)
        {
            string path = GetFilePath(relativePath);

            if (!File.Exists(path))
            {
                Debug.LogError($"Can not load file at {path}. File does not exists!");
                throw new FileNotFoundException($"{path} does not exist!");
            }

            try
            {
                var data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
                throw;
            }
        }

        public bool IsFileExist(string relativePath)
        {
            string path = GetFilePath(relativePath);

            return File.Exists(path);
        }

        public bool DeleteFile(string relativePath)
        {
            string path = GetFilePath(relativePath);
            if (File.Exists(path))
            {
                File.Delete(path);

                Debug.Log($"File {relativePath} deleted");
                return true;
            }

            Debug.Log($"File {relativePath} not exist");
            return false;
        }

        private static string GetFilePath(string relativePath)
        {
            return Path.Combine(Application.persistentDataPath, relativePath);
        }
    }
}