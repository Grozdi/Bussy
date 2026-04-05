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
    [Tooltip("Optional player reference. If empty, it is found at runtime by tag.")]
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Ranges")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;

    [Header("Attack")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;

    private EnemyState currentState = EnemyState.Idle;
    private PlayerHealth playerHealth;
    private float nextAttackTime;

    private void Awake()
    {
        FindPlayerReferences();
    }

    private void Update()
    {
        // Retry finding player if references are missing.
        if (player == null || playerHealth == null)
        {
            FindPlayerReferences();
        }

        // If player still not found, stay idle safely.
        if (player == null || playerHealth == null)
        {
            currentState = EnemyState.Idle;
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        UpdateState(distanceToPlayer);
        RunState(distanceToPlayer);
    }

    /// <summary>
    /// Finds player Transform and PlayerHealth using Player tag.
    /// </summary>
    private void FindPlayerReferences()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject == null)
        {
            player = null;
            playerHealth = null;
            return;
        }

        player = playerObject.transform;
        playerHealth = playerObject.GetComponent<PlayerHealth>();
    }

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

    private void RunState(float distanceToPlayer)
    {
        switch (currentState)
        {
            case EnemyState.Idle:
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
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.forward = direction;
        }
    }

    private void AttackPlayer(float distanceToPlayer)
    {
        // Face player while attacking.
        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude > 0.0001f)
        {
            transform.forward = lookDirection.normalized;
        }

        // Prevent attack spamming.
        if (Time.time < nextAttackTime)
        {
            return;
        }

        playerHealth.TakeDamage(attackDamage);
        nextAttackTime = Time.time + attackCooldown;

        Debug.Log($"{gameObject.name} attacked player for {attackDamage} damage at distance {distanceToPlayer:F2}");
    }
}
