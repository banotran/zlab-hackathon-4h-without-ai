using System.Collections.Generic;
using DG.Tweening;
using Game.Manager;
using UnityEngine;

namespace Game.Core
{
    public class TowerBuilder : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject _cubePrefab;

        [Header("References")] 
        [SerializeField] private ColorDatabase _colorDb;

        [Header("Tower Configs")]
        [SerializeField] private int _gridSize = 5;
        [SerializeField] private int _floorCount = 10;
        [SerializeField] private float _spacing = 1.05f;
        [SerializeField] private float _floorHeight = 1.05f;

        [Header("Animation Settings")]
        [SerializeField] private float _appearPunchDuration = 0.2f;
        [SerializeField] private float _fillDuration = 0.5f;
        [SerializeField] private float _dropHeight = 3f;
        [SerializeField] private float _breakPunchScale = 0.25f;
        [SerializeField] private float _disappearDuration = 0.35f;
        [SerializeField] private float _disappearDelayTime = 0.15f;

        public List<FloorData> Floors = new();

        public int GridSize => _gridSize;
        public float Spacing => _spacing;
        public float Height => _floorHeight;


        #region Tower
        /// <summary>
        /// Hàm xây tháp
        /// </summary>
        public void BuildTower()
        {
            Floors.Clear();
            foreach(Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < _floorCount; i++)
            {
                FloorData floor = GenerateFloor(i);
                Floors.Add(floor);
            }
        }

        /// <summary>
        /// Tạo dữ liệu cho tầng
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private FloorData GenerateFloor(int idx)
        {
            var floor = new FloorData
            {
                Size = _gridSize,
                Cubes = new GameObject[_gridSize, _gridSize]
            };

            ShapeKey key;
            int rot;
            Vector2Int[] cells;
            Vector2Int bounds = new Vector2Int(0, 0);
            ColorId colorId = _colorDb.GetRandomColor();
            
            do
            {
                key = TetrisShapes.Keys[Random.Range(0, TetrisShapes.Keys.Count)];
                rot = Random.Range(0, 4);
                cells = TetrisShapes.Rotate(TetrisShapes.Shapes[key], rot);
                bounds = TetrisShapes.GetBounds(cells);
            }
            while (bounds.x >_gridSize || bounds.y > _gridSize);

            floor.ShapeKey = key;
            floor.Rotation = rot;
            floor.ColorId = colorId;
            // int offsetX = Random.Range(_gridSize / 2, _gridSize - bounds.x + 1);
            // int offsetY = Random.Range(_gridSize / 2, _gridSize - bounds.y + 1);
            int offsetX = _gridSize / 2;
            int offsetY = _gridSize / 2;
            foreach(var c in cells)
            {
                floor.HoleCells.Add(new Vector2Int(c.x + offsetX, c.y + offsetY));
            }

            // Tạo object tầng
            GameObject floorParent = new GameObject($"Floor_{idx}");
            floorParent.transform.SetParent(this.transform);
            floorParent.transform.localPosition = new Vector3(0, idx * _floorHeight, 0);

            // tạo tháp từ prefab
            for (int x = 0; x < _gridSize; x++)
            {
                for(int y = 0; y < _gridSize; y++)
                {
                    var pos = new Vector2Int(x, y);
                    if (floor.HoleCells.Contains(pos)) continue;

                    GameObject cube = Instantiate(_cubePrefab, floorParent.transform);
                    Vector3 cubePos = new Vector3(x * _spacing, 0, y * _spacing);
                    cube.transform.localPosition = cubePos;
                    cube.transform.DOPunchScale(_breakPunchScale * Vector3.one, _appearPunchDuration).SetEase(Ease.OutBounce);
                    cube.GetComponent<MeshRenderer>().material = _colorDb.GetMatById(colorId);
                    floor.Cubes[x, y] = cube;
                }
            }

            return floor;
        }
        #endregion

        public Vector3 GetWorldPositionForCell(int floorIdx, Vector2Int cell)
        {
            Transform floorParent = transform.GetChild(floorIdx);
            Vector3 pos = new Vector3(cell.x * _spacing, 0, cell.y * _spacing);
            return floorParent.TransformPoint(pos);
        }

        public void ClearFloor(int floorIdx, System.Action OnCompleted = null)
        {
            var floor = Floors[floorIdx];
            if (floor.IsCleared)
            {
                OnCompleted?.Invoke();
                return;
            }

            Transform floorPr = transform.GetChild(floorIdx);

            foreach (var c in floor.HoleCells)
            {
                Vector3 target = GetWorldPositionForCell(floorIdx, c);
                GameObject cube = Instantiate(_cubePrefab, target + Vector3.up * _dropHeight, Quaternion.identity, floorPr);
                cube.GetComponent<MeshRenderer>().material = _colorDb.GetMatById(floor.ColorId);
                cube.transform.DOMove(target, _fillDuration).SetEase(Ease.OutBounce);
            }

            floor.IsCleared = true;

            DOVirtual.DelayedCall(_fillDuration, () =>
            {
               floorPr.DOPunchPosition(Vector3.one * _breakPunchScale, 0.25f, 6, 0.5f).OnComplete(() =>
               {
                    BreakFloor(floorIdx, OnCompleted);
               });
            });
        }

        private void BreakFloor(int floorIdx, System.Action OnCompleted = null)
        {
            var floor = Floors[floorIdx];
            List<GameObject> cubes = new List<GameObject>();

            for (int x = 0; x < floor.Size; x++)
            {
                for (int y = 0 ; y < floor.Size; y++)
                {
                    if (floor.Cubes[x, y] != null)
                    {
                        cubes.Add(floor.Cubes[x, y]);
                    }
                }
            }

            if (cubes.Count == 0)
            {
                OnCompleted?.Invoke();
                return;
            }

            int remaining = cubes.Count;
            foreach(var c in cubes)
            {
                float delay = Random.Range(0f, _disappearDelayTime);
                Sequence s = DOTween.Sequence();

                s.AppendInterval(delay);
                s.Append(c.transform.DOScale(Vector3.zero, _disappearDuration).SetEase(Ease.InBack));

                // if (c.TryGetComponent<MeshRenderer>(out var r) && r.material.HasProperty("_Color"))
                // {
                //     s.Join(r.material.DOFade(0f, _disappearDuration));
                // }

                s.OnComplete(() =>
                {
                    EffectManager.Instance.PlayBlastParticle(c.transform.position, _colorDb.GetColorById(floor.ColorId));
                    Destroy(c);
                    remaining--;
                    if (remaining <= 0) OnCompleted?.Invoke();
                });
            }

            floor.Cubes = new GameObject[floor.Size, floor.Size];
        }
    }
}
///////////////////////// HELP