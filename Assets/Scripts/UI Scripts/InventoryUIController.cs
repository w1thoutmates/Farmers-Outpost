using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InventoryUIController : MonoBehaviour
{
    public static InventoryUIController Instance { get; private set; }
    
    [FormerlySerializedAs("chestPanel")] public DynamicInventoryDisplay inventoryPanel;
    public DynamicInventoryDisplay playerBackpackPanel;
    
    public static UnityAction<InventorySystem> OnChestInventoryOpened;
    public static UnityAction OnChestInventoryClosed;
    
    public bool IsAnyInventoryOpen =>
        inventoryPanel.gameObject.activeInHierarchy ||
        playerBackpackPanel.gameObject.activeInHierarchy;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        CloseAllInventories();
    }

    void OnEnable()
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested += DisplayInventory;
    }

    void OnDisable()
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested -= DisplayInventory;
    }

    void Update()
    {
        bool isAnyUIOpen = inventoryPanel.gameObject.activeInHierarchy || playerBackpackPanel.gameObject.activeInHierarchy;

        if (isAnyUIOpen)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CloseAllInventories();
                return;
            }

            if (Keyboard.current.bKey.wasPressedThisFrame)
            {
                CloseAllInventories();
                return;
            }
        }
        else
        {
            if (Keyboard.current.bKey.wasPressedThisFrame)
            {
                OpenPlayerBackpackIfClosed();
            }
        }
    }

    void DisplayInventory(InventorySystem invToDisplay, int offset)
    {
        if (invToDisplay == null)
        {
            CloseAllInventories();
            return;
        }

        inventoryPanel.gameObject.SetActive(true);
        inventoryPanel.RefreshDynamicInventory(invToDisplay, offset);

        OpenPlayerBackpackIfClosed();

        OnChestInventoryOpened?.Invoke(invToDisplay);
    }

    void DisplayPlayerBackpack(InventorySystem invToDisplay, int offset)
    {
        playerBackpackPanel.gameObject.SetActive(true);
        playerBackpackPanel.RefreshDynamicInventory(invToDisplay, offset);
    }

    private void OpenPlayerBackpackIfClosed()
    {
        if (playerBackpackPanel.gameObject.activeInHierarchy) return;

        var playerInventory = FindAnyObjectByType<PlayerInventoryHolder>();

        if (playerInventory != null)
        {
            DisplayPlayerBackpack(playerInventory.PrimaryInventorySystem, playerInventory.Offset);
        }
    }

    public void CloseAllInventories()
    {
        inventoryPanel.gameObject.SetActive(false);
        playerBackpackPanel.gameObject.SetActive(false);
        
        OnChestInventoryClosed?.Invoke();
    }
}
