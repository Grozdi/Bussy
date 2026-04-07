using UnityEngine;

/// <summary>
/// Simple spell casting system.
/// Casts projectile prefabs from a spawn point on right mouse click.
/// Supports single-shot and spread multi-shot.
/// </summary>
public class SpellCaster : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject alternateProjectilePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform playerCamera;

    [Header("Cast Settings")]
    [SerializeField] private float castCooldown = 0.75f;
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private float projectileDamage = 15f;

    [Header("Projectile Pattern")]
    [Min(1)]
    [SerializeField] private int projectileCount = 1;
    [SerializeField] private float spreadAngle = 12f;

    private float nextCastTime;
    private ProjectileAttackModifier activeModifier;

    private void Awake()
    {
        EnsureSpawnPoint();
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

        EnsureSpawnPoint();
        if (spawnPoint == null)
        {
            Debug.LogWarning("Cast failed: no valid spawnPoint or player transform found.");
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

        // Ensure at least one projectile always spawns.
        int shotCount = Mathf.Max(1, projectileCount);

        Vector3 baseDirection = playerCamera.forward;
        if (baseDirection.sqrMagnitude <= 0.0001f)
        {
            baseDirection = transform.forward;
        }

        float clampedSpread = Mathf.Max(0f, spreadAngle);
        float angleStep = shotCount > 1 ? clampedSpread / (shotCount - 1) : 0f;
        float startAngle = shotCount > 1 ? -clampedSpread * 0.5f : 0f;

        for (int i = 0; i < shotCount; i++)
        {
            float angle = startAngle + (i * angleStep);

            Quaternion rotationOffset = Quaternion.Euler(0f, angle, 0f);
            Vector3 direction = rotationOffset * baseDirection;

            if (float.IsNaN(direction.x) || float.IsNaN(direction.y) || float.IsNaN(direction.z) || direction.sqrMagnitude <= 0.0001f)
            {
                direction = baseDirection;
            }

            SpawnProjectile(prefabToUse, direction.normalized);
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

    private GameObject GetProjectilePrefabForCurrentCast()
    {
        if (alternateProjectilePrefab != null)
        {
            return alternateProjectilePrefab;
        }

        if (activeModifier != null && activeModifier.overrideProjectilePrefab != null)
        {
            return activeModifier.overrideProjectilePrefab;
        }

        return projectilePrefab;
    }

    private void EnsureSpawnPoint()
    {
        if (spawnPoint != null)
        {
            return;
        }

        // Safe fallback: use player's transform when no spawn point is assigned.
        spawnPoint = transform;
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

    // Compatibility helper for existing systems.
    public void SetMultiShot(bool enabled, int shotCount = 3)
    {
        projectileCount = enabled ? Mathf.Max(2, shotCount) : 1;
    }

    public void SetAlternateProjectilePrefab(GameObject prefab)
    {
        alternateProjectilePrefab = prefab;
    }

    public void SetProjectilePattern(int count, float spread)
    {
        projectileCount = Mathf.Max(1, count);
        spreadAngle = Mathf.Max(0f, spread);
    }

    public void SetAttackModifier(ProjectileAttackModifier modifier)
    {
        activeModifier = modifier;

        if (activeModifier != null)
        {
            projectileCount = Mathf.Max(1, 1 + activeModifier.additionalProjectiles);
            spreadAngle = activeModifier.spreadAngle;
        }
    }
}
