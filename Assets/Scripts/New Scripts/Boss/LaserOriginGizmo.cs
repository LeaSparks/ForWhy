using UnityEngine;

public class LaserOriginGizmo : MonoBehaviour
{
    public Color gizmoColor = Color.red;
    public float gizmoRadius = 0.15f;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, gizmoRadius);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.8f);
    }
}