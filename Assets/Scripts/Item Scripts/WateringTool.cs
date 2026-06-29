using System;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Inventory System/Inventory Item/Watering Tool")]
public class WateringTool : ItemTool
{
    [SerializeField] public int maxWaterCapacity = 100;
    [SerializeField] public int reduceWaterByUse = 25;
    
    public override void Use(InventorySlot slot)
    {
        base.Use(slot);
    }

    public override void RightClickUse(InventorySlot slot)
    {
        base.RightClickUse(slot);
    }
}
