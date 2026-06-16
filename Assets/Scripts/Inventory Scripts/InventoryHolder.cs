using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class InventoryHolder : MonoBehaviour
{
    [SerializeField] private int inventorySize;
    [SerializeField] protected InventorySystem primaryInventorySystem;

    public InventorySystem PrimaryInventorySystem => primaryInventorySystem;

    public static UnityAction<InventorySystem> OnDynamicInventoryDisplayRequested;

    protected virtual void Awake()
    {
        primaryInventorySystem = new InventorySystem(inventorySize);
    }
}
