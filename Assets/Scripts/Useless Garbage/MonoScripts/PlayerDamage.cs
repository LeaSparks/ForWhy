using UnityEngine;

public class PlayerDamage : MonoBehaviour

{
    [SerializeField] private GameObject deathCanvas;
    [SerializeField] private GameObject hudCanvas;
    void OnCollisionEnter(Collision collision)
    {
        {
            Debug.Log("collision detected");
        }
      

        if(collision.gameObject.tag=="boss")
        { 
            deathCanvas.gameObject.SetActive(true);
            hudCanvas.gameObject.SetActive(false);

            Destroy(gameObject); 
        } 
    }
}