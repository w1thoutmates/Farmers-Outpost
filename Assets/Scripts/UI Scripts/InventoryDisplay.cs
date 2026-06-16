using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public abstract class InventoryDisplay : MonoBehaviour
{
    [SerializeField] MouseItemData mouseItemData;

    protected InventorySystem inventorySystem;
    protected Dictionary<InventorySlotUI, InventorySlot> slotDictionary;

    public InventorySystem InventorySystem => inventorySystem;
    public Dictionary<InventorySlotUI, InventorySlot> SlotDictionary => slotDictionary;

    protected virtual void Start()
    {

    }

    public abstract void AssignSlots(InventorySystem invToDisplay);

    protected virtual void UpdateSlot(InventorySlot updatedSlot)
    {
        foreach (var slot in slotDictionary)
        {
            if (slot.Value == updatedSlot) // Slot Value is the "under the hood" inventory slot. 
            {
                slot.Key.UpdateUISlot(updatedSlot); // Slot Key is the UI representation of the Value. 
            }
        }
    }

    public void SlotClicked(InventorySlotUI clickedUISlot)
    {
        bool isShiftPressed = Keyboard.current.leftShiftKey.isPressed;

        if (isShiftPressed && clickedUISlot.AssignedInventorySlot.ItemData != null)
        {
            HandleShiftClick(clickedUISlot);
            return;
        }

        if (clickedUISlot.AssignedInventorySlot.ItemData != null && mouseItemData.assignedInventorySlot.ItemData == null)
        {
            mouseItemData.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);
            clickedUISlot.ClearSlot();
            return;
        }

        if (clickedUISlot.AssignedInventorySlot.ItemData == null && mouseItemData.assignedInventorySlot.ItemData != null)
        {
            clickedUISlot.AssignedInventorySlot.AssignItem(mouseItemData.assignedInventorySlot);
            clickedUISlot.UpdateUISlot();

            mouseItemData.ClearSlot();
            return;
        }

        // Если предмет на который кликнули и предмет под курсором НЕ ПУСТЫЕ (т.е. заполненные)
        if (clickedUISlot.AssignedInventorySlot.ItemData != null && mouseItemData.assignedInventorySlot.ItemData != null)
        {
            bool isSameItem = clickedUISlot.AssignedInventorySlot.ItemData == mouseItemData.assignedInventorySlot.ItemData;

            // Если предмет под курсором и на который кликнули одинаковый и еще можно стакнуть этот предмет - стакаем
            if (isSameItem && clickedUISlot.AssignedInventorySlot.EnoughSpaceLeftInStack(mouseItemData.assignedInventorySlot.StackSize))
            {
                clickedUISlot.AssignedInventorySlot.AssignItem(mouseItemData.assignedInventorySlot);
                clickedUISlot.UpdateUISlot();

                mouseItemData.ClearSlot();
                return;
            }
            // Если предмет под курсором и на который кликнули одинаковый и уже нельзя стакнуть этот предмет
            else if (isSameItem && !clickedUISlot.AssignedInventorySlot.EnoughSpaceLeftInStack(mouseItemData.assignedInventorySlot.StackSize, out int leftInStack))
            {
                if (leftInStack < 1) // stack is full so swap the items;
                {
                    SwapSlots(clickedUISlot);
                    return;
                } 
                else // slot is not at max, so take what's needed from the mouse inventory
                {
                    int remainingOnMouse = mouseItemData.assignedInventorySlot.StackSize - leftInStack;
                    clickedUISlot.AssignedInventorySlot.AddToStack(leftInStack);
                    clickedUISlot.UpdateUISlot();

                    var newItem = new InventorySlot(mouseItemData.assignedInventorySlot.ItemData, remainingOnMouse);
                    mouseItemData.ClearSlot();
                    mouseItemData.UpdateMouseSlot(newItem);
                    return;
                }
            }

            // Если предмет под курсором и на который кликнули Разные - меняем их местами
            else if (!isSameItem)
            {
                SwapSlots(clickedUISlot);
                return;
            }
        }
    } 

    private void SwapSlots(InventorySlotUI clickedUISlot)
    {
        var clonedSlot = new InventorySlot(mouseItemData.assignedInventorySlot.ItemData, mouseItemData.assignedInventorySlot.StackSize);
        mouseItemData.ClearSlot();
        mouseItemData.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);

        clickedUISlot.ClearSlot();
        clickedUISlot.AssignedInventorySlot.AssignItem(clonedSlot);
        clickedUISlot.UpdateUISlot();
    }

    public void SlotRightClicked(InventorySlotUI clickedUISlot)
    {
        if (clickedUISlot.AssignedInventorySlot.ItemData != null &&
            mouseItemData.assignedInventorySlot.ItemData == null)
        {
            if (clickedUISlot.AssignedInventorySlot.SplitStack(out InventorySlot halfStackSlot))
            {
                mouseItemData.UpdateMouseSlot(halfStackSlot);
                clickedUISlot.UpdateUISlot();
                return;
            }
        }

        if (clickedUISlot.AssignedInventorySlot.ItemData == null && mouseItemData.assignedInventorySlot.ItemData != null)
        {
            ItemData item = mouseItemData.assignedInventorySlot.ItemData;

            mouseItemData.assignedInventorySlot.RemoveFromStack(1);

            if (mouseItemData.assignedInventorySlot.ItemData == null)
            {
                mouseItemData.ClearSlot();
            }
            else
            {
                mouseItemData.UpdateMouseSlot(mouseItemData.assignedInventorySlot);
            }

            clickedUISlot.AssignedInventorySlot.AssignItem(new InventorySlot(item, 1));
            clickedUISlot.UpdateUISlot();

            return;
        }

        // Если предмет на который кликнули и предмет под курсором НЕ ПУСТЫЕ (т.е. заполненные)
        if (clickedUISlot.AssignedInventorySlot.ItemData != null && mouseItemData.assignedInventorySlot.ItemData != null)
        {
            bool isSameItem = clickedUISlot.AssignedInventorySlot.ItemData == mouseItemData.assignedInventorySlot.ItemData;

            // Если предмет под курсором то тот же что и предмет, по которому кликнули и стак предмета по которому кликнули не забит
            if (isSameItem && clickedUISlot.AssignedInventorySlot.StackSize != clickedUISlot.AssignedInventorySlot.ItemData.maxStackSize)
            {
                ItemData item = mouseItemData.assignedInventorySlot.ItemData;

                mouseItemData.assignedInventorySlot.RemoveFromStack(1);

                if (mouseItemData.assignedInventorySlot.ItemData == null)
                {
                    mouseItemData.ClearSlot();
                }
                else
                {
                    mouseItemData.UpdateMouseSlot(mouseItemData.assignedInventorySlot);
                }

                clickedUISlot.AssignedInventorySlot.AddToStack(1);
                clickedUISlot.UpdateUISlot();
                return;
            }
        }
    }

    void HandleShiftClick(InventorySlotUI clickedUISlot)
    {
        InventorySystem targetSystem = GetTargetInvetorySystem();

        if (targetSystem == null) return;

        InventorySlot sourceSlot = clickedUISlot.AssignedInventorySlot;
        ItemData itemData = sourceSlot.ItemData;
        int amountToMove = sourceSlot.StackSize;

        targetSystem.AddToInventory(itemData, amountToMove, out int remainingAmount);

        if (remainingAmount == amountToMove) return;

        if (remainingAmount <= 0)
        {
            clickedUISlot.ClearSlot();
        }
        else
        {
            sourceSlot.RemoveFromStack(amountToMove - remainingAmount);
        }

        inventorySystem.OnInventorySlotChanged?.Invoke(sourceSlot);
    }

    InventorySystem GetTargetInvetorySystem()
    {
        DynamicInventoryDisplay[] dynamicDisplays = FindObjectsByType<DynamicInventoryDisplay>();

        DynamicInventoryDisplay chestDisplay = null;
        DynamicInventoryDisplay backpackDisplay = null;

        foreach (var display in dynamicDisplays)
        {
            if (!display.gameObject.activeInHierarchy) continue;

            if (display.DisplayType == DynamicInventoryDisplay.DynamicType.Chest)
                chestDisplay = display;
            else if (display.DisplayType == DynamicInventoryDisplay.DynamicType.Backpack)
                backpackDisplay = display;
        }

        StaticInventoryDisplay hotbarDisplay = FindAnyObjectByType<StaticInventoryDisplay>();

        if (this is StaticInventoryDisplay)
        {
            if (chestDisplay != null) return chestDisplay.InventorySystem;
            if (backpackDisplay != null) return backpackDisplay.InventorySystem;
        }

        else if (this is DynamicInventoryDisplay currentDynamic)
        {
            if (currentDynamic.DisplayType == DynamicInventoryDisplay.DynamicType.Chest)
            {
                if (backpackDisplay != null) return backpackDisplay.InventorySystem;
                if (hotbarDisplay != null) return hotbarDisplay.InventorySystem;
            }
            else if (currentDynamic.DisplayType == DynamicInventoryDisplay.DynamicType.Backpack)
            {
                if (chestDisplay != null) return chestDisplay.InventorySystem;
                if (hotbarDisplay != null) return hotbarDisplay.InventorySystem;
            }
        }

        return null;
    }
}
