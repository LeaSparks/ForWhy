using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
    
    [SerializeField] private int detonationTimer;
    void Awake()
    {
        StartCoroutine(Example());
        
        Debug.Log("Bomb Created!");

        IEnumerator Example()
        {
            yield return new WaitForSeconds(detonationTimer);
            Destroy(gameObject);
            Debug.Log("Bomb Exploded!");

            
        }


    }
    
}



