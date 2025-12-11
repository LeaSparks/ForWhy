using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    public enum HealthLevel
    {
        Player = 1,    // 1 Health (for Player)
        Low = 100,     // 100 Health
        Medium = 200,  // 200 Health
        High = 300     // 300 Health
    }

    [Header("Entity Health")]
    public HealthLevel healthLevel;  // Select health level from enum
    private float currentHealth;

    private void Start()
    {
        // Set the entity's health based on the selected HealthLevel
        SetHealthBasedOnLevel();
    }

    private void SetHealthBasedOnLevel()
    {
        // Set health based on selected health level enum
        switch (healthLevel)
        {
            case HealthLevel.Player:
                currentHealth = (float)HealthLevel.Player;
                break;
            case HealthLevel.Low:
                currentHealth = (float)HealthLevel.Low;
                break;
            case HealthLevel.Medium:
                currentHealth = (float)HealthLevel.Medium;
                break;
            case HealthLevel.High:
                currentHealth = (float)HealthLevel.High;
                break;
        }

        // Optional: Log the current health to the console to verify
        Debug.Log("Entity health set to: " + currentHealth);
    }

    // Method to handle damage
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Entity has died.");
        Destroy(gameObject); // Destroy the entity when health reaches 0
    }
}