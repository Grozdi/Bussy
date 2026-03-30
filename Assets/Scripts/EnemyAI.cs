using UnityEngine;

/// <summary>
/// Simple enemy AI with three states:
/// Idle -> Chase -> Attack
/// Includes basic attack behavior with cooldown.
/// </summary>
public class EnemyAI : MonoBehaviour
{
    private enum EnemyState
    {
        Idle,
        Chase,
        Attack
    }

    [Header("References")]
    [Tooltip("Target player to track.")]
    [SerializeField] private Transform player;

    [Header("Movement")]
    [Tooltip("Move speed while chasing the player.")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Ranges")]
    [Tooltip("Distance at which the enemy starts chasing.")]
    [SerializeField] private float detectionRange = 10f;

    [Tooltip("Distance at which the enemy attacks and stops moving.")]
    [SerializeField] private float attackRange = 2f;

    [Header("Attack")]
    [Tooltip("Damage dealt to the player per attack.")]
    [SerializeField] private float attackDamage = 10f;

    [Tooltip("Time in seconds between attacks.")]
    [SerializeField] private float attackCooldown = 1f;

    private EnemyState currentState = EnemyState.Idle;
    private float nextAttackTime;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        // Optional fallback to common player tag.
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        CachePlayerHealth();
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        // Keep robust if player object changes at runtime.
        if (playerHealth == null)
        {
            CachePlayerHealth();
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        UpdateState(distanceToPlayer);
        RunState(distanceToPlayer);
    }

    /// <summary>
    /// Updates the current AI state based on distance to player.
    /// </summary>
    private void UpdateState(float distanceToPlayer)
    {
        if (distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attack;
        }
        else if (distanceToPlayer <= detectionRange)
        {
            currentState = EnemyState.Chase;
        }
        else
        {
            currentState = EnemyState.Idle;
        }
    }

    /// <summary>
    /// Executes behavior for the current state.
    /// </summary>
    private void RunState(float distanceToPlayer)
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                // Stay still.
                break;

            case EnemyState.Chase:
                ChasePlayer();
                break;

            case EnemyState.Attack:
                AttackPlayer(distanceToPlayer);
                break;
        }
    }

    private void ChasePlayer()
    {
        // Move toward the player using transform-based movement.
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Optional: face movement direction while chasing.
        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.forward = direction;
        }
    }

    private void AttackPlayer(float distanceToPlayer)
    {
        // Stop moving while attacking.
        // (No position updates in attack state.)

        // Keep facing player while in attack range.
        Vector3 lookDirection = (player.position - transform.position);
        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude > 0.0001f)
        {
            transform.forward = lookDirection.normalized;
        }

        // Prevent attack spamming with cooldown.
        if (Time.time < nextAttackTime)
        {
            return;
        }

        if (playerHealth == null)
        {
            Debug.LogWarning($"{gameObject.name} cannot attack: PlayerHealth not found.");
            return;
        }

        // Deal damage and set next attack time.
        playerHealth.TakeDamage(attackDamage);
        nextAttackTime = Time.time + attackCooldown;

        Debug.Log($"{gameObject.name} attacked player for {attackDamage} damage at distance {distanceToPlayer:F2}");
    }

    private void CachePlayerHealth()
    {
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }
}
