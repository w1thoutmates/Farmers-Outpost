using System;
using UnityEngine;

[System.Serializable]
public class InventorySlot : ISerializationCallbackReceiver
{
    [NonSerialized] private ItemData itemData;
    [SerializeField] private int stackSize;
    [SerializeField] private int itemID = -1;
    private float _durability;
    private float _watering;

    public ItemData ItemData => itemData;

    public int StackSize => stackSize;
    
    public float Durability => _durability;
    
    public float Watering => _watering;

    public InventorySlot(ItemData source, int amount)
    {
        SetItem(source, amount);
    }

    public InventorySlot()
    {
        ClearSlot();
    }
    
    public void Use()
    {
        ItemData?.Use(this);
    }

    public void ClearSlot()
    {
        itemData = null;
        itemID = -1;
        stackSize = -1;
    }

    public bool EnoughSpaceLeftInStack(int amountToAdd)
    {
        return (itemData == null || itemData != null && stackSize + amountToAdd <= itemData.maxStackSize);
    }

    public bool EnoughSpaceLeftInStack(int amountToAdd, out int amountRemaining)
    {
        amountRemaining = itemData.maxStackSize - stackSize;
        return EnoughSpaceLeftInStack(amountToAdd);
    }

    public void AddToStack(int amount) 
    {
        stackSize = Mathf.Clamp(stackSize + amount, 0, itemData.maxStackSize);
    }

    public void RemoveFromStack(int amount)
    {
        stackSize = Mathf.Clamp(stackSize - amount, 0, itemData.maxStackSize);

        if (stackSize <= 0)
        {
            ClearSlot();
        }
    }

    public void UpdateInventorySlot(ItemData source, int amount)
    {
        SetItem(source, amount);
    }

    public void AssignItem(InventorySlot invSlot)
    {
        if (itemData == invSlot.itemData)
        {
            AddToStack(invSlot.stackSize);
        } else
        {
            itemData = invSlot.ItemData;
            itemID = itemData.id;
            stackSize = 0;
            AddToStack(invSlot.StackSize);
        }
    }

    public bool SplitStack(out InventorySlot splitStack)
    {
        if (stackSize <= 1)
        {
            splitStack = null;
            return false;
        }

        int halfStack = Mathf.RoundToInt(stackSize / 2);
        RemoveFromStack(halfStack);

        splitStack = new InventorySlot(itemData, halfStack);

        return true;
    }
    
    private void SetItem(ItemData source, int amount)
    {
        itemData = source;
        itemID = source.id;
        stackSize = amount;

        if (source is ItemTool tool)
        {
            _durability = tool.maxDurability;
        }

        if (source is WateringTool wateringTool)
        {
            _watering = wateringTool.maxWaterCapacity;
        }
    }

    public bool ReduceWatering(float reduceAmount, WateringTool toolData)
    {
        if (_watering <= 0) return false;
        
        _watering = Mathf.Clamp(_watering - reduceAmount, 0, toolData.maxWaterCapacity);
        return true;
    }

    public void ReduceDurability(float reduceDurabilityByUse, ItemTool toolData)
    {
        _durability = Mathf.Clamp(_durability - reduceDurabilityByUse, 0, toolData.maxDurability);
        if (_durability <= 0)
        {
            BreakItem();
        }
    }

    public void BreakItem()
    {
        ClearSlot();
        // Tool break sound
        EventBus.NotifyThatToolWasDestroyed(this);
    }

    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
        if (itemID == -1) return;

        var db = Resources.Load<Database>("Database");
        itemData = db.GetItem(itemID);
    }
    
    public void SetWatering(float amount)
    {
        _watering = Mathf.Clamp(amount, 0, GetMaxWaterCapacity());
    }

    public float GetMaxWaterCapacity()
    {
        if (itemData is WateringTool wateringTool)
            return wateringTool.maxWaterCapacity;
        return 0;
    }

    public float GetMaxDurability()
    {
        if (itemData is ItemTool tool && itemData is not WateringTool)
            return tool.maxDurability;
        return 0;
    }

    public bool IsWateringTool()
    {
        return itemData is WateringTool;
    }

    public bool IsToolWithDurability()
    {
        return itemData is ItemTool && !IsWateringTool();
    }
    
    public bool RefillWatering(float amount)
    {
        if (!IsWateringTool()) return false;
    
        float maxCapacity = GetMaxWaterCapacity();
        if (_watering >= maxCapacity) return false;
    
        _watering = Mathf.Clamp(_watering + amount, 0, maxCapacity);
        return true;
    }
}
