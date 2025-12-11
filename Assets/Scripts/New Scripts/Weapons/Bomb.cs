using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Bomb Settings")]
    public float lifetime = 5f;           
    public float explosionRadius = 5f;     
    public float explosionDamage = 30f;   
    public LayerMask damageLayers;         
    public GameObject explosionEffectPrefab; 

    public delegate void BombDestroyedHandler();
    public event BombDestroyedHandler OnBombDestroyed;

    private void Start()
    {
        Invoke(nameof(Explode), lifetime);
    }

    public void Explode()
    {
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, damageLayers);
        foreach (Collider hit in hitColliders)
        {
            // Apply damage if the object has a Health script
            Health targetHealth = hit.GetComponent<Health>();
            if (targetHealth != null)
            {
                ApplyExplosionDamage(targetHealth);
            }
        }

        OnBombDestroyed?.Invoke();

        Destroy(gameObject);
    }

    private void ApplyExplosionDamage(Health targetHealth)
    {
        switch (targetHealth.healthLevel)
        {
            case EntityHealth.HealthLevel.Player:
                targetHealth.TakeDamage(explosionDamage * 0.2f); // 20% of explosion damage
                break;

            case EntityHealth.HealthLevel.Low:
                targetHealth.TakeDamage(explosionDamage);
                break;

            case EntityHealth.HealthLevel.Medium:
                targetHealth.TakeDamage(explosionDamage);
                break;

            case EntityHealth.HealthLevel.High:
                targetHealth.TakeDamage(explosionDamage);
                break;
        }
    }
}
