
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Core
{
    public static class TetrisShapes
    {
        public static readonly Dictionary<ShapeKey, Vector2Int[]> Shapes = new Dictionary<ShapeKey, Vector2Int[]>
        {
            {
                ShapeKey.I, 
                new Vector2Int[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(2, 0),
                    new Vector2Int(3, 0),
                }
            },
            {
                ShapeKey.O,
                new Vector2Int[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 1),
                }
            },
            {
                ShapeKey.T,
                new Vector2Int[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(2, 0),
                    new Vector2Int(1, 1),
                }
            },  
            {
                ShapeKey.S,
                new Vector2Int[]
                {
                    new Vector2Int(1, 0),
                    new Vector2Int(2, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 1),
                }
            },  
            {
                ShapeKey.Z,
                new Vector2Int[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(1, 1),
                    new Vector2Int(2, 1),
                }
            },  
            {
                ShapeKey.L,
                new Vector2Int[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(0, 2),
                    new Vector2Int(1, 0),
                }
            },  
            {
                ShapeKey.J,
                new Vector2Int[]
                {
                    new Vector2Int(1, 0),
                    new Vector2Int(1, 1),
                    new Vector2Int(1, 2),
                    new Vector2Int(0, 0),
                }
            },            
        };

        public static List<ShapeKey> Keys = new List<ShapeKey>(Shapes.Keys);

        public static Vector2Int[] Rotate(Vector2Int[] cells, int times)
        {
            Vector2Int[] res = (Vector2Int[])cells.Clone();
            int t = ((times % 4) + 4) % 4;

            for (int i = 0; i < t; i++)
            {
                for (int j = 0; j < res.Length; j++)
                {
                    Vector2Int c = res[j];
                    res[j] = new Vector2Int(c.y, -c.x);
                }
            }
            return Normalize(res);
        } 

        private static Vector2Int[] Normalize(Vector2Int[] a)
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;

            foreach(var c in a)
            {
                minX = c.x < minX ? c.x : minX;
                minY = c.y < minY ? c.y : minY;
            }

            Vector2Int[] res = new Vector2Int[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                res[i] = new Vector2Int(a[i].x - minX, a[i].y - minY);
            }
            return res;
        }

        public static Vector2Int GetBounds(Vector2Int[] cells)
        {
            int maxX = 0; 
            int maxY = 0;
            foreach(var c in cells)
            {
                maxX = c.x > maxX ? c.x : maxX;
                maxY = c.y > maxY ? c.y : maxY;
            }

            return new Vector2Int(maxX + 1, maxY + 1);
        }
    }
}