using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu(menuName = "Inventory System/Inventory Item")]
public class ItemData : ScriptableObject
{
    public int id = -1;
    public string displayName;
    [TextArea(4, 4)]
    public string description;
    public Sprite icon;
    public int maxStackSize;
    public ItemWorld itemWorld;

    public virtual void Use()
    {
        if (EventSystem.current != null && MouseItemData.IsPointerOverUIObject())
        {
            return; 
        }
        if (InventoryUIController.Instance != null)
        {
            bool isChestOpen = InventoryUIController.Instance.inventoryPanel.gameObject.activeInHierarchy;
            bool isBackpackOpen = InventoryUIController.Instance.playerBackpackPanel.gameObject.activeInHierarchy;
            if (isChestOpen || isBackpackOpen) return;
        }
        
        Debug.Log($"[{displayName}] was used.");
    }
}
