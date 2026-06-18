using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InventoryUIController : MonoBehaviour
{
    public static InventoryUIController Instance { get; private set; }
    
    [FormerlySerializedAs("chestPanel")] public DynamicInventoryDisplay inventoryPanel;
    public DynamicInventoryDisplay playerBackpackPanel;
    
    private bool _justOpenedThisFrame;

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
        bool isChestOpen = inventoryPanel.gameObject.activeInHierarchy;
        bool isBackpackOpen = playerBackpackPanel.gameObject.activeInHierarchy;
        bool isAnyUIOpen = isChestOpen || isBackpackOpen;

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

            if (Keyboard.current.fKey.wasPressedThisFrame && isChestOpen && !_justOpenedThisFrame)
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

    void LateUpdate()
    {
        _justOpenedThisFrame = false;
    }

    void DisplayInventory(InventorySystem invToDisplay, int offset)
    {
        if (invToDisplay == null)
        {
            CloseAllInventories();
            return;
        }
        
        _justOpenedThisFrame = true;

        inventoryPanel.gameObject.SetActive(true);
        inventoryPanel.RefreshDynamicInventory(invToDisplay, offset);

        OpenPlayerBackpackIfClosed();

        // InputSystem.Update();
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

    private void CloseAllInventories()
    {
        inventoryPanel.gameObject.SetActive(false);
        playerBackpackPanel.gameObject.SetActive(false);
    }
}
