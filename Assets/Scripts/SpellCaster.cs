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

    private float nextCastTime;

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

        nextCastTime = Time.time + castCooldown;

        Vector3 direction = playerCamera.forward;
        Quaternion spawnRotation = Quaternion.LookRotation(direction, Vector3.up);
        GameObject projectileObject = Instantiate(projectilePrefab, spawnPoint.position, spawnRotation);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Fire(direction, projectileSpeed, projectileDamage, Projectile.DamageTarget.Enemy);
        }

        Debug.Log($"Spell cast: spawned {projectileObject.name} from {spawnPoint.position}.");
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
}
