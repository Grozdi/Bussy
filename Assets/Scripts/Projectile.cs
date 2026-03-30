using UnityEngine;

/// <summary>
/// Simple projectile behavior:
/// - Moves forward at a constant speed
/// - Applies damage on impact if target has EnemyHealth
/// - Destroys itself after impact or when lifetime expires
/// </summary>
public class Projectile : MonoBehaviour
{
    [Tooltip("Forward movement speed (units per second).")]
    public float speed = 20f;

    [Tooltip("Damage applied when hitting an EnemyHealth target.")]
    public float damage = 10f;

    [Tooltip("How long the projectile lives before auto-destruction.")]
    public float lifetime = 3f;

    private void Start()
    {
        // Auto-destroy after a fixed lifetime to avoid lingering projectiles.
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move forward constantly in the projectile's local forward direction.
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Try to find EnemyHealth on the collided object.
        EnemyHealth enemyHealth = collision.collider.GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }

        // Destroy projectile after any impact.
        Destroy(gameObject);
    }
}
