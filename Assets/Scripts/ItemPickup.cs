using UnityEngine;

/// <summary>
/// Item pickup that applies ItemData stat bonuses to the player.
/// Uses trigger collision and destroys itself after pickup.
/// </summary>
public class ItemPickup : MonoBehaviour
{
    [Tooltip("Item data that defines bonuses applied on pickup.")]
    [SerializeField] private ItemData itemData;

    [Tooltip("Renderer used to show item type color. If empty, auto-finds one.")]
    [SerializeField] private Renderer targetRenderer;

    private void Awake()
    {
        EnsureRendererReference();
        ApplyColorFromItemType();
    }

    private void OnValidate()
    {
        EnsureRendererReference();
        ApplyColorFromItemType();
    }

    public void SetItemData(ItemData data)
    {
        itemData = data;
        ApplyColorFromItemType();
    }

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

    private void EnsureRendererReference()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<Renderer>();
        }
    }

    private void ApplyColorFromItemType()
    {
        if (itemData == null || targetRenderer == null)
        {
            return;
        }

        Color color = Color.white;

        switch (itemData.itemType)
        {
            case ItemData.ItemType.Damage:
                color = Color.red;
                break;

            case ItemData.ItemType.Magic:
                color = Color.blue;
                break;

            case ItemData.ItemType.Speed:
                color = Color.green;
                break;
        }

        // Use material instance color for clear visual feedback.
        targetRenderer.material.color = color;
    }
}
