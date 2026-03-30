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

    [Tooltip("Extra distance to leave Attack state, helping smooth transitions.")]
    [SerializeField] private float attackExitBuffer = 0.35f;

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
        ResolvePlayerReferences();
    }

    private void Update()
    {
        // Keep references valid and avoid null-reference errors.
        if (!ResolvePlayerReferences())
        {
            SetState(EnemyState.Idle);
            return;
        }

        float sqrDistanceToPlayer = (player.position - transform.position).sqrMagnitude;

        UpdateState(sqrDistanceToPlayer);
        RunState(sqrDistanceToPlayer);
    }

    /// <summary>
    /// Resolves player Transform and PlayerHealth references.
    /// Returns true when both references are available.
    /// </summary>
    private bool ResolvePlayerReferences()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (player == null)
        {
            playerHealth = null;
            return false;
        }

        if (playerHealth == null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        return playerHealth != null;
    }

    /// <summary>
    /// Updates the current AI state based on distance to player.
    /// Uses a small buffer to avoid rapid state flipping around attack range.
    /// </summary>
    private void UpdateState(float sqrDistanceToPlayer)
    {
        float attackRangeSqr = attackRange * attackRange;
        float attackExitRangeSqr = (attackRange + attackExitBuffer) * (attackRange + attackExitBuffer);
        float detectionRangeSqr = detectionRange * detectionRange;

        if (currentState == EnemyState.Attack)
        {
            if (sqrDistanceToPlayer <= attackExitRangeSqr)
            {
                SetState(EnemyState.Attack);
            }
            else if (sqrDistanceToPlayer <= detectionRangeSqr)
            {
                SetState(EnemyState.Chase);
            }
            else
            {
                SetState(EnemyState.Idle);
            }

            return;
        }

        if (sqrDistanceToPlayer <= attackRangeSqr)
        {
            SetState(EnemyState.Attack);
        }
        else if (sqrDistanceToPlayer <= detectionRangeSqr)
        {
            SetState(EnemyState.Chase);
        }
        else
        {
            SetState(EnemyState.Idle);
        }
    }

    /// <summary>
    /// Executes behavior for the current state.
    /// </summary>
    private void RunState(float sqrDistanceToPlayer)
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
                AttackPlayer(Mathf.Sqrt(sqrDistanceToPlayer));
                break;
        }
    }

    private void SetState(EnemyState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;
    }

    private void ChasePlayer()
    {
        // Move toward the player using transform-based movement.
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Face movement direction while chasing.
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
        Vector3 lookDirection = player.position - transform.position;
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

        playerHealth.TakeDamage(attackDamage);
        nextAttackTime = Time.time + attackCooldown;

        Debug.Log($"{gameObject.name} attacked player for {attackDamage} damage at distance {distanceToPlayer:F2}");
    }
}
