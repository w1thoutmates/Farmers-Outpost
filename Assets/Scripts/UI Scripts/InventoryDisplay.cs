using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public abstract class InventoryDisplay : MonoBehaviour
{
    [SerializeField] protected MouseItemData mouseItemData;

    protected InventorySystem inventorySystem;
    protected Dictionary<InventorySlotUI, InventorySlot> slotDictionary;

    public InventorySystem InventorySystem => inventorySystem;
    public Dictionary<InventorySlotUI, InventorySlot> SlotDictionary => slotDictionary;

    protected virtual void Start()
    {

    }

    public abstract void AssignSlots(InventorySystem invToDisplay, int offset);

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
            HandleShiftClick(clickedUISlot, true);
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

        if (clickedUISlot.AssignedInventorySlot.ItemData != null && mouseItemData.assignedInventorySlot.ItemData != null)
        {
            bool isSameItem = clickedUISlot.AssignedInventorySlot.ItemData == mouseItemData.assignedInventorySlot.ItemData;

            if (isSameItem && clickedUISlot.AssignedInventorySlot.EnoughSpaceLeftInStack(mouseItemData.assignedInventorySlot.StackSize))
            {
                clickedUISlot.AssignedInventorySlot.AssignItem(mouseItemData.assignedInventorySlot);
                clickedUISlot.UpdateUISlot();

                mouseItemData.ClearSlot();
                return;
            }
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
        bool isShiftPressed = Keyboard.current.leftShiftKey.isPressed;

        if (isShiftPressed && clickedUISlot.AssignedInventorySlot.ItemData != null)
        {
            HandleShiftClick(clickedUISlot, false);
            return;
        }
        
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

        if (clickedUISlot.AssignedInventorySlot.ItemData != null && mouseItemData.assignedInventorySlot.ItemData != null)
        {
            bool isSameItem = clickedUISlot.AssignedInventorySlot.ItemData == mouseItemData.assignedInventorySlot.ItemData;

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

    void HandleShiftClick(InventorySlotUI clickedUISlot, bool isLeftClick)
    {
        InventorySystem targetSystem = GetTargetInventorySystem();
        if (targetSystem == null) return;

        InventorySlot sourceSlot = clickedUISlot.AssignedInventorySlot;
        ItemData itemData = sourceSlot.ItemData;
        int halfStack = Mathf.CeilToInt(sourceSlot.StackSize / 2f);
        
        if (!isLeftClick && halfStack <= 0) return;
        
        int amountToMove = isLeftClick ? sourceSlot.StackSize : halfStack;
        int baseRemainingInSource = sourceSlot.StackSize - amountToMove;

        if (targetSystem == this.inventorySystem)
        {
            int slotIndex = this.inventorySystem.InventorySlots.IndexOf(sourceSlot);
            
            bool isClickedInHotbar = slotIndex < 7;

            int startIdx = isClickedInHotbar ? 7 : 0;
            int endIdx = isClickedInHotbar ? this.inventorySystem.InventorySize : 7;

            int remainingAmount = amountToMove;

            for (int i = startIdx; i < endIdx; i++)
            {
                var slot = this.inventorySystem.InventorySlots[i];
                if (slot.ItemData == itemData)
                {
                    int spaceLeft = itemData.maxStackSize - slot.StackSize;
                    if (spaceLeft > 0)
                    {
                        int amountToFill = Mathf.Min(remainingAmount, spaceLeft);
                        slot.AddToStack(amountToFill);
                        remainingAmount -= amountToFill;
                        this.inventorySystem.OnInventorySlotChanged?.Invoke(slot);
                    }
                }
                if (remainingAmount <= 0) break;
            }

            if (remainingAmount > 0)
            {
                for (int i = startIdx; i < endIdx; i++)
                {
                    var slot = this.inventorySystem.InventorySlots[i];
                    if (slot.ItemData == null)
                    {
                        int amountToPlace = Mathf.Min(remainingAmount, itemData.maxStackSize);
                        slot.UpdateInventorySlot(itemData, amountToPlace);
                        remainingAmount -= amountToPlace;
                        this.inventorySystem.OnInventorySlotChanged?.Invoke(slot);
                    }
                    if (remainingAmount <= 0) break;
                }
            }
            
            int finalSourceAmount = baseRemainingInSource + remainingAmount;

            if (finalSourceAmount <= 0) 
            {
                clickedUISlot.ClearSlot();
            }
            else 
            {
                sourceSlot.UpdateInventorySlot(itemData, finalSourceAmount);
                clickedUISlot.UpdateUISlot();
            }

            this.inventorySystem.OnInventorySlotChanged?.Invoke(sourceSlot);
            return; 
        }

        targetSystem.AddToInventory(itemData, amountToMove, out int remAmount);
        
        int totalRemaining = baseRemainingInSource + remAmount;

        if (totalRemaining <= 0)
        {
            clickedUISlot.ClearSlot();
        }
        else
        {
            sourceSlot.UpdateInventorySlot(itemData, totalRemaining);
            clickedUISlot.UpdateUISlot();
        }

        inventorySystem.OnInventorySlotChanged?.Invoke(sourceSlot);
    }

    InventorySystem GetTargetInventorySystem()
    {
        if (InventoryUIController.Instance == null) return null;

        var uiController = InventoryUIController.Instance;
        DynamicInventoryDisplay chestPanel = uiController.inventoryPanel;
    
        bool isChestOpen = chestPanel != null && chestPanel.gameObject.activeInHierarchy;

        if (this is DynamicInventoryDisplay currentDynamic && currentDynamic == chestPanel)
        {
            StaticInventoryDisplay hotbar = FindAnyObjectByType<StaticInventoryDisplay>();
            return hotbar != null ? hotbar.InventorySystem : null;
        }

        if (isChestOpen)
        {
            return chestPanel.InventorySystem;
        }
        return this.InventorySystem;
    }
}
