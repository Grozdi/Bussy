using UnityEngine;

/// <summary>
/// Lightweight bat enemy AI with two states:
/// - Idle: hover/move slowly
/// - Aggressive: chase and attack player
///
/// Bat becomes aggressive when the player is close AND
/// either moving fast or jumping.
/// </summary>
public class BatEnemyAI : MonoBehaviour
{
    private enum BatState
    {
        Idle,
        Aggressive
    }

    [Header("References")]
    [Tooltip("Optional player transform. If empty, found using Player tag.")]
    [SerializeField] private Transform player;

    [Header("Detection")]
    [Tooltip("Distance at which bat starts evaluating player behavior.")]
    [SerializeField] private float detectionRange = 12f;

    [Tooltip("Estimated player horizontal speed needed to trigger aggression.")]
    [SerializeField] private float playerFastMoveThreshold = 5f;

    [Tooltip("Estimated upward speed considered a jump.")]
    [SerializeField] private float jumpVelocityThreshold = 1.2f;

    [Header("Movement")]
    [Tooltip("Slow movement speed while idle.")]
    [SerializeField] private float idleMoveSpeed = 1.5f;

    [Tooltip("Chase speed while aggressive.")]
    [SerializeField] private float aggressiveMoveSpeed = 5f;

    [Tooltip("Hover height offset from initial position.")]
    [SerializeField] private float hoverAmplitude = 0.5f;

    [Tooltip("Hover speed while idle.")]
    [SerializeField] private float hoverFrequency = 2f;

    [Header("Attack")]
    [Tooltip("Distance at which bat can attack the player.")]
    [SerializeField] private float attackRange = 1.5f;

    [Tooltip("Damage dealt per attack.")]
    [SerializeField] private float attackDamage = 8f;

    [Tooltip("Cooldown time between attacks.")]
    [SerializeField] private float attackCooldown = 1f;

    private BatState currentState = BatState.Idle;
    private PlayerHealth playerHealth;

    private Vector3 startPosition;
    private Vector3 lastPlayerPosition;
    private bool hasLastPlayerPosition;
    private float nextAttackTime;

    private void Awake()
    {
        startPosition = transform.position;
        ResolvePlayerReferences();
    }

    private void Update()
    {
        if (!ResolvePlayerReferences())
        {
            RunIdleHover();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float estimatedPlayerHorizontalSpeed;
        float estimatedPlayerVerticalSpeed;
        EstimatePlayerMovement(out estimatedPlayerHorizontalSpeed, out estimatedPlayerVerticalSpeed);

        UpdateState(distanceToPlayer, estimatedPlayerHorizontalSpeed, estimatedPlayerVerticalSpeed);

        if (currentState == BatState.Aggressive)
        {
            RunAggressive(distanceToPlayer);
        }
        else
        {
            RunIdleHover();
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
                hasLastPlayerPosition = false;
            }
        }

        if (player != null && playerHealth == null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        return player != null && playerHealth != null;
    }

    private void EstimatePlayerMovement(out float horizontalSpeed, out float verticalSpeed)
    {
        horizontalSpeed = 0f;
        verticalSpeed = 0f;

        if (!hasLastPlayerPosition)
        {
            lastPlayerPosition = player.position;
            hasLastPlayerPosition = true;
            return;
        }

        Vector3 delta = player.position - lastPlayerPosition;
        float deltaTime = Mathf.Max(Time.deltaTime, 0.0001f);

        horizontalSpeed = new Vector2(delta.x, delta.z).magnitude / deltaTime;
        verticalSpeed = delta.y / deltaTime;

        lastPlayerPosition = player.position;
    }

    private void UpdateState(float distanceToPlayer, float playerHorizontalSpeed, float playerVerticalSpeed)
    {
        bool playerInRange = distanceToPlayer <= detectionRange;
        bool playerMovingFast = playerHorizontalSpeed >= playerFastMoveThreshold;
        bool playerJumped = playerVerticalSpeed >= jumpVelocityThreshold;

        currentState = (playerInRange && (playerMovingFast || playerJumped))
            ? BatState.Aggressive
            : BatState.Idle;
    }

    private void RunIdleHover()
    {
        // Hover around the initial position with a smooth sine wave.
        float hoverOffset = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
        Vector3 target = startPosition + Vector3.up * hoverOffset;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            idleMoveSpeed * Time.deltaTime);
    }

    private void RunAggressive(float distanceToPlayer)
    {
        // Chase toward player.
        Vector3 toPlayer = (player.position - transform.position).normalized;
        transform.position += toPlayer * aggressiveMoveSpeed * Time.deltaTime;

        if (toPlayer.sqrMagnitude > 0.0001f)
        {
            transform.forward = toPlayer;
        }

        // Attack when close enough and cooldown is ready.
        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            playerHealth.TakeDamage(attackDamage);
            nextAttackTime = Time.time + attackCooldown;
        }
    }
}
