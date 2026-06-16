using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[System.Serializable]
public class InventorySystem
{
    [SerializeField] private List<InventorySlot> inventorySlots;

    private int inventorySize;

    public List<InventorySlot> InventorySlots => inventorySlots;
    public int InventorySize => InventorySlots.Count; // inventorySize;

    public UnityAction<InventorySlot> OnInventorySlotChanged;

    public InventorySystem(int size)
    {
        inventorySize = size;
        inventorySlots = new List<InventorySlot>(size);

        for (int i = 0; i < size; i++)
        {
            inventorySlots.Add(new InventorySlot());
        }
    }

    public bool AddToInventory(ItemData itemToAdd, int amountToAdd)
    {
        AddToInventory(itemToAdd, amountToAdd, out int remainingAmount);
        return remainingAmount <= 0;
    }

    public bool AddToInventory(ItemData itemToAdd, int amountToAdd, out int remainingAmount)
    {
        remainingAmount = amountToAdd;

        if (ContainsItem(itemToAdd, out List<InventorySlot> slots))
        {
            foreach (var slot in slots)
            {
                int spaceLeft = itemToAdd.maxStackSize - slot.StackSize;

                if (spaceLeft > 0)
                {
                    int amountToFill = Mathf.Min(remainingAmount, spaceLeft);

                    slot.AddToStack(amountToFill);
                    remainingAmount -= amountToFill;
                    OnInventorySlotChanged?.Invoke(slot);
                }

                if (remainingAmount <= 0) return true;
            }
        }

        while (remainingAmount > 0 && HasFreeSlot(out InventorySlot freeSlot))
        {
            int amountToPlace = Mathf.Min(remainingAmount, itemToAdd.maxStackSize);

            freeSlot.UpdateInventorySlot(itemToAdd, amountToPlace);
            remainingAmount -= amountToPlace;
            OnInventorySlotChanged?.Invoke(freeSlot);
        }

        return remainingAmount < amountToAdd;
    }

    public bool ContainsItem(ItemData itemToAdd, out List<InventorySlot> slots)
    {
        slots = InventorySlots.Where(i => i.ItemData == itemToAdd).ToList();

        return slots != null; //slots.Count > 0;
    }

    public bool HasFreeSlot(out InventorySlot freeSlot)
    {
        freeSlot = InventorySlots.FirstOrDefault(i => i.ItemData == null);
        return freeSlot != null;
    }
}
