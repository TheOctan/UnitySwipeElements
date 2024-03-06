using UnityEngine;

namespace OctanGames.Extensions
{
    public static class ArrayExtensions
    {
        public static T GetElementByIndex<T>(this T[,] array, Vector2Int index)
        {
            return array[index.x, index.y];
        }

        public static void SetElementByIndex<T>(this T[,] array, Vector2Int index, T element)
        {
            array[index.x, index.y] = element;
        }
    }
}