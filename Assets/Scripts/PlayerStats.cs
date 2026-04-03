using UnityEngine;

/// <summary>
/// Simple player stats component.
/// Tracks gold and runtime stat bonuses.
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [Tooltip("Current amount of gold the player has.")]
    public int gold = 0;

    [Header("Runtime Item Bonuses")]
    [SerializeField] private float totalMeleeDamageBonus;
    [SerializeField] private float totalSpellDamageBonus;
    [SerializeField] private float totalMovementSpeedBonus;

    /// <summary>
    /// Adds gold to the player's total and logs the updated value.
    /// </summary>
    public void AddGold(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        gold += amount;
        Debug.Log($"Player gold is now: {gold}");
    }

    /// <summary>
    /// Applies item stat bonuses to relevant player systems.
    /// Bonuses are cumulative for the current run.
    /// </summary>
    public void ApplyItemBonuses(ItemData itemData)
    {
        if (itemData == null)
        {
            return;
        }

        FirstPersonMeleeAttack melee = GetComponent<FirstPersonMeleeAttack>();
        SpellCaster spellCaster = GetComponent<SpellCaster>();
        FirstPersonController controller = GetComponent<FirstPersonController>();

        if (melee != null)
        {
            float newMelee = melee.GetDamage() + itemData.meleeDamageBonus;
            melee.SetDamage(newMelee);
            totalMeleeDamageBonus += itemData.meleeDamageBonus;
        }

        if (spellCaster != null)
        {
            float newSpell = spellCaster.GetSpellDamage() + itemData.spellDamageBonus;
            spellCaster.SetSpellDamage(newSpell);
            totalSpellDamageBonus += itemData.spellDamageBonus;
        }

        if (controller != null)
        {
            float newSpeed = controller.GetMoveSpeed() + itemData.movementSpeedBonus;
            controller.SetMoveSpeed(newSpeed);
            totalMovementSpeedBonus += itemData.movementSpeedBonus;
        }

        Debug.Log($"Applied item bonuses. Totals => Melee: +{totalMeleeDamageBonus}, Spell: +{totalSpellDamageBonus}, Speed: +{totalMovementSpeedBonus}");
    }
}
