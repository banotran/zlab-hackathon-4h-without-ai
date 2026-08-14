using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShapeController : Singleton<ShapeController>
{
    [SerializeField] private LayerMask clickMask;
    [SerializeField] private List<Shape> availableShapes;

    private Vector2 currentMousePosition;
    private Shape selectingShape;

    public Vector2 CurrentMousePosition => currentMousePosition;
    public Vector3 WorldMousePosition => Camera.main.ScreenToWorldPoint(currentMousePosition);

    public void OnMousePrimary(InputValue inputValue)
    {
        currentMousePosition = inputValue.Get<Vector2>();
    }

    public void OnMouseClick(InputValue inputValue)
    {
        var value = inputValue.Get<float>();
        if (value > 0.5f)
        {
            Debug.Log($"Mouse down at {currentMousePosition}");
            CheckObjectHit();
        }
        else
        {
            Debug.Log($"Mouse up at {currentMousePosition}");
            if (selectingShape != null)
            {
                selectingShape.SetShapeReleased();
                PublicEvents.OnShapeReleased?.Invoke(selectingShape);
            }
            selectingShape = null;

        }
    }

    public void CheckObjectHit()
    {
        Ray ray = Camera.main.ScreenPointToRay(currentMousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, clickMask))
        {
            Transform hitTransform = hit.transform;
            if (hitTransform.parent != null)
                hitTransform = hitTransform.parent;

            foreach (var shape in availableShapes)
                if (shape.transform == hitTransform)
                {
                    selectingShape = shape;
                    selectingShape.SetShapeClicked();
                }

            Debug.Log($"Hit game object with name: {hitTransform.name}");
        }
    }
}
