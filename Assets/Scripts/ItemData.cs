using UnityEngine;

/// <summary>
/// Data asset for an item and its stat bonuses.
/// </summary>
[CreateAssetMenu(fileName = "NewItemData", menuName = "Game/Item Data")]
public class ItemData : ScriptableObject
{
    public enum ItemType
    {
        Damage,
        Magic,
        Speed
    }

    [Header("Basic Info")]
    public string itemName;
    [TextArea]
    public string description;
    public ItemType itemType;

    [Header("Stat Modifiers")]
    public float meleeDamageBonus;
    public float spellDamageBonus;
    public float movementSpeedBonus;
}
