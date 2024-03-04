using System;
using UnityEngine;

namespace OctanGames.Gameplay.Levels
{
    [CreateAssetMenu(fileName = "Level Config", menuName = "Gameplay/Create Level Config", order = 1)]
    [Serializable]
    public class LevelConfig : ScriptableObject
    {
        [HideInInspector] public Color[] _Colors = new[] {Color.white, Color.blue, Color.yellow};
        [HideInInspector] public int Rows;
        [HideInInspector] public int Columns;
        public int[] Map;
        private int _rows;
        private int _columns;

        public void CreateMap()
        {
            Map = new int[Rows*Columns];
            _rows = Rows;
            _columns = Columns;
        }

        public void ResizeMap()
        {
            var newArray = new int[Rows, Columns];
            int minRows = Math.Min(Rows, _rows);
            int minCols = Math.Min(Columns, _columns);
            for(int i = 0; i < minRows; i++)
            for(int j = 0; j < minCols; j++)
                newArray[i, j] = this[i, j];
            
            CreateMap();
            for(int i = 0; i < Rows; i++)
            for(int j = 0; j < Columns; j++)
                this[i, j] = newArray[i, j];
        }

        public int[,] GetMap()
        {
            var newArray = new int[Rows, Columns];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    newArray[i, j] = this[i, j];
                }
            }

            return newArray;
        }

        public bool IsMapNull()
        {
            return Map == null;
        }
        
        public int this[int i, int j]
        {
            get => Map[i * Columns + j];
            set => Map[i * Columns + j] = value;
        }
    }
}