using UnityEngine;

/// <summary>
/// Item pickup that applies ItemData stat bonuses to the player.
/// Uses trigger collision and destroys itself after pickup.
/// </summary>
public class ItemPickup : MonoBehaviour
{
    [Tooltip("Item data that defines bonuses applied on pickup.")]
    [SerializeField] private ItemData itemData;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (itemData == null)
        {
            Debug.LogWarning("ItemPickup failed: itemData is not assigned.");
            return;
        }

        // Centralized stat application via PlayerStats keeps structure modular.
        PlayerStats playerStats = other.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.ApplyItemBonuses(itemData);
            Debug.Log($"Picked up item: {itemData.itemName}");
            Destroy(gameObject);
            return;
        }

        Debug.LogWarning("ItemPickup failed: PlayerStats not found on player.");
    }
}
