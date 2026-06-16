using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public abstract class InventoryHolder : MonoBehaviour
{
    [SerializeField] private int inventorySize;
    [SerializeField] protected InventorySystem primaryInventorySystem;
    [SerializeField] protected int offset = 7;
    
    public int Offset => offset;

    public InventorySystem PrimaryInventorySystem => primaryInventorySystem;

    public static UnityAction<InventorySystem, int> OnDynamicInventoryDisplayRequested;

    protected virtual void Awake()
    {
        SaveLoad.OnLoadGame += LoadInventory;
        
        primaryInventorySystem = new InventorySystem(inventorySize);
    }

    protected abstract void LoadInventory(SaveData arg0);

}

[System.Serializable]
public struct InventorySaveData
{
    public InventorySystem inventorySystem;
    public Vector3 position;
    public Quaternion rotation;

    public InventorySaveData(InventorySystem inventorySystem, Vector3 position, Quaternion rotation)
    {
        this.inventorySystem = inventorySystem;
        this.position = position;
        this.rotation = rotation;
    }
    
    public InventorySaveData(InventorySystem inventorySystem)
    {
        this.inventorySystem = inventorySystem;
        this.position = Vector3.zero;
        this.rotation = Quaternion.identity;
    }
}

