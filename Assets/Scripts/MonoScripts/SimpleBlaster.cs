using UnityEngine;

public class SimpleBlaster : MonoBehaviour
{
    public GameObject objectToSpawn;
    public Transform turret;
    public KeyCode fire = KeyCode.Space;


    void Update()
    {
        if (Input.GetButton("fire"))
        {
            Instantiate(objectToSpawn, turret.position, turret.rotation);
        }
    }
}
