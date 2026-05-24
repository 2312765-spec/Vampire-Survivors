using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}