using UnityEngine;

public class Health : MonoBehaviour
{
    public EntityHealth.HealthLevel healthLevel;  
    private float currentHealth;

  
    public static event System.Action OnPlayerDeath;

    private void Start()
    {
        
        SetHealthBasedOnLevel();
    }

    private void SetHealthBasedOnLevel()
    {
        switch (healthLevel)
        {
            case EntityHealth.HealthLevel.Player:
                currentHealth = (float)EntityHealth.HealthLevel.Player;
                break;
            case EntityHealth.HealthLevel.Low:
                currentHealth = (float)EntityHealth.HealthLevel.Low;
                break;
            case EntityHealth.HealthLevel.Medium:
                currentHealth = (float)EntityHealth.HealthLevel.Medium;
                break;
            case EntityHealth.HealthLevel.High:
                currentHealth = (float)EntityHealth.HealthLevel.High;
                break;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Entity has died.");

        
        OnPlayerDeath?.Invoke(); 
        
        Destroy(gameObject);
    }
}