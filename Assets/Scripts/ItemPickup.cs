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
        // Only react to the player.
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (itemData == null)
        {
            Debug.LogWarning("ItemPickup failed: itemData is not assigned.");
            return;
        }

        // Resolve target scripts from player and apply bonuses.
        FirstPersonMeleeAttack melee = other.GetComponent<FirstPersonMeleeAttack>();
        if (melee != null)
        {
            float newMelee = melee.GetDamage() + itemData.meleeDamageBonus;
            melee.SetDamage(newMelee);
        }

        SpellCaster spellCaster = other.GetComponent<SpellCaster>();
        if (spellCaster != null)
        {
            float newSpell = spellCaster.GetSpellDamage() + itemData.spellDamageBonus;
            spellCaster.SetSpellDamage(newSpell);
        }

        FirstPersonController controller = other.GetComponent<FirstPersonController>();
        if (controller != null)
        {
            float newSpeed = controller.GetMoveSpeed() + itemData.movementSpeedBonus;
            controller.SetMoveSpeed(newSpeed);
        }

        Debug.Log($"Picked up item: {itemData.itemName}. Applied bonuses (Melee +{itemData.meleeDamageBonus}, Spell +{itemData.spellDamageBonus}, Speed +{itemData.movementSpeedBonus}).");

        // Remove pickup from scene after successful pickup.
        Destroy(gameObject);
    }
}
