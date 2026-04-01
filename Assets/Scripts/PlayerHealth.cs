using UnityEngine;

/// <summary>
/// Simple player health component.
/// Tracks health and handles player death behavior.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Tooltip("Current player health.")]
    public float health = 100f;

    [Header("Scripts To Disable On Death")]
    [SerializeField] private FirstPersonController firstPersonController;
    [SerializeField] private FirstPersonMeleeAttack firstPersonMeleeAttack;
    [SerializeField] private SpellCaster spellCaster;

    private bool isDead;

    private void Awake()
    {
        // Optional auto-wiring for common setup.
        if (firstPersonController == null)
        {
            firstPersonController = GetComponent<FirstPersonController>();
        }

        if (firstPersonMeleeAttack == null)
        {
            firstPersonMeleeAttack = GetComponent<FirstPersonMeleeAttack>();
        }

        if (spellCaster == null)
        {
            spellCaster = GetComponent<SpellCaster>();
        }
    }

    /// <summary>
    /// Applies damage to the player.
    /// </summary>
    /// <param name="amount">Damage amount to subtract from health.</param>
    public void TakeDamage(float amount)
    {
        // Ignore invalid damage values or repeated damage after death.
        if (amount <= 0f || isDead)
        {
            return;
        }

        // Reduce health by incoming damage.
        health -= amount;

        // Log damage and current health.
        Debug.Log($"Player took {amount} damage. Remaining health: {health}");

        // Detect death when health reaches zero or below.
        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        health = 0f;

        Debug.Log("Player died");

        // Disable movement and attack scripts to prevent further input.
        if (firstPersonController != null)
        {
            firstPersonController.enabled = false;
        }

        if (firstPersonMeleeAttack != null)
        {
            firstPersonMeleeAttack.enabled = false;
        }

        if (spellCaster != null)
        {
            spellCaster.enabled = false;
        }
    }
}
