using UnityEngine;

public class SaveData
{
    public SerializableDictionary<string, InventorySaveData> chestDictionary;
    public InventorySaveData playerInventory;

    public SaveData()
    {
        this.chestDictionary = new SerializableDictionary<string, InventorySaveData>();
        this.playerInventory = new InventorySaveData();
    }
}
