using UnityEngine;
using UnityEngine.Events;

public class SaveGameManager : MonoBehaviour
{
    public static SaveData data;

    void Awake()
    {
        data = new SaveData();
        SaveLoad.OnLoadGame += LoadData;
    }

    public void DeleteData()
    {
        SaveLoad.DeleteSaveData();
    }

    public static void SaveData()
    {
        var saveData = data;
        
        SaveLoad.SaveGame(saveData);
    }

    public static void TryLoadData()
    {
        SaveLoad.LoadGame();
    }

    public static void LoadData(SaveData dataToLoad)
    {
        data = dataToLoad;
    }
}
