
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    public sealed class FloorData
    {
        public int Size;
        public HashSet<Vector2Int> HoleCells = new();
        public ShapeKey ShapeKey = ShapeKey.O;
        public int Rotation;
        public ColorId ColorId;
        public GameObject[,] Cubes;
        public bool IsCleared = false;
    }
}