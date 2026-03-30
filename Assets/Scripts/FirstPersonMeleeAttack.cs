using UnityEngine;

/// <summary>
/// Simple first-person melee attack system.
/// Uses a camera-forward raycast to detect and damage enemies.
/// </summary>
public class FirstPersonMeleeAttack : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Camera used to fire the melee raycast. If not assigned, Camera.main is used.")]
    [SerializeField] private Camera playerCamera;

    [Header("Attack Settings")]
    [Tooltip("Maximum distance the melee attack can reach.")]
    [SerializeField] private float attackRange = 2f;

    [Tooltip("Damage dealt per successful hit.")]
    [SerializeField] private float damage = 25f;

    [Tooltip("Cooldown time in seconds between attacks.")]
    [SerializeField] private float attackCooldown = 0.5f;

    // Time when the next attack is allowed.
    private float nextAttackTime;

    private void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
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

        // Set next allowed attack time immediately.
        nextAttackTime = Time.time + attackCooldown;

        if (playerCamera == null)
        {
            Debug.LogWarning("Attack failed: no player camera assigned.");
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        // Raycast forward from camera.
        if (Physics.Raycast(ray, out RaycastHit hit, attackRange))
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
}
