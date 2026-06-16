using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInventoryHolder : InventoryHolder
{
    [SerializeField] protected int secondaryInventorySize;
    [SerializeField] protected InventorySystem secondaryInventorySystem;

    public InventorySystem SecondaryInventorySystem => secondaryInventorySystem;

    public static UnityAction<InventorySystem> OnPlayerBackpackDisplayRequested;

    void Update()
    {
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            OnPlayerBackpackDisplayRequested?.Invoke(secondaryInventorySystem);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        secondaryInventorySystem = new InventorySystem(secondaryInventorySize);
    }

    public bool AddToInventory(ItemData data, int amount)
    {
        if (primaryInventorySystem.AddToInventory(data, amount))
        {
            return true;
        } else if (secondaryInventorySystem.AddToInventory(data, amount))
        {
            return true;
        }

        return false;
    }
}
