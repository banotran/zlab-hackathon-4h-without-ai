using System.Collections.Generic;
using System.Runtime.InteropServices;
using DG.Tweening;
using Game.Core;
using MeowgaByte.Utils;
using Unity.Android.Gradle;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Manager
{
    public sealed class GameManager : MonoSingleton<GameManager>
    {
        [Header("Ref")]
        [SerializeField] private ColorDatabase _colorDb;
        [SerializeField] private TowerBuilder _towerBuilder;
        [SerializeField] private GameObject _cubePrefab;
        [SerializeField] private Transform[] _optionSlots;

        [SerializeField] public CameraFollowTower _cameraFollow;

        
        [Header("Option Visual Settings")]
        [SerializeField] private float _spacing = 0.6f;
        [SerializeField] private float _scale = 0.55f;

        [Header("Animation Settings")]
        [SerializeField] private float _fillDuration = 0.5f;
        [SerializeField] private float _dropHeight = 3f;

        private int _currentFloor = 0;
        private readonly List<BlockOption> _currentOptions = new List<BlockOption>();

        #region Base
        private void Start()
        {
            _towerBuilder.BuildTower();
            _currentFloor = _towerBuilder.Floors.Count - 1;
            if (_cameraFollow != null) _cameraFollow.SnapToFloor(_currentFloor);
            SpawnOptions();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _towerBuilder.BuildTower();
                SpawnOptions();
            }
        }
        #endregion

        #region Option Handler
        private void SpawnOptions()
        {
            ClearOptions();
            var floor = _towerBuilder.Floors[_currentFloor];

            int correctSlot = Random.Range(0, _optionSlots.Length);

            for(int i = 0; i < _optionSlots.Length; i++)
            {
                ShapeKey key;
                int rot;
                ColorId colorId;
                if (i == correctSlot)
                {
                    key = floor.ShapeKey;
                    rot = floor.Rotation;
                    colorId = floor.ColorId;
                }
                else
                {
                    do
                    {
                        key = TetrisShapes.Keys[Random.Range(0, TetrisShapes.Keys.Count)];
                    }
                    while (key == floor.ShapeKey);
                    rot = Random.Range(0, 4);
                    colorId = floor.ColorId;
                }
                CreateOptionVisual(_optionSlots[i], key, rot, colorId);
            }
        }

        private void ClearOptions()
        {
            foreach(var o in _currentOptions)
            {
                if (o != null)
                {
                    o.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
                    {
                        Destroy(o.gameObject);
                    });
                }
            }
            _currentOptions.Clear();
        }

        private void CreateOptionVisual(Transform slot, ShapeKey key, int rot, ColorId colorId)
        {
            GameObject holder = new GameObject($"Option_{key}");
            holder.transform.SetParent(slot);
            holder.transform.localPosition = Vector3.zero;
            holder.transform.localRotation = Quaternion.identity;

            var cells = TetrisShapes.Rotate(TetrisShapes.Shapes[key], rot);

            foreach(var c in cells)
            {
                GameObject cube = Instantiate(_cubePrefab, holder.transform);
                cube.transform.localPosition = new Vector3(c.x * _spacing, 0, c.y * _spacing);
                cube.GetComponent<MeshRenderer>().material = _colorDb.GetMatById(colorId);
                cube.transform.localScale = Vector3.one * _scale;
            }

            BoxCollider collider = holder.AddComponent<BoxCollider>();
            collider.size = new Vector3(3f, 1f, 3f);
            collider.center = new Vector3(0.9f, 0, 0.9f);

            BlockOption option = holder.AddComponent<BlockOption>();
            option.Key = key;
            option.Rotation = rot;
            option.OnClicked = OnOptionClicked;
            _currentOptions.Add(option);
        }

        private void OnOptionClicked(BlockOption op)
        {
            var floor = _towerBuilder.Floors[_currentFloor];
            bool correct = op.Key == floor.ShapeKey;

            if (correct)
            {
                ClearOptions();
                _towerBuilder.ClearFloor(_currentFloor, AdvanceFloor);
            }
            else
            {
                op.transform.DOShakePosition(0.3f, 0.15f, 10, 90);
            }
        }

        private void AdvanceFloor()
        {
            _currentFloor--;
            if (_currentFloor >= _towerBuilder.Floors.Count)
            {
                Debug.Log("Win");
                return;
            }
            if (_cameraFollow != null) _cameraFollow.MoveToFloor(_currentFloor);
            SpawnOptions();
        }
        #endregion
    }
}