using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUIController : MonoBehaviour
{
    public DynamicInventoryDisplay chestPanel;
    public DynamicInventoryDisplay playerBackpackPanel;

    void Awake()
    {
        CloseAllInventories();
    }

    void OnEnable()
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested += DisplayInventory;
        PlayerInventoryHolder.OnPlayerBackpackDisplayRequested += DisplayPlayerBackpack;
    }

    void OnDisable()
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested -= DisplayInventory;
        PlayerInventoryHolder.OnPlayerBackpackDisplayRequested -= DisplayPlayerBackpack;
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (chestPanel.gameObject.activeInHierarchy || playerBackpackPanel.gameObject.activeInHierarchy)
            {
                CloseAllInventories();
            }
        }
    }

    void DisplayInventory(InventorySystem invToDisplay)
    {
        if (invToDisplay == null)
        {
            CloseAllInventories();
            return;
        }

        chestPanel.gameObject.SetActive(true);
        chestPanel.RefreshDynamicInventory(invToDisplay);

        OpenPlayerBackpackIfClosed();
    }

    void DisplayPlayerBackpack(InventorySystem invToDisplay)
    {
        playerBackpackPanel.gameObject.SetActive(true);
        playerBackpackPanel.RefreshDynamicInventory(invToDisplay);
    }

    private void OpenPlayerBackpackIfClosed()
    {
        if (playerBackpackPanel.gameObject.activeInHierarchy) return;

        var playerInventory = FindAnyObjectByType<PlayerInventoryHolder>();

        if (playerInventory != null)
        {
            DisplayPlayerBackpack(playerInventory.SecondaryInventorySystem);
        }
    }

    private void CloseAllInventories()
    {
        chestPanel.gameObject.SetActive(false);
        playerBackpackPanel.gameObject.SetActive(false);
    }
}
