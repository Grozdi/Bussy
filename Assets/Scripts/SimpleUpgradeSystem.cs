using System;
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
        ResolveReferences();
    }

    private void Update()
    {
        // Keep references fresh in case objects are swapped at runtime.
        ResolveReferences();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            BuyMeleeUpgrade();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            BuySpellUpgrade();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            BuySpeedUpgrade();
        }
    }

    private void BuyMeleeUpgrade()
    {
        const string upgradeName = "Melee Damage";

        TryPurchaseUpgrade(
            meleeUpgradeCost,
            meleeAttack != null,
            upgradeName,
            () =>
            {
                float oldValue = meleeAttack.GetDamage();
                float newValue = oldValue + meleeDamageIncrease;

                meleeAttack.SetDamage(newValue);

                LogUpgradeSuccess(upgradeName, oldValue, newValue, meleeUpgradeCost);
            });
    }

    private void BuySpellUpgrade()
    {
        const string upgradeName = "Spell Damage";

        TryPurchaseUpgrade(
            spellUpgradeCost,
            spellCaster != null,
            upgradeName,
            () =>
            {
                float oldValue = spellCaster.GetSpellDamage();
                float newValue = oldValue + spellDamageIncrease;

                spellCaster.SetSpellDamage(newValue);

                LogUpgradeSuccess(upgradeName, oldValue, newValue, spellUpgradeCost);
            });
    }

    private void BuySpeedUpgrade()
    {
        const string upgradeName = "Move Speed";

        TryPurchaseUpgrade(
            speedUpgradeCost,
            firstPersonController != null,
            upgradeName,
            () =>
            {
                float oldValue = firstPersonController.GetMoveSpeed();
                float newValue = oldValue + moveSpeedIncrease;

                firstPersonController.SetMoveSpeed(newValue);

                LogUpgradeSuccess(upgradeName, oldValue, newValue, speedUpgradeCost);
            });
    }

    /// <summary>
    /// Centralized purchase flow to keep logic modular and consistent.
    /// </summary>
    private void TryPurchaseUpgrade(int cost, bool hasTargetReference, string upgradeName, Action applyUpgrade)
    {
        if (playerStats == null)
        {
            LogUpgradeFailure(upgradeName, "PlayerStats reference is missing.");
            return;
        }

        if (!hasTargetReference)
        {
            LogUpgradeFailure(upgradeName, "Required component reference is missing.");
            return;
        }

        if (playerStats.gold < cost)
        {
            LogUpgradeFailure(upgradeName, $"Not enough gold (need {cost}, have {playerStats.gold}).");
            return;
        }

        // Deduct gold first, then apply upgrade.
        playerStats.gold -= cost;
        applyUpgrade?.Invoke();
    }

    private void LogUpgradeSuccess(string upgradeName, float oldValue, float newValue, int cost)
    {
        Debug.Log(
            $"[Upgrade Success] {upgradeName} | Cost: {cost} | Old: {oldValue:F2} -> New: {newValue:F2} | Gold Left: {playerStats.gold}");
    }

    private void LogUpgradeFailure(string upgradeName, string reason)
    {
        int gold = playerStats != null ? playerStats.gold : 0;
        Debug.LogWarning($"[Upgrade Failed] {upgradeName} | Reason: {reason} | Gold: {gold}");
    }

    /// <summary>
    /// Attempts to fill missing references safely.
    /// </summary>
    private void ResolveReferences()
    {
        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
        }

        if (meleeAttack == null)
        {
            meleeAttack = GetComponent<FirstPersonMeleeAttack>();
        }

        if (spellCaster == null)
        {
            spellCaster = GetComponent<SpellCaster>();
        }

        if (firstPersonController == null)
        {
            firstPersonController = GetComponent<FirstPersonController>();
        }

        // Fallback: try to find components on Player-tagged object.
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            return;
        }

        if (playerStats == null)
        {
            playerStats = playerObject.GetComponent<PlayerStats>();
        }

        if (meleeAttack == null)
        {
            meleeAttack = playerObject.GetComponent<FirstPersonMeleeAttack>();
        }

        if (spellCaster == null)
        {
            spellCaster = playerObject.GetComponent<SpellCaster>();
        }

        if (firstPersonController == null)
        {
            firstPersonController = playerObject.GetComponent<FirstPersonController>();
        }
    }
}
