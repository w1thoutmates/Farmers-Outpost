using System;
using UnityEngine;

[System.Serializable]
public class InventorySlot : ISerializationCallbackReceiver
{
    [NonSerialized] private ItemData itemData; // Сделали не сериализуемым, чтобы в файле сохранения не путало. Если что то сломается - вернуть аннотацию [SerializeField]
    [SerializeField] private int stackSize;
    [SerializeField] private int itemID = -1;

    public ItemData ItemData => itemData;

    public int StackSize => stackSize;

    public InventorySlot(ItemData source, int amount)
    {
        this.itemData = source;
        itemID = itemData.id;
        this.stackSize = amount;
    }

    public InventorySlot()
    {
        ClearSlot();
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

    public void UpdateInventorySlot(ItemData data, int amount)
    {
        itemData = data;
        itemID = itemData.id;
        stackSize = amount;
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

    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
        if (itemID == -1) return;

        var db = Resources.Load<Database>("Database");
        itemData = db.GetItem(itemID);
    }
}
