using UnityEngine;

/// <summary>
/// Simple health component for an enemy.
/// Attach this to any enemy GameObject.
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    [Tooltip("Current enemy health.")]
    public float health = 50f;

    /// <summary>
    /// Applies damage to this enemy.
    /// </summary>
    /// <param name="amount">Damage amount to subtract from health.</param>
    public void TakeDamage(float amount)
    {
        // Ignore non-positive damage values.
        if (amount <= 0f)
        {
            return;
        }

        // Reduce health by incoming damage.
        health -= amount;

        // Log damage and updated health for debugging.
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining health: {health}");

        // If health is zero or less, destroy this enemy.
        if (health <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
