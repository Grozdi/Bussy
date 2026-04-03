using UnityEngine;

/// <summary>
/// Simple health component for an enemy.
/// Attach this to any enemy GameObject.
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    [Tooltip("Current enemy health.")]
    public float health = 50f;

    [Tooltip("Gold given to the player when this enemy dies.")]
    public int goldReward = 10;

    // Prevents double death handling/reward in edge cases.
    private bool isDead;

    /// <summary>
    /// Applies damage to this enemy.
    /// </summary>
    /// <param name="amount">Damage amount to subtract from health.</param>
    public void TakeDamage(float amount)
    {
        if (isDead)
        {
            return;
        }

        // Ignore non-positive damage values.
        if (amount <= 0f)
        {
            return;
        }

        // Reduce health by incoming damage.
        health -= amount;

        // Log damage and updated health for debugging.
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining health: {health}");

        // If health is zero or less, handle death.
        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        // Try to find PlayerStats safely and reward gold.
        PlayerStats playerStats = FindPlayerStats();
        if (playerStats != null)
        {
            playerStats.AddGold(goldReward);
            Debug.Log($"{gameObject.name} died. Awarded {goldReward} gold.");
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} died, but no PlayerStats was found to receive {goldReward} gold.");
        }

        Destroy(gameObject);
    }

    private PlayerStats FindPlayerStats()
    {
        // Preferred: find Player by tag and get PlayerStats.
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            PlayerStats statsOnPlayer = playerObject.GetComponent<PlayerStats>();
            if (statsOnPlayer != null)
            {
                return statsOnPlayer;
            }
        }

        // Fallback: find any PlayerStats instance in scene.
        return FindFirstObjectByType<PlayerStats>();
    }
}
