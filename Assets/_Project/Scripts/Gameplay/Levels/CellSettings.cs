using System;
using System.Collections.Generic;
using UnityEngine;

namespace OctanGames.Gameplay.Levels
{
    [CreateAssetMenu(fileName = "CellSettings", menuName = "Gameplay/Create Cell Settings", order = 1)]
    public class CellSettings : ScriptableObject
    {
        public CellView CellPrefab;
        public List<CellAnimation> CellAnimations;

        [Serializable]
        public class CellAnimation
        {
            public Sprite Sprite;
            public Sprite[] IdleAnimation;
            public Sprite[] DestroyAnimation;
        }
    }
}