using System.Linq;
using UnityEngine;

public class DynamicInventoryDisplay : InventoryDisplay
{
    [SerializeField] protected InventorySlotUI pfInvSlot;

    public enum DynamicType { Backpack, Chest }
    [SerializeField] private DynamicType displayType;
    public DynamicType DisplayType => displayType;

    protected override void Start()
    {
        base.Start();
    }

    public void RefreshDynamicInventory(InventorySystem invToDisplay)
    {
        ClearSlots();
        inventorySystem = invToDisplay;
        if (inventorySystem != null) inventorySystem.OnInventorySlotChanged += UpdateSlot;
        AssignSlots(invToDisplay);
    }

    public override void AssignSlots(InventorySystem invToDisplay)
    {
        slotDictionary = new System.Collections.Generic.Dictionary<InventorySlotUI, InventorySlot>();

        if (invToDisplay == null) return;

        for (int i = 0; i < invToDisplay.InventorySize; i++)
        {
            var uiSlot = Instantiate(pfInvSlot, transform);
            slotDictionary.Add(uiSlot, invToDisplay.InventorySlots[i]);
            uiSlot.Init(invToDisplay.InventorySlots[i]);
            uiSlot.UpdateUISlot();
        }
    }

    private void ClearSlots()
    {
        foreach (var item in transform.Cast<Transform>())
        {
            Destroy(item.gameObject);
        }

        if (slotDictionary != null) SlotDictionary.Clear();
    }

    private void OnDisable()
    {
        if (inventorySystem != null) inventorySystem.OnInventorySlotChanged -= UpdateSlot;
    }
}
