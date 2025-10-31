using UnityEngine;

public class SpawnBomb : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn; 
    [SerializeField] private Transform player;       
    [SerializeField] private KeyCode spawnKey = KeyCode.Space; 

    void Update()
    {
        if (Input.GetKeyDown(spawnKey))
        {
            Instantiate(objectToSpawn, player.position, player.rotation);
        }
    }
}