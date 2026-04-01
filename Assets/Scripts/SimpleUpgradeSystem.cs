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
        TryPurchaseUpgrade(
            meleeUpgradeCost,
            meleeAttack != null,
            "Melee upgrade",
            () =>
            {
                float newDamage = meleeAttack.GetDamage() + meleeDamageIncrease;
                meleeAttack.SetDamage(newDamage);
                Debug.Log($"Melee upgrade purchased. New melee damage: {newDamage}. Gold left: {playerStats.gold}");
            });
    }

    private void BuySpellUpgrade()
    {
        TryPurchaseUpgrade(
            spellUpgradeCost,
            spellCaster != null,
            "Spell upgrade",
            () =>
            {
                float newDamage = spellCaster.GetSpellDamage() + spellDamageIncrease;
                spellCaster.SetSpellDamage(newDamage);
                Debug.Log($"Spell upgrade purchased. New spell damage: {newDamage}. Gold left: {playerStats.gold}");
            });
    }

    private void BuySpeedUpgrade()
    {
        TryPurchaseUpgrade(
            speedUpgradeCost,
            firstPersonController != null,
            "Speed upgrade",
            () =>
            {
                float newSpeed = firstPersonController.GetMoveSpeed() + moveSpeedIncrease;
                firstPersonController.SetMoveSpeed(newSpeed);
                Debug.Log($"Speed upgrade purchased. New move speed: {newSpeed}. Gold left: {playerStats.gold}");
            });
    }

    /// <summary>
    /// Centralized purchase flow to keep logic modular and consistent.
    /// </summary>
    private void TryPurchaseUpgrade(int cost, bool hasTargetReference, string upgradeName, Action applyUpgrade)
    {
        if (playerStats == null)
        {
            Debug.LogWarning($"{upgradeName} failed: PlayerStats reference is missing.");
            return;
        }

        if (!hasTargetReference)
        {
            Debug.LogWarning($"{upgradeName} failed: required component reference is missing.");
            return;
        }

        if (playerStats.gold < cost)
        {
            Debug.Log($"{upgradeName} failed: not enough gold. Need {cost}, have {playerStats.gold}.");
            return;
        }

        // Deduct gold first, then apply upgrade.
        playerStats.gold -= cost;
        applyUpgrade?.Invoke();
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
