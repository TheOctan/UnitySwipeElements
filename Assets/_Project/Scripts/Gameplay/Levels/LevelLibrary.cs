using System.Collections.Generic;
using UnityEngine;

namespace OctanGames.Gameplay.Levels
{
    [CreateAssetMenu(fileName = "LevelLibrary", menuName = "Gameplay/Create Level Library", order = 0)]
    public class LevelLibraryLibrary : ScriptableObject, ILevelLibrary
    {
        [SerializeField] private List<LevelConfig> _list;

        public int Count => _list.Count;

        public int[,] GetMapByNumber(int index)
        {
            return _list[index].GetMap();
        }

    }
}