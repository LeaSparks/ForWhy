// LaserDamage.cs
using UnityEngine;

public class LaserDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damagePerSecond = 40f;
    public float hitRadius = 1.2f;
    public LayerMask damageLayers;

    [Header("Impact Effect")]
    public GameObject floorHitEffect;

    private GameObject activeEffect;

    public void ApplyLaserDamage(Vector3 hitPoint)
    {
        if (floorHitEffect != null)
        {
            if (activeEffect == null)
            {
                activeEffect = Instantiate(floorHitEffect, hitPoint, Quaternion.identity);
            }
            else
            {
                activeEffect.transform.position = hitPoint;
            }
        }

        Collider[] hits = Physics.OverlapSphere(hitPoint, hitRadius, damageLayers);

        foreach (Collider c in hits)
        {
            Health health = c.GetComponent<Health>();
            if (health != null)
            {
                DealScaledDamage(health);
            }
        }
    }

    void DealScaledDamage(Health targetHealth)
    {
        float dmg = damagePerSecond * Time.deltaTime;

        switch (targetHealth.healthLevel)
        {
            case EntityHealth.HealthLevel.Player:
                targetHealth.TakeDamage(dmg * 0.2f);
                break;

            case EntityHealth.HealthLevel.Low:
            case EntityHealth.HealthLevel.Medium:
            case EntityHealth.HealthLevel.High:
                targetHealth.TakeDamage(dmg);
                break;
        }
    }

    public void StopLaser()
    {
        if (activeEffect != null)
            Destroy(activeEffect);
    }
}