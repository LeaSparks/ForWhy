using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // Event declaration using Action delegate
    public event System.Action<int> OnHealthChanged;
    public event System.Action OnPlayerDeath;
    
    [SerializeField] private int _currentHealth;
    [SerializeField] private int _maxHealth = 100;
    
    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;
    
    private void Start()
    {
        _currentHealth = _maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        _currentHealth = Mathf.Max(0, _currentHealth - damage);
        
        // Notify all observers of health change
        OnHealthChanged?.Invoke(_currentHealth);
        
        if (_currentHealth <= 0)
        {
            OnPlayerDeath?.Invoke();
        }
    }
}