using UnityEngine;

public class StaticInventoryDisplay : InventoryDisplay
{
    [SerializeField] private InventoryHolder inventoryHolder;
    [SerializeField] private InventorySlotUI[] slots;

    protected override void Start()
    {
        base.Start();

        if (inventoryHolder != null)
        {
            inventorySystem = inventoryHolder.PrimaryInventorySystem;
            inventorySystem.OnInventorySlotChanged += UpdateSlot;
        } else
        {
            Debug.Log($"No inventory assigned to {this.gameObject}");
        }

        AssignSlots(inventorySystem);
    }

    public override void AssignSlots(InventorySystem invToDisplay)
    {
        slotDictionary = new System.Collections.Generic.Dictionary<InventorySlotUI, InventorySlot>();

        if (slots.Length != inventorySystem.InventorySize) Debug.Log($"Inventory slots out of sync on {this.gameObject}");

        for (int i = 0; i < inventorySystem.InventorySize; i ++)
        {
            slotDictionary.Add(slots[i], inventorySystem.InventorySlots[i]);
            slots[i].Init(inventorySystem.InventorySlots[i]);
        }
    }
}
