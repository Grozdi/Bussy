using UnityEngine;

/// <summary>
/// Reusable projectile used by both player and enemies.
/// Handles movement, collision, and damage.
/// </summary>
public class Projectile : MonoBehaviour
{
    public enum DamageTarget
    {
        Enemy,
        Player,
        Both
    }

    [Tooltip("Movement speed (units per second).")]
    [SerializeField] private float speed = 20f;

    [Tooltip("Damage dealt on impact.")]
    [SerializeField] private float damage = 10f;

    [Tooltip("Lifetime before auto-destruction.")]
    [SerializeField] private float lifetime = 3f;

    [Tooltip("Which side this projectile can damage.")]
    [SerializeField] private DamageTarget damageTarget = DamageTarget.Enemy;

    private Vector3 moveDirection;

    private void Awake()
    {
        moveDirection = transform.forward;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    /// <summary>
    /// Initializes projectile flight and damage settings.
    /// </summary>
    public void Fire(Vector3 direction, float speedValue, float damageValue, DamageTarget target)
    {
        moveDirection = direction.sqrMagnitude > 0.0001f ? direction.normalized : transform.forward;
        speed = speedValue;
        damage = damageValue;
        damageTarget = target;
        transform.forward = moveDirection;
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

    private void ApplyDamage(Component hit)
    {
        if (damageTarget == DamageTarget.Enemy || damageTarget == DamageTarget.Both)
        {
            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }

        if (damageTarget == DamageTarget.Player || damageTarget == DamageTarget.Both)
        {
            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
