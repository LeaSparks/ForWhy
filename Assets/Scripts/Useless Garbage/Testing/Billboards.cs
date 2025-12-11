using UnityEngine;

public class Billboards : MonoBehaviour
{
    [SerializeField] private Camera _activeCamera;

    

        void Update()
        {
            Camera camera = Camera.main;
            if (camera != null)

                transform.LookAt(camera.transform);
            transform.Rotate(0, 180, 0);
        }
    
}