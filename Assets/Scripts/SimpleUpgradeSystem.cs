using UnityEngine;

/// <summary>
/// Simple keyboard-driven upgrade system (no UI).
/// Uses PlayerStats gold to buy upgrades for melee damage,
/// spell damage, and movement speed.
/// </summary>
public class SimpleUpgradeSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private FirstPersonMeleeAttack meleeAttack;
    [SerializeField] private SpellCaster spellCaster;
    [SerializeField] private FirstPersonController firstPersonController;

    [Header("Upgrade Costs")]
    [SerializeField] private int meleeUpgradeCost = 25;
    [SerializeField] private int spellUpgradeCost = 25;
    [SerializeField] private int speedUpgradeCost = 25;

    [Header("Upgrade Amounts")]
    [SerializeField] private float meleeDamageIncrease = 5f;
    [SerializeField] private float spellDamageIncrease = 5f;
    [SerializeField] private float moveSpeedIncrease = 0.75f;

    private void Awake()
    {
        // Optional fallback to auto-find PlayerStats on same object.
        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
        }
    }

    private void Update()
    {
        // Press 1 to buy melee damage upgrade.
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            BuyMeleeUpgrade();
        }

        // Press 2 to buy spell damage upgrade.
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            BuySpellUpgrade();
        }

        // Press 3 to buy move speed upgrade.
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            BuySpeedUpgrade();
        }
    }

    private void BuyMeleeUpgrade()
    {
        if (!CanAfford(meleeUpgradeCost))
        {
            Debug.Log($"Melee upgrade failed: not enough gold. Need {meleeUpgradeCost}, have {GetGoldSafe()}.");
            return;
        }

        if (meleeAttack == null)
        {
            Debug.LogWarning("Melee upgrade failed: FirstPersonMeleeAttack reference is missing.");
            return;
        }

        SpendGold(meleeUpgradeCost);

        float newDamage = meleeAttack.GetDamage() + meleeDamageIncrease;
        meleeAttack.SetDamage(newDamage);

        Debug.Log($"Melee upgrade purchased. New melee damage: {newDamage}. Gold left: {playerStats.gold}");
    }

    private void BuySpellUpgrade()
    {
        if (!CanAfford(spellUpgradeCost))
        {
            Debug.Log($"Spell upgrade failed: not enough gold. Need {spellUpgradeCost}, have {GetGoldSafe()}.");
            return;
        }

        if (spellCaster == null)
        {
            Debug.LogWarning("Spell upgrade failed: SpellCaster reference is missing.");
            return;
        }

        SpendGold(spellUpgradeCost);

        float newDamage = spellCaster.GetSpellDamage() + spellDamageIncrease;
        spellCaster.SetSpellDamage(newDamage);

        Debug.Log($"Spell upgrade purchased. New spell damage: {newDamage}. Gold left: {playerStats.gold}");
    }

    private void BuySpeedUpgrade()
    {
        if (!CanAfford(speedUpgradeCost))
        {
            Debug.Log($"Speed upgrade failed: not enough gold. Need {speedUpgradeCost}, have {GetGoldSafe()}.");
            return;
        }

        if (firstPersonController == null)
        {
            Debug.LogWarning("Speed upgrade failed: FirstPersonController reference is missing.");
            return;
        }

        SpendGold(speedUpgradeCost);

        float newSpeed = firstPersonController.GetMoveSpeed() + moveSpeedIncrease;
        firstPersonController.SetMoveSpeed(newSpeed);

        Debug.Log($"Speed upgrade purchased. New move speed: {newSpeed}. Gold left: {playerStats.gold}");
    }

    private bool CanAfford(int cost)
    {
        return playerStats != null && playerStats.gold >= cost;
    }

    private int GetGoldSafe()
    {
        return playerStats != null ? playerStats.gold : 0;
    }

    private void SpendGold(int amount)
    {
        // Deduct gold directly as requested.
        playerStats.gold -= amount;
        Debug.Log($"Spent {amount} gold. Current gold: {playerStats.gold}");
    }
}
