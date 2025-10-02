using UnityEngine;

public class MenuCameraController : MonoBehaviour
{
    [SerializeField] private float _lookSpeed = 2f;
    [SerializeField] private CharacterController controller;

    void Update() 
    {
// Camera Rotation
    float mouseX = Input.GetAxis("Mouse X") * _lookSpeed;
    float mouseY = Input.GetAxis("Mouse Y") * _lookSpeed;

    transform.Rotate(Vector3.up * mouseX);
}
}
