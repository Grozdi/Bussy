using UnityEngine;

/// <summary>
/// Lightweight bat AI with simple behavior:
/// - Chase fast when player moves fast or jumps
/// - Otherwise hover/move slowly
/// </summary>
public class BatEnemyAI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Optional player transform. If empty, found using Player tag.")]
    [SerializeField] private Transform player;

    [Header("Player Movement Triggers")]
    [Tooltip("Estimated player horizontal speed needed to trigger fast chase.")]
    [SerializeField] private float playerSpeedThreshold = 5f;

    [Tooltip("Estimated upward player speed needed to trigger fast chase (jump).")]
    [SerializeField] private float playerVerticalVelocityThreshold = 1.2f;

    [Header("Bat Movement")]
    [Tooltip("Movement speed when not aggressive.")]
    [SerializeField] private float slowMoveSpeed = 1.5f;

    [Tooltip("Movement speed when aggressive.")]
    [SerializeField] private float fastChaseSpeed = 5f;

    [Tooltip("Idle hover height offset from start position.")]
    [SerializeField] private float hoverAmplitude = 0.5f;

    [Tooltip("Idle hover frequency.")]
    [SerializeField] private float hoverFrequency = 2f;

    [Header("Attack")]
    [Tooltip("Distance at which bat can attack player.")]
    [SerializeField] private float attackRange = 1.5f;

    [Tooltip("Damage dealt per hit.")]
    [SerializeField] private float attackDamage = 8f;

    [Tooltip("Cooldown between attacks.")]
    [SerializeField] private float attackCooldown = 1f;

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
            RunSlowHover();
            return;
        }

        float playerSpeed;
        float playerVerticalVelocity;
        EstimatePlayerMovement(out playerSpeed, out playerVerticalVelocity);

        bool shouldChaseFast = playerSpeed > playerSpeedThreshold || playerVerticalVelocity > playerVerticalVelocityThreshold;

        if (shouldChaseFast)
        {
            RunFastChaseAndAttack();
        }
        else
        {
            RunSlowHover();
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

    private void EstimatePlayerMovement(out float horizontalSpeed, out float verticalVelocity)
    {
        horizontalSpeed = 0f;
        verticalVelocity = 0f;

        if (!hasLastPlayerPosition)
        {
            lastPlayerPosition = player.position;
            hasLastPlayerPosition = true;
            return;
        }

        Vector3 delta = player.position - lastPlayerPosition;
        float dt = Mathf.Max(Time.deltaTime, 0.0001f);

        horizontalSpeed = new Vector2(delta.x, delta.z).magnitude / dt;
        verticalVelocity = delta.y / dt;

        lastPlayerPosition = player.position;
    }

    private void RunSlowHover()
    {
        // Hover gently around initial position.
        float hoverOffset = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
        Vector3 target = startPosition + Vector3.up * hoverOffset;

        transform.position = Vector3.MoveTowards(transform.position, target, slowMoveSpeed * Time.deltaTime);
    }

    private void RunFastChaseAndAttack()
    {
        // Chase directly toward player at fast speed.
        Vector3 toPlayer = player.position - transform.position;
        Vector3 direction = toPlayer.normalized;

        transform.position += direction * fastChaseSpeed * Time.deltaTime;

        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.forward = direction;
        }

        // Attack when close and cooldown is ready.
        float distanceToPlayer = toPlayer.magnitude;
        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            playerHealth.TakeDamage(attackDamage);
            nextAttackTime = Time.time + attackCooldown;
        }
    }
}
