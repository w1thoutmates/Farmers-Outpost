using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InventoryUIController : MonoBehaviour
{
    public static InventoryUIController Instance { get; private set; }
    
    [FormerlySerializedAs("chestPanel")] public DynamicInventoryDisplay inventoryPanel;
    public DynamicInventoryDisplay playerBackpackPanel;

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
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (inventoryPanel.gameObject.activeInHierarchy || playerBackpackPanel.gameObject.activeInHierarchy)
            {
                CloseAllInventories();
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
            DisplayPlayerBackpack(playerInventory.PrimaryInventorySystem, 7);
        }
    }

    private void CloseAllInventories()
    {
        inventoryPanel.gameObject.SetActive(false);
        playerBackpackPanel.gameObject.SetActive(false);
    }
}
