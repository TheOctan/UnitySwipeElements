using System.Collections.Generic;
using UnityEngine;

namespace OctanGames.Gameplay.Levels
{
    [CreateAssetMenu(fileName = "LevelLibrary", menuName = "Gameplay/Create Level Library", order = 0)]
    public class LevelLibrary : ScriptableObject, ILevels
    {
        [SerializeField] private List<LevelConfig> _list;

        public int[,] this[int index] => _list[index].GetMap();

        public int Count => _list.Count;
    }
}