using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(UniqueID))]
public class ChestInventory : InventoryHolder, IInteractable
{
    [SerializeField] private float availableInventoryTimeOutOfRadius = 2f;

    private bool isInAvailableRadius = false;
    private float timer;
    private bool isOpened;

    public UnityAction<IInteractable> OnInteractionComplete { get; set; }

    protected override void Awake()
    {
        base.Awake();

        SaveLoad.OnLoadGame += LoadInventory;

        timer = availableInventoryTimeOutOfRadius;
    }

    void Start()
    {
        string uniqueId = GetComponent<UniqueID>().ID;

        if (SaveGameManager.data == null)
        {
            Debug.LogError("SaveGameManager.data не инициализирован! Проверьте порядок загрузки менеджера сохранения.");
            return;
        }

        if (SaveGameManager.data.chestDictionary == null)
        {
            SaveGameManager.data.chestDictionary = new SerializableDictionary<string, InventorySaveData>();
        }

        if (SaveGameManager.data.chestDictionary.TryGetValue(uniqueId, out InventorySaveData existingChestData))
        {
            this.primaryInventorySystem = existingChestData.inventorySystem;
            this.transform.position = existingChestData.position;
            this.transform.rotation = existingChestData.rotation;
        }
        else
        {
            var chestSaveData = new InventorySaveData(primaryInventorySystem, this.transform.position, this.transform.rotation);
            SaveGameManager.data.chestDictionary.Add(uniqueId, chestSaveData);
        }
    }

    protected override void LoadInventory(SaveData data)
    {
        if (data != null && data.chestDictionary != null && 
            data.chestDictionary.TryGetValue(GetComponent<UniqueID>().ID, out InventorySaveData chestSaveData))
        {
            this.primaryInventorySystem = chestSaveData.inventorySystem;
            this.transform.position = chestSaveData.position;
            this.transform.rotation = chestSaveData.rotation;
        }
    }

    public void Interact(Interactor interactor, out bool isInteractWasSuccessful)
    {
        OnDynamicInventoryDisplayRequested?.Invoke(primaryInventorySystem, 0);
        isInteractWasSuccessful = true;
        isOpened = true;

        timer = availableInventoryTimeOutOfRadius;
    }

    public void EndInteraction()
    {
        if (!isOpened) return;

        OnDynamicInventoryDisplayRequested?.Invoke(null, 0);

        isOpened = false;
        OnInteractionComplete?.Invoke(this);
    }

    void Update()
    {
        if (isOpened && !isInAvailableRadius)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                EndInteraction();
            }
        }
    }

    public void SetPlayerInRadius(bool inRadius)
    {
        isInAvailableRadius = inRadius;

        if (inRadius)
        {
            timer = availableInventoryTimeOutOfRadius;
        }
    }
}
