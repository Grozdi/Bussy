using UnityEngine;

/// <summary>
/// Simple enemy projectile:
/// - Moves toward player when fired
/// - Damages PlayerHealth on hit
/// - Destroys itself after a short lifetime
/// </summary>
public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 4f;

    private Transform target;
    private Vector3 travelDirection;

    /// <summary>
    /// Called by shooter when projectile is spawned.
    /// </summary>
    public void FireAt(Transform playerTarget)
    {
        target = playerTarget;

        if (target != null)
        {
            travelDirection = (target.position - transform.position).normalized;
        }
        else
        {
            // Fallback: keep current forward direction.
            travelDirection = transform.forward;
        }
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // If target still exists, keep moving toward its current position.
        if (target != null)
        {
            travelDirection = (target.position - transform.position).normalized;
        }

        transform.position += travelDirection * speed * Time.deltaTime;
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

    private void ApplyDamage(Component hitComponent)
    {
        PlayerHealth playerHealth = hitComponent.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }
}
