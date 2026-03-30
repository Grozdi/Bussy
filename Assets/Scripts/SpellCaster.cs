using UnityEngine;

/// <summary>
/// Simple spell casting system.
/// Casts a projectile prefab from a spawn point on right mouse click.
/// </summary>
public class SpellCaster : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Projectile prefab to spawn when casting.")]
    [SerializeField] private GameObject projectilePrefab;

    [Tooltip("Spawn point used for projectile position and direction.")]
    [SerializeField] private Transform spawnPoint;

    [Header("Cast Settings")]
    [Tooltip("Cooldown time in seconds between casts.")]
    [SerializeField] private float castCooldown = 0.75f;

    [Tooltip("Speed applied to the spawned projectile.")]
    [SerializeField] private float projectileSpeed = 20f;

    [Tooltip("Damage assigned to the spawned projectile.")]
    [SerializeField] private float projectileDamage = 15f;

    // Time when the next cast is allowed.
    private float nextCastTime;

    private void Update()
    {
        // Trigger spell cast on right mouse click.
        if (Input.GetMouseButtonDown(1))
        {
            TryCastSpell();
        }
    }

    /// <summary>
    /// Attempts to cast a spell if cooldown and references are valid.
    /// </summary>
    private void TryCastSpell()
    {
        // Prevent casting during cooldown.
        if (Time.time < nextCastTime)
        {
            Debug.Log("Cast blocked: cooldown active.");
            return;
        }

        if (projectilePrefab == null)
        {
            Debug.LogWarning("Cast failed: projectilePrefab is not assigned.");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("Cast failed: spawnPoint is not assigned.");
            return;
        }

        // Set the next time casting is allowed.
        nextCastTime = Time.time + castCooldown;

        // Spawn projectile at spawn point.
        GameObject projectileObject = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);

        // If the prefab has our Projectile script, set tunable values.
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.speed = projectileSpeed;
            projectile.damage = projectileDamage;
        }

        // Apply forward movement using Rigidbody when available.
        Rigidbody rb = projectileObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = spawnPoint.forward * projectileSpeed;
        }

        Debug.Log($"Spell cast: spawned {projectileObject.name} at speed {projectileSpeed}, damage {projectileDamage}.");
    }
}
