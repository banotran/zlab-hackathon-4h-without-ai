using UnityEngine;

public class Cube : MonoBehaviour
{
    public Vector3 Position => transform.position;
    public Vector3 LocalPosition => transform.localPosition;
    void Start()
    {
        if (CubeController.HasInstance)
            CubeController.Instance.AddCube(this);
    }

    void OnDestroy()
    {
        if (CubeController.HasInstance)
            CubeController.Instance.RemoveCube(this);
    }
}