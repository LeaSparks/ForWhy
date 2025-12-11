using UnityEngine;
using UnityEngine.Events;

public class CollectableItem : MonoBehaviour
{
    // UnityEvents visible in Inspector
    public UnityEvent OnItemCollected;
    public UnityEvent<int> OnPointsAwarded;
    
    [SerializeField] private int _pointValue = 10;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Invoke events
            OnItemCollected?.Invoke();
            OnPointsAwarded?.Invoke(_pointValue);
            
            Destroy(gameObject);
        }
    }
}