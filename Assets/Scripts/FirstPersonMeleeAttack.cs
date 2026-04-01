using UnityEngine;

/// <summary>
/// Simple first-person melee attack system.
/// Uses a camera-forward raycast to detect and damage enemies.
/// </summary>
public class FirstPersonMeleeAttack : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Transform of the player's camera. If not assigned, Camera.main.transform is used when available.")]
    [SerializeField] private Transform playerCamera;

    [Header("Attack Settings")]
    [Tooltip("Maximum distance the melee attack can reach.")]
    [SerializeField] private float attackRange = 2f;

    [Tooltip("Damage dealt per successful hit.")]
    [SerializeField] private float damage = 25f;

    [Tooltip("Cooldown time in seconds between attacks.")]
    [SerializeField] private float attackCooldown = 0.5f;

    [Tooltip("Color of the debug ray shown in the Scene view.")]
    [SerializeField] private Color debugRayColor = Color.red;

    // Time when the next attack is allowed.
    private float nextAttackTime;

    private void Awake()
    {
        AssignCameraIfMissing();
    }

    private void Update()
    {
        // Trigger attack on left mouse click.
        if (Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }
    }

    /// <summary>
    /// Attempts to perform an attack if cooldown allows it.
    /// </summary>
    private void TryAttack()
    {
        // Prevent attacking during cooldown.
        if (Time.time < nextAttackTime)
        {
            Debug.Log("Attack blocked: cooldown active.");
            return;
        }

        // Keep script robust if camera gets lost/unassigned at runtime.
        if (!AssignCameraIfMissing())
        {
            Debug.LogWarning("Attack failed: player camera Transform is not assigned.");
            return;
        }

        // Set next allowed attack time once we know we can attempt an attack.
        nextAttackTime = Time.time + attackCooldown;

        // Ensure the ray starts from camera position and goes in camera forward direction.
        Vector3 rayOrigin = playerCamera.position;
        Vector3 rayDirection = playerCamera.forward;

        // Visualize the attack ray in the Scene view.
        Debug.DrawRay(rayOrigin, rayDirection * attackRange, debugRayColor, 0.15f);

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, attackRange))
        {
            // Check if the hit object has EnemyHealth.
            EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log($"Melee hit: {hit.collider.name} for {damage} damage.");
            }
            else
            {
                Debug.Log($"Melee hit: {hit.collider.name}, but no EnemyHealth component found.");
            }
        }
        else
        {
            Debug.Log("Melee miss: no target in range.");
        }
    }

    /// <summary>
    /// Assigns camera transform from Camera.main when missing.
    /// Returns true when a valid transform is available.
    /// </summary>
    private bool AssignCameraIfMissing()
    {
        if (playerCamera != null)
        {
            return true;
        }

        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            return false;
        }

        playerCamera = mainCamera.transform;
        return true;
    }

    /// <summary>
    /// Sets melee damage value (used by upgrade systems).
    /// </summary>
    public void SetDamage(float value)
    {
        damage = Mathf.Max(0f, value);
    }

    /// <summary>
    /// Returns current melee damage value.
    /// </summary>
    public float GetDamage()
    {
        return damage;
    }
}
