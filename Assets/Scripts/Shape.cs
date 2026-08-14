using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    [SerializeField] List<Cube> cubes = new();
    public List<Cube> Cubes => cubes;

    private bool isDragged = false;
    private Vector3 draggedStartPositionDelta;
    private Vector3 originPosition;

    void Start()
    {
        originPosition = transform.position;
    }


    public void SetShapeClicked()
    {
        isDragged = true;
        draggedStartPositionDelta = ShapeController.Instance.WorldMousePosition - transform.position;
    }

    public void SetShapeReleased()
    {
        isDragged = false;
    }

    void Update()
    {
        if (isDragged)
        {
            var newPos = ShapeController.Instance.WorldMousePosition - draggedStartPositionDelta;
            newPos.y = transform.position.y;
            transform.position = newPos;
        }
    }

    public void ResetPosition()
    {
        transform.position = originPosition;
    }
}
