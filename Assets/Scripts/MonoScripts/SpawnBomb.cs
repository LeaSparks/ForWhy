using UnityEngine;

public class SpawnBomb : MonoBehaviour
{
    public GameObject objectToSpawn; 
    public Transform player;       
    public KeyCode spawnKey = KeyCode.Space; 

    void Update()
    {
        if (Input.GetKeyDown(spawnKey))
        {
            Instantiate(objectToSpawn, player.position, player.rotation);
        }
    }
}