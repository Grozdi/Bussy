using UnityEngine;

/// <summary>
/// Simple skeleton ranged AI:
/// - Detects player at distance
/// - Keeps distance (backs away if too close)
/// - Shoots projectiles with cooldown
/// </summary>
public class SkeletonRangedAI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Optional player reference. If empty, found via Player tag.")]
    [SerializeField] private Transform player;

    [Tooltip("Projectile prefab fired by the skeleton.")]
    [SerializeField] private GameObject projectilePrefab;

    [Tooltip("Spawn point for projectiles. If empty, uses this transform.")]
    [SerializeField] private Transform projectileSpawnPoint;

    [Header("Detection / Distance")]
    [Tooltip("Distance at which skeleton starts engaging the player.")]
    [SerializeField] private float detectionRange = 15f;

    [Tooltip("If player is closer than this, skeleton moves away.")]
    [SerializeField] private float tooCloseRange = 5f;

    [Tooltip("Movement speed used while backing away.")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Attack")]
    [Tooltip("Seconds between projectile shots.")]
    [SerializeField] private float attackCooldown = 1.5f;

    [Tooltip("Projectile launch speed.")]
    [SerializeField] private float projectileSpeed = 14f;

    [Tooltip("Damage dealt to PlayerHealth when projectile hits.")]
    [SerializeField] private float projectileDamage = 10f;

    private PlayerHealth playerHealth;
    private float nextAttackTime;

    private void Awake()
    {
        if (projectileSpawnPoint == null)
        {
            projectileSpawnPoint = transform;
        }

        ResolvePlayerReferences();
    }

    private void Update()
    {
        if (!ResolvePlayerReferences())
        {
            return;
        }

        Vector3 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;

        if (distance > detectionRange)
        {
            return;
        }

        FacePlayer(toPlayer);

        if (distance < tooCloseRange)
        {
            MoveAwayFromPlayer(toPlayer.normalized);
        }

        if (Time.time >= nextAttackTime)
        {
            ShootProjectile();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private bool ResolvePlayerReferences()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
                playerHealth = playerObject.GetComponent<PlayerHealth>();
            }
        }

        if (player != null && playerHealth == null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        return player != null && playerHealth != null;
    }

    private void FacePlayer(Vector3 toPlayer)
    {
        Vector3 flatDirection = new Vector3(toPlayer.x, 0f, toPlayer.z);
        if (flatDirection.sqrMagnitude > 0.0001f)
        {
            transform.forward = flatDirection.normalized;
        }
    }

    private void MoveAwayFromPlayer(Vector3 toPlayerDirection)
    {
        Vector3 awayDirection = -toPlayerDirection;
        transform.position += awayDirection * moveSpeed * Time.deltaTime;
    }

    private void ShootProjectile()
    {
        if (projectilePrefab == null)
        {
            return;
        }

        Vector3 spawnPosition = projectileSpawnPoint.position;
        Vector3 shootDirection = (player.position - spawnPosition).normalized;
        Quaternion spawnRotation = Quaternion.LookRotation(shootDirection, Vector3.up);

        GameObject projectileObject = Instantiate(projectilePrefab, spawnPosition, spawnRotation);
        if (projectileObject == null)
        {
            return;
        }

        // Ensure spawned projectile can damage PlayerHealth.
        SkeletonProjectile projectile = projectileObject.GetComponent<SkeletonProjectile>();
        if (projectile == null)
        {
            projectile = projectileObject.AddComponent<SkeletonProjectile>();
        }

        projectile.Initialize(projectileDamage);

        Rigidbody rb = projectileObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = shootDirection * projectileSpeed;
        }
    }
}

/// <summary>
/// Minimal projectile damage handler for skeleton shots.
/// Calls PlayerHealth.TakeDamage on hit and destroys itself.
/// </summary>
public class SkeletonProjectile : MonoBehaviour
{
    private float damage;

    public void Initialize(float damageValue)
    {
        damage = damageValue;
    }

    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerHealth health = collision.collider.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
