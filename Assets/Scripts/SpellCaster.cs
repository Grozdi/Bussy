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

    [Tooltip("Spawn point used for projectile position.")]
    [SerializeField] private Transform spawnPoint;

    [Tooltip("Player camera used to define projectile forward direction.")]
    [SerializeField] private Transform playerCamera;

    [Header("Cast Settings")]
    [Tooltip("Cooldown time in seconds between casts.")]
    [SerializeField] private float castCooldown = 0.75f;

    [Tooltip("Speed applied to the spawned projectile.")]
    [SerializeField] private float projectileSpeed = 20f;

    [Tooltip("Damage assigned to the spawned projectile.")]
    [SerializeField] private float projectileDamage = 15f;

    // Time when the next cast is allowed.
    private float nextCastTime;

    private void Awake()
    {
        AssignCameraIfMissing();
    }

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

        if (!AssignCameraIfMissing())
        {
            Debug.LogWarning("Cast failed: playerCamera is not assigned and Camera.main was not found.");
            return;
        }

        // Set the next time casting is allowed.
        nextCastTime = Time.time + castCooldown;

        // Spawn at the spawn point position, but face camera forward direction.
        Quaternion spawnRotation = Quaternion.LookRotation(playerCamera.forward, Vector3.up);
        GameObject projectileObject = Instantiate(projectilePrefab, spawnPoint.position, spawnRotation);

        // If the prefab has our Projectile script, set tunable values.
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.speed = projectileSpeed;
            projectile.damage = projectileDamage;
        }

        // Apply forward movement in camera forward direction using Rigidbody when available.
        Rigidbody rb = projectileObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = playerCamera.forward * projectileSpeed;
        }

        Debug.Log($"Spell cast: spawned {projectileObject.name} from {spawnPoint.position} toward camera forward.");
    }

    /// <summary>
    /// Assigns player camera from Camera.main when missing.
    /// Returns true when a valid camera transform is available.
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
    /// Sets projectile damage value (used by upgrade systems).
    /// </summary>
    public void SetSpellDamage(float value)
    {
        projectileDamage = Mathf.Max(0f, value);
    }

    /// <summary>
    /// Returns current projectile damage value.
    /// </summary>
    public float GetSpellDamage()
    {
        return projectileDamage;
    }
}
