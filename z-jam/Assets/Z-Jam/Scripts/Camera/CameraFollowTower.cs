using UnityEngine;
using DG.Tweening;
using Game.Core;


public class CameraFollowTower : MonoBehaviour
{
    [Header("Tham chieu")]
    public TowerBuilder towerBuilder;

    public float orbitRadius = 8f;

    public float pitchAngle = 60f;

    public float yawAngle = 45f;

    [Header("Animation")]
    public float followDuration = 0.6f;
    public Ease followEase = Ease.InOutSine;

    Tween moveTween;
    Tween rotateTween;

    void Start()
    {
        if (towerBuilder != null)
            SnapToFloor(0);
    }

    public void SnapToFloor(int floorIndex)
    {
        Vector3 center = TowerCenter(floorIndex);
        Vector3 pos = center + GetOffset();
        transform.position = pos;
        transform.LookAt(center);
    }

    public void MoveToFloor(int floorIndex, float duration = -1f)
    {
        if (duration < 0f) duration = followDuration;

        Vector3 center = TowerCenter(floorIndex);
        Vector3 targetPos = center + GetOffset();
        Quaternion targetRot = Quaternion.LookRotation((center - targetPos).normalized, Vector3.up);

        moveTween?.Kill();
        rotateTween?.Kill();

        moveTween = transform.DOMove(targetPos, duration).SetEase(followEase);
        rotateTween = transform.DORotateQuaternion(targetRot, duration).SetEase(followEase);
    }
    Vector3 GetOffset()
    {
        float pitchRad = pitchAngle * Mathf.Deg2Rad;
        float yawRad = yawAngle * Mathf.Deg2Rad;

        float horizontalDist = orbitRadius * Mathf.Cos(pitchRad);
        float height = orbitRadius * Mathf.Sin(pitchRad);

        return new Vector3(
            horizontalDist * Mathf.Sin(yawRad),
            height,
            horizontalDist * Mathf.Cos(yawRad)
        );
    }

    Vector3 TowerCenter(int floorIndex)
    {
        float half = (towerBuilder.GridSize - 1) * towerBuilder.Spacing / 2f;
        float y = floorIndex * towerBuilder.Height;
        return new Vector3(half, y, half);
    }
}
