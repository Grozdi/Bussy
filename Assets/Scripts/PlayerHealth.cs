using UnityEngine;

/// <summary>
/// Simple player health component.
/// Tracks health and logs damage/death events.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Tooltip("Current player health.")]
    public float health = 100f;

    /// <summary>
    /// Applies damage to the player.
    /// </summary>
    /// <param name="amount">Damage amount to subtract from health.</param>
    public void TakeDamage(float amount)
    {
        // Ignore invalid damage values.
        if (amount <= 0f)
        {
            return;
        }

        // Reduce health by incoming damage.
        health -= amount;

        // Log damage and current health.
        Debug.Log($"Player took {amount} damage. Remaining health: {health}");

        // Log death when health reaches zero or below.
        if (health <= 0f)
        {
            Debug.Log("Player died.");
        }
    }
}
