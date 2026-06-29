using System;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Inventory System/Inventory Item/Tool")]
public class ItemTool : ItemData
{
    public float maxDurability;
    public float reduceDurabilityByUse = 1;
    public ToolActionType actionType;
    
    public override void Use(InventorySlot slot)
    {
        base.Use(slot);
        
        slot.ReduceDurability(reduceDurabilityByUse, this);

        if (slot.Durability <= 0)
        {
            slot.BreakItem();
        }
    }
    
}

public enum ToolActionType
{
    None,
    Hoe,
    Watering,
    Shovel,
    Axe
}
