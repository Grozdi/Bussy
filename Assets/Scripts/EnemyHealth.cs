using System.Collections.Generic;
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

    [Header("Item Drop")]
    [Tooltip("Pickup prefab spawned when an item drop succeeds.")]
    [SerializeField] private GameObject itemPickupPrefab;

    [Tooltip("Possible items this enemy can drop.")]
    [SerializeField] private List<ItemData> possibleItemDrops = new List<ItemData>();

    [Tooltip("Chance (0 to 1) to drop an item on death.")]
    [Range(0f, 1f)]
    [SerializeField] private float itemDropChance = 0.25f;

    // Prevents double death handling/reward in edge cases.
    private bool isDead;

    /// <summary>
    /// Applies damage to this enemy.
    /// </summary>
    public void TakeDamage(float amount)
    {
        if (isDead)
        {
            return;
        }

        if (amount <= 0f)
        {
            return;
        }

        health -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining health: {health}");

        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        RewardGold();
        TryDropItem();

        Destroy(gameObject);
    }

    private void RewardGold()
    {
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
    }

    private void TryDropItem()
    {
        if (itemPickupPrefab == null)
        {
            return;
        }

        if (possibleItemDrops == null || possibleItemDrops.Count == 0)
        {
            return;
        }

        if (Random.value > itemDropChance)
        {
            return;
        }

        ItemData dropItem = possibleItemDrops[Random.Range(0, possibleItemDrops.Count)];
        if (dropItem == null)
        {
            return;
        }

        GameObject pickupObject = Instantiate(itemPickupPrefab, transform.position, Quaternion.identity);
        ItemPickup pickup = pickupObject.GetComponent<ItemPickup>();
        if (pickup != null)
        {
            pickup.SetItemData(dropItem);
        }

        Debug.Log($"{gameObject.name} dropped item: {dropItem.itemName}");
    }

    private PlayerStats FindPlayerStats()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            PlayerStats statsOnPlayer = playerObject.GetComponent<PlayerStats>();
            if (statsOnPlayer != null)
            {
                return statsOnPlayer;
            }
        }

        return FindFirstObjectByType<PlayerStats>();
    }
}
