using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInventoryHolder : InventoryHolder
{
    public static UnityAction OnPlayerInventoryChanged;

    void Start()
    {
        SaveGameManager.data.playerInventory = new InventorySaveData(primaryInventorySystem);
    }
    
    // void Update()
    // {
    //     if (Keyboard.current.bKey.wasPressedThisFrame)
    //     {
    //         if (InventoryUIController.Instance != null)
    //         {
    //             var backpack = InventoryUIController.Instance.playerBackpackPanel;
    //         
    //             if (backpack.gameObject.activeInHierarchy)
    //             {
    //                 backpack.gameObject.SetActive(false);
    //             }
    //             else
    //             {
    //                 backpack.gameObject.SetActive(true);
    //                 backpack.RefreshDynamicInventory(primaryInventorySystem, offset);
    //             }
    //         }
    //     }
    // }
    
    protected override void LoadInventory(SaveData data)
    {
        if (data.playerInventory.inventorySystem != null)
        {
            this.primaryInventorySystem = data.playerInventory.inventorySystem;
            OnPlayerInventoryChanged?.Invoke();
        }
    }

    public bool AddToInventory(ItemData data, int amount)
    {
        if (primaryInventorySystem.AddToInventory(data, amount))
        {
            return true;
        }

        return false;
    }
}
