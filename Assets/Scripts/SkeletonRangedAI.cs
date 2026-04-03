using UnityEngine;

/// <summary>
/// Simple skeleton ranged AI:
/// - Detects player within range
/// - Keeps distance by moving away when player is too close
/// - Shoots projectile prefab with cooldown
/// </summary>
public class SkeletonRangedAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;

    [Header("Distance")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float tooCloseRange = 5f;
    [SerializeField] private float moveAwaySpeed = 3f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float projectileSpeed = 14f;
    [SerializeField] private float projectileDamage = 10f;

    private float nextAttackTime;

    private void Awake()
    {
        if (projectileSpawnPoint == null)
        {
            projectileSpawnPoint = transform;
        }
    }

    private void Update()
    {
        if (!ResolvePlayer())
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
            MoveAway(toPlayer.normalized);
        }

        if (Time.time >= nextAttackTime)
        {
            Shoot(toPlayer.normalized);
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private bool ResolvePlayer()
    {
        if (player != null)
        {
            return true;
        }

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject == null)
        {
            return false;
        }

        player = playerObject.transform;
        return true;
    }

    private void FacePlayer(Vector3 toPlayer)
    {
        Vector3 flat = new Vector3(toPlayer.x, 0f, toPlayer.z);
        if (flat.sqrMagnitude > 0.0001f)
        {
            transform.forward = flat.normalized;
        }
    }

    private void MoveAway(Vector3 toPlayerDirection)
    {
        transform.position += -toPlayerDirection * moveAwaySpeed * Time.deltaTime;
    }

    private void Shoot(Vector3 direction)
    {
        if (projectilePrefab == null)
        {
            return;
        }

        GameObject projectileObject = Instantiate(
            projectilePrefab,
            projectileSpawnPoint.position,
            Quaternion.LookRotation(direction, Vector3.up));

        SkeletonProjectile projectile = projectileObject.GetComponent<SkeletonProjectile>();
        if (projectile == null)
        {
            projectile = projectileObject.AddComponent<SkeletonProjectile>();
        }

        projectile.SetDamage(projectileDamage);

        Rigidbody rb = projectileObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
        }
    }
}
