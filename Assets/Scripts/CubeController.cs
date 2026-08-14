using System.Collections.Generic;
using System.Xml.Schema;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class CubeController : Singleton<CubeController>
{
    private Dictionary<Vector3, Cube> cubeList = new();
    [SerializeField] private List<Vector3> currentEmptyPositions = new();
    [SerializeField] private float snapDistance;

    [SerializeField] private Transform cubeLayersTransform;
    [SerializeField] private List<Transform> cubeLayers = new();

    [SerializeField] private int currentActiveLayer = 0;

    public void AddCube(Cube cube)
    {
        cubeList.Add(cube.Position, cube);
    }

    public void RemoveCube(Cube cube)
    {
        cubeList.Remove(cube.Position);
    }

    void OnEnable()
    {
        PublicEvents.OnShapeReleased += HandleShapeReleased;
    }

    void OnDisable()
    {
        PublicEvents.OnShapeReleased -= HandleShapeReleased;
    }

    void Start()
    {
        LoadCurrentEmptyPosition();
        for (int i = 0; i < cubeLayersTransform.childCount; ++i)
        {
            cubeLayers.Add(cubeLayersTransform.GetChild(i));
        }
    }

    private bool CheckShapeFillableAtPosition(Shape shape, Vector3 position)
    {
        foreach (var cube in shape.Cubes)
        {
            var checkPosition = position + cube.LocalPosition;
            Debug.Log($"Position checking at {checkPosition}");
            if (!currentEmptyPositions.Contains(checkPosition))
                return false;
        }
        return true;
    }

    private void LoadCurrentEmptyPosition()
    {
        currentEmptyPositions.Clear();
        for (int x = 0; x < 5; ++x)
            for (int z = -4; z < 1; z++)
            {
                var vector = new Vector3(x, currentActiveLayer, z);
                if (cubeList.ContainsKey(vector))
                    continue;

                vector.y = 0;
                currentEmptyPositions.Add(vector);
            }
    }

    private void HandleShapeReleased(Shape shape)
    {
        var shapePos2D = To2DGrid(shape.transform.position);

        Vector3 emptyVector = Vector3.one * -10;
        Vector3 snapPosition = emptyVector;
        float minDistance = 0;

        foreach (var emptyPosition in currentEmptyPositions)
        {
            var emptyPos2D = To2DGrid(emptyPosition);
            var distance = Vector2.Distance(shapePos2D, emptyPos2D);

            Debug.Log($"Checking position at {emptyPosition}, 2d: {emptyPos2D}");

            if (distance > snapDistance)
            {
                Debug.Log($"Rejected because distance is too great, distance: {distance}, lim: {snapDistance}");
                continue;
            }

            if (snapPosition == emptyVector || distance < minDistance)
            {
                minDistance = distance;
                snapPosition = emptyPosition;
                Debug.Log($"Checking snap position with distance {minDistance}, snapPosition {snapPosition}, snapDistance {snapDistance}");
            }
        }

        if (snapPosition == emptyVector || !CheckShapeFillableAtPosition(shape, snapPosition))
        {
            shape.ResetPosition();
        }
        else
        {
            shape.transform.position = snapPosition;
            shape.transform.parent = cubeLayers[-currentActiveLayer];
            foreach (var cube in shape.Cubes)
            {
                currentEmptyPositions.Remove(cube.Position);
                if (currentEmptyPositions.Count == 0)
                {
                    HandleMoveUpLayer();
                }
            }
        }
    }

    public Vector2 To2DGrid(Vector3 position)
    {
        return new Vector2(position.x, position.z);
    }

    private void HandleMoveUpLayer()
    {
        cubeLayers[-currentActiveLayer].gameObject.SetActive(false);
        currentActiveLayer--;
        cubeLayersTransform.Translate(Vector3.up);
        LoadCurrentEmptyPosition();
    }
}