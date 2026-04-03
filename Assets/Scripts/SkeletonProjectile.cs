using UnityEngine;

/// <summary>
/// Projectile used by skeleton ranged enemies.
/// Damages PlayerHealth on hit and destroys itself.
/// </summary>
public class SkeletonProjectile : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 5f;

    public void SetDamage(float value)
    {
        damage = value;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ApplyDamage(collision.collider);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        ApplyDamage(other);
        Destroy(gameObject);
    }

    private void ApplyDamage(Component target)
    {
        PlayerHealth health = target.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }
    }
}
