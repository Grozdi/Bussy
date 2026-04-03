using UnityEngine;

/// <summary>
/// Data asset for an item and its stat bonuses.
/// </summary>
[CreateAssetMenu(fileName = "NewItemData", menuName = "Game/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    [TextArea]
    public string description;

    [Header("Stat Modifiers")]
    public float meleeDamageBonus;
    public float spellDamageBonus;
    public float movementSpeedBonus;
}
