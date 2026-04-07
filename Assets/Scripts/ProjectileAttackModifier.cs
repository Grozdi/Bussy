using UnityEngine;

/// <summary>
/// Simple modifier for projectile-based attacks.
/// Can change projectile prefab and fire extra projectiles.
/// </summary>
[CreateAssetMenu(fileName = "NewProjectileAttackModifier", menuName = "Game/Projectile Attack Modifier")]
public class ProjectileAttackModifier : ScriptableObject
{
    [Tooltip("If assigned, replaces the default projectile prefab while modifier is active.")]
    public GameObject overrideProjectilePrefab;

    [Tooltip("Number of additional projectiles to fire (0 = normal single shot).")]
    [Min(0)]
    public int additionalProjectiles = 0;

    [Tooltip("Total spread angle used for multi-shot (degrees).")]
    [Min(0f)]
    public float spreadAngle = 10f;
}
