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

    [Header("State Flow")]
    [Tooltip("Minimum time to stay in a state before switching to another.")]
    [SerializeField] private float minStateDuration = 0.2f;

    private EnemyState currentState = EnemyState.Idle;
    private float stateEnterTime;
    private float nextAttackTime;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        ResolvePlayerReferences();
        stateEnterTime = Time.time;
    }

    private void Update()
    {
        // Keep references valid and avoid null-reference errors.
        if (!ResolvePlayerReferences())
        {
            TrySetState(EnemyState.Idle);
            return;
        }

        float sqrDistanceToPlayer = (player.position - transform.position).sqrMagnitude;

        EnemyState desiredState = DetermineState(sqrDistanceToPlayer);
        TrySetState(desiredState);

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
    /// Determines what state the AI wants to be in based on player distance.
    /// </summary>
    private EnemyState DetermineState(float sqrDistanceToPlayer)
    {
        float attackRangeSqr = attackRange * attackRange;
        float attackExitRangeSqr = (attackRange + attackExitBuffer) * (attackRange + attackExitBuffer);
        float detectionRangeSqr = detectionRange * detectionRange;

        // If already attacking, allow a small hysteresis before leaving attack.
        if (currentState == EnemyState.Attack)
        {
            if (sqrDistanceToPlayer <= attackExitRangeSqr)
            {
                return EnemyState.Attack;
            }

            if (sqrDistanceToPlayer <= detectionRangeSqr)
            {
                return EnemyState.Chase;
            }

            return EnemyState.Idle;
        }

        if (sqrDistanceToPlayer <= attackRangeSqr)
        {
            return EnemyState.Attack;
        }

        if (sqrDistanceToPlayer <= detectionRangeSqr)
        {
            return EnemyState.Chase;
        }

        return EnemyState.Idle;
    }

    /// <summary>
    /// Applies state changes with a small transition delay and debug logs.
    /// </summary>
    private void TrySetState(EnemyState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        // Slight delay between state transitions.
        if (Time.time < stateEnterTime + minStateDuration)
        {
            return;
        }

        currentState = newState;
        stateEnterTime = Time.time;

        Debug.Log($"{gameObject.name} state changed to: {currentState}");
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
