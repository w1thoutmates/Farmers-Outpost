using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private int stackSize;

    public ItemData ItemData => itemData;

    public int StackSize => stackSize;

    public InventorySlot(ItemData source, int amount)
    {
        this.itemData = source;
        this.stackSize = amount;
    }

    public InventorySlot()
    {
        ClearSlot();
    }

    public void ClearSlot()
    {
        itemData = null;
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
}
