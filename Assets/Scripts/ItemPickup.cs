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
        ApplyColorFromItemType(useRuntimeMaterial: true);
    }

    private void OnValidate()
    {
        // Editor-safe path:
        // - avoid touching prefab asset materials
        // - use sharedMaterial (not material)
        if (IsPrefabAsset())
        {
            return;
        }

        EnsureRendererReference();
        ApplyColorFromItemType(useRuntimeMaterial: false);
    }

    public void SetItemData(ItemData data)
    {
        if (data == null)
        {
            Debug.LogWarning("ItemPickup SetItemData failed: data is null.");
            return;
        }

        itemData = data;
        EnsureRendererReference();
        ApplyColorFromItemType(useRuntimeMaterial: Application.isPlaying);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null || !other.CompareTag("Player"))
        {
            return;
        }

        if (itemData == null)
        {
            Debug.LogWarning("ItemPickup failed: itemData is not assigned.");
            return;
        }

        PlayerStats playerStats = other.GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogWarning("ItemPickup failed: PlayerStats not found on player.");
            return;
        }

        playerStats.ApplyItemBonuses(itemData);
        Debug.Log($"Picked up item: {itemData.itemName}");
        Destroy(gameObject);
    }

    private void EnsureRendererReference()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<Renderer>();
        }
    }

    private void ApplyColorFromItemType(bool useRuntimeMaterial)
    {
        if (itemData == null)
        {
            return;
        }

        EnsureRendererReference();
        if (targetRenderer == null)
        {
            return;
        }

        Color color;
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
            default:
                color = Color.white;
                break;
        }

        if (useRuntimeMaterial)
        {
            // Runtime-only instance modification.
            if (targetRenderer.material != null)
            {
                targetRenderer.material.color = color;
            }
        }
        else
        {
            // Editor-safe shared material update.
            if (targetRenderer.sharedMaterial != null)
            {
                targetRenderer.sharedMaterial.color = color;
            }
        }
    }

    private bool IsPrefabAsset()
    {
#if UNITY_EDITOR
        return UnityEditor.PrefabUtility.IsPartOfPrefabAsset(gameObject);
#else
        return false;
#endif
    }
}
