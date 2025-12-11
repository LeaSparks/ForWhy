
using UnityEngine;

public class FollowFogToCamera : MonoBehaviour
{
    public Transform target;
    public float height = 1.5f;
    public bool lockY = true;
    public Vector3 offset = Vector3.zero;

    void Reset()
    {
        target = Camera.main ? Camera.main.transform : null;
    }

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 pos = target.position + offset;
        if (lockY) pos.y = target.position.y + height;
        transform.position = pos;
    }
}
