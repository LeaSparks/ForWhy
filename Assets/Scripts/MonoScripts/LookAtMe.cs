using UnityEngine;

public class LookAtMe : MonoBehaviour
{
    Camera mainCamera;
    
    [SerializeField] private int xAngle;
    [SerializeField] private int yAngle;
    [SerializeField] private int zAngle;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        transform.LookAt(mainCamera.transform);
        transform.Rotate(xAngle, yAngle, zAngle);
    }
}
