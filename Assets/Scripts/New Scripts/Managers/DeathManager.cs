using UnityEngine;

public class DeathManager : MonoBehaviour
{
    public GameObject gameCanvas;     
    public GameObject deathCanvas;     
    public ScoreManager scoreManager;  

    private PlayerStats playerStats;   

    void OnEnable()
    {
        Health.OnPlayerDeath += HandlePlayerDeath;

    
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerStats = player.GetComponent<PlayerStats>();
    }

    void OnDisable()
    {
        Health.OnPlayerDeath -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        Debug.Log("DeathManager: Player has died.");

        if (playerStats != null)
            playerStats.StopTracking();

        if (gameCanvas != null)
            gameCanvas.SetActive(false);

        if (deathCanvas != null)
            deathCanvas.SetActive(true);

        if (scoreManager != null)
            scoreManager.OnPlayerDeath();

    }
}