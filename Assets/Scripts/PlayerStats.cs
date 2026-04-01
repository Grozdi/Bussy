using UnityEngine;

/// <summary>
/// Simple player stats component.
/// Currently tracks only gold.
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [Tooltip("Current amount of gold the player has.")]
    public int gold = 0;

    /// <summary>
    /// Adds gold to the player's total and logs the updated value.
    /// </summary>
    /// <param name="amount">Amount of gold to add.</param>
    public void AddGold(int amount)
    {
        // Ignore zero or negative additions to keep this method simple.
        if (amount <= 0)
        {
            return;
        }

        gold += amount;
        Debug.Log($"Player gold is now: {gold}");
    }
}
