using System;
using UnityEngine;

public static class PublicEvents
{
    public static Action<Cube> OnCubePlaced;
    public static Action<Shape> OnShapeClicked;
    public static Action<Shape> OnShapeReleased;
    public static Action OnMouseUp;
}
