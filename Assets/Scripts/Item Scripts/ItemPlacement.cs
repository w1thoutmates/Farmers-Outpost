using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Inventory System/Inventory Item/Placement")]
public class ItemPlacement : ItemData
{
    public Vector2Int size = Vector2Int.one;
    public GameObject prefab;

    public override void Use(InventorySlot slot)
    {
        base.Use(slot);
    }
}
