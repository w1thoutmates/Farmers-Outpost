using System;
using UnityEngine;

public class StaticInventoryDisplay : InventoryDisplay
{
    [SerializeField] private InventoryHolder inventoryHolder;
    [SerializeField] protected InventorySlotUI[] slots;

    protected virtual void OnEnable()
    {
        PlayerInventoryHolder.OnPlayerInventoryChanged += RefreshStaticDisplay;
        EventBus.onUINeedToRefresh += UpdateAllSlotsDisplay;
    }

    protected virtual void OnDisable()
    {
        PlayerInventoryHolder.OnPlayerInventoryChanged -= RefreshStaticDisplay;
        if (inventorySystem != null)
        {
            inventorySystem.OnInventorySlotChanged -= UpdateSlot;
        }
        
        EventBus.onUINeedToRefresh -= UpdateAllSlotsDisplay;
    }

    protected void RefreshStaticDisplay()
    {
        if (inventoryHolder != null)
        {
            if (inventorySystem != null) inventorySystem.OnInventorySlotChanged -= UpdateSlot;

            inventorySystem = inventoryHolder.PrimaryInventorySystem;
            inventorySystem.OnInventorySlotChanged += UpdateSlot;
            
            AssignSlots(inventorySystem, 0);
        }

        foreach (InventorySlotUI slot in slots)
        {
            if (slot.AssignedInventorySlot != null)
            {
                slot.UpdateUISlot();
                slot.UpdateDurabilityDisplay(slot.AssignedInventorySlot);
            }
        }
    }
    
    public void UpdateAllSlotsDisplay()
    {
        foreach (InventorySlotUI slot in slots)
        {
            if (slot.AssignedInventorySlot != null)
            {
                slot.UpdateUISlot();
                slot.UpdateDurabilityDisplay(slot.AssignedInventorySlot);
            }
        }
    }

    protected override void Start()
    {
        base.Start();

        if (inventoryHolder != null)
        {
            inventorySystem = inventoryHolder.PrimaryInventorySystem;
            inventorySystem.OnInventorySlotChanged += UpdateSlot;
        }

        AssignSlots(inventorySystem, 0);
    }

    public override void AssignSlots(InventorySystem invToDisplay, int offset)
    {
        slotDictionary = new System.Collections.Generic.Dictionary<InventorySlotUI, InventorySlot>();

        if (invToDisplay == null || slots == null) return;

        for (int i = 0; i < inventoryHolder.Offset; i++)
        {
            if (i < invToDisplay.InventorySlots.Count && i < slots.Length)
            {
                slotDictionary.Add(slots[i], invToDisplay.InventorySlots[i]);
                slots[i].Init(invToDisplay.InventorySlots[i]);
                slots[i].UpdateUISlot();
            }
        }
    }
}