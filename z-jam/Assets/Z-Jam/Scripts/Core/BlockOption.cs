using Game.Core;
using UnityEngine;

public class BlockOption : MonoBehaviour
{
    public ShapeKey Key;
    public int Rotation;
    public System.Action<BlockOption> OnClicked;

    private void OnMouseDown()
    {
        OnClicked?.Invoke(this);
    }
}
