using UnityEngine;

/// <summary>
/// Item pickup that applies ItemData stat bonuses to the player.
/// Separates editor visual refresh from runtime pickup logic.
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
    }

    private void Start()
    {
        // Runtime visual update only.
        ApplyVisualRuntime();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Editor visual update only (safe path).
        if (Application.isPlaying)
        {
            return;
        }

        if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(gameObject))
        {
            return;
        }

        EnsureRendererReference();
        ApplyVisualEditor();
    }
#endif

    public void SetItemData(ItemData data)
    {
        if (data == null)
        {
            Debug.LogWarning("ItemPickup SetItemData failed: data is null.");
            return;
        }

        itemData = data;
        EnsureRendererReference();

        // Keep visuals updated in both editor and runtime contexts.
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            ApplyVisualEditor();
            return;
        }
#endif
        ApplyVisualRuntime();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null)
        {
            return;
        }

        if (!other.CompareTag("Player"))
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

        // Directly modify attack behavior when item has projectile modifier data.
        if (itemData.projectileAttackModifier != null)
        {
            SpellCaster spellCaster = other.GetComponent<SpellCaster>();
            if (spellCaster != null)
            {
                int shotCount = 1 + itemData.projectileAttackModifier.additionalProjectiles;
                bool enableMultiShot = itemData.projectileAttackModifier.additionalProjectiles > 0;

                spellCaster.SetMultiShot(enableMultiShot, shotCount);
                spellCaster.SetAlternateProjectilePrefab(itemData.projectileAttackModifier.overrideProjectilePrefab);
            }
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

    private bool TryGetItemColor(out Color color)
    {
        color = Color.white;

        if (itemData == null)
        {
            return false;
        }

        switch (itemData.itemType)
        {
            case ItemData.ItemType.Damage:
                color = Color.red;
                return true;
            case ItemData.ItemType.Magic:
                color = Color.blue;
                return true;
            case ItemData.ItemType.Speed:
                color = Color.green;
                return true;
            default:
                return false;
        }
    }

    private void ApplyVisualRuntime()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (targetRenderer == null)
        {
            return;
        }

        if (!TryGetItemColor(out Color color))
        {
            return;
        }

        if (targetRenderer.material != null)
        {
            targetRenderer.material.color = color;
        }
    }

    private void ApplyVisualEditor()
    {
        if (targetRenderer == null)
        {
            return;
        }

        if (!TryGetItemColor(out Color color))
        {
            return;
        }

        if (targetRenderer.sharedMaterial != null)
        {
            targetRenderer.sharedMaterial.color = color;
        }
    }
}
