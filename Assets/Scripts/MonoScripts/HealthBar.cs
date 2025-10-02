using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private PlayerHealth _playerHealth;
    
    private void Start()
    {
        _playerHealth = FindFirstObjectByType<PlayerHealth>();
        
        // Subscribe to events
        _playerHealth.OnHealthChanged += UpdateHealthDisplay;
        _playerHealth.OnPlayerDeath += HandlePlayerDeath;
    }
    
    private void OnDestroy()
    {
        // Always unsubscribe to prevent memory leaks
        if (_playerHealth != null)
        {
            _playerHealth.OnHealthChanged -= UpdateHealthDisplay;
            _playerHealth.OnPlayerDeath -= HandlePlayerDeath;
        }
    }
    
    private void UpdateHealthDisplay(int newHealth)
    {
        // Update UI element
        Debug.Log($"Health updated: {newHealth}");
    }
    
    private void HandlePlayerDeath()
    {
        Debug.Log("Player has died!");
    }
}