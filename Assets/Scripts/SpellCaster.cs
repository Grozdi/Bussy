using UnityEngine;

/// <summary>
/// Simple spell casting system.
/// Casts a projectile prefab from a spawn point on right mouse click.
/// Supports optional projectile attack modifiers.
/// </summary>
public class SpellCaster : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Projectile prefab to spawn when casting.")]
    [SerializeField] private GameObject projectilePrefab;

    [Tooltip("Alternate projectile prefab used for multi-shot if assigned.")]
    [SerializeField] private GameObject alternateProjectilePrefab;

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

    [Header("Multi-Shot")]
    [Tooltip("If true, fires multiple projectiles per cast.")]
    [SerializeField] private bool multiShotEnabled;

    [Tooltip("Projectile count used when multi-shot is enabled.")]
    [Min(2)]
    [SerializeField] private int multiShotCount = 3;

    private float nextCastTime;
    private ProjectileAttackModifier activeModifier;

    private void Awake()
    {
        AssignCameraIfMissing();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            TryCastSpell();
        }
    }

    private void TryCastSpell()
    {
        if (Time.time < nextCastTime)
        {
            Debug.Log("Cast blocked: cooldown active.");
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

        GameObject prefabToUse = GetProjectilePrefabForCurrentCast();
        if (prefabToUse == null)
        {
            Debug.LogWarning("Cast failed: projectile prefab is not assigned.");
            return;
        }

        nextCastTime = Time.time + castCooldown;

        Vector3 baseDirection = playerCamera.forward;
        int shotCount = GetShotCount();
        float spread = GetSpreadAngle();

        for (int i = 0; i < shotCount; i++)
        {
            Vector3 shotDirection = GetSpreadDirection(baseDirection, i, shotCount, spread);
            SpawnProjectile(prefabToUse, shotDirection);
        }

        Debug.Log($"Spell cast: fired {shotCount} projectile(s).");
    }

    private void SpawnProjectile(GameObject prefab, Vector3 direction)
    {
        Quaternion spawnRotation = Quaternion.LookRotation(direction, Vector3.up);
        GameObject projectileObject = Instantiate(prefab, spawnPoint.position, spawnRotation);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Fire(direction, projectileSpeed, projectileDamage, Projectile.DamageTarget.Enemy);
        }
    }

    private Vector3 GetSpreadDirection(Vector3 baseDirection, int shotIndex, int shotCount, float spreadAngle)
    {
        if (shotCount <= 1 || spreadAngle <= 0f)
        {
            return baseDirection;
        }

        float t = (float)shotIndex / (shotCount - 1);
        float angle = Mathf.Lerp(-spreadAngle * 0.5f, spreadAngle * 0.5f, t);
        return Quaternion.AngleAxis(angle, Vector3.up) * baseDirection;
    }

    private GameObject GetProjectilePrefabForCurrentCast()
    {
        // Multi-shot override first (explicit gameplay toggle).
        if (multiShotEnabled && alternateProjectilePrefab != null)
        {
            return alternateProjectilePrefab;
        }

        // Fallback to active modifier override.
        if (activeModifier != null && activeModifier.overrideProjectilePrefab != null)
        {
            return activeModifier.overrideProjectilePrefab;
        }

        // Default prefab.
        return projectilePrefab;
    }

    private int GetShotCount()
    {
        if (multiShotEnabled)
        {
            return Mathf.Max(2, multiShotCount);
        }

        int extra = activeModifier != null ? activeModifier.additionalProjectiles : 0;
        return Mathf.Max(1, 1 + extra);
    }

    private float GetSpreadAngle()
    {
        if (multiShotEnabled)
        {
            return 12f;
        }

        return activeModifier != null ? activeModifier.spreadAngle : 0f;
    }

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

    public void SetSpellDamage(float value)
    {
        projectileDamage = Mathf.Max(0f, value);
    }

    public float GetSpellDamage()
    {
        return projectileDamage;
    }

    /// <summary>
    /// Sets the active projectile attack modifier (or null to clear).
    /// </summary>
    public void SetAttackModifier(ProjectileAttackModifier modifier)
    {
        activeModifier = modifier;
    }
}
