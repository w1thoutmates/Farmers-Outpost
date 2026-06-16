using System.IO;
using UnityEngine;
using UnityEngine.Events;

public static class SaveLoad
{
    public static UnityAction OnSaveGame;
    public static UnityAction<SaveData> OnLoadGame;

    private static string _directory = "/SaveData/";
    private static string _fileName = "SaveData.sav";

    public static bool SaveGame(SaveData data)
    {
         OnSaveGame?.Invoke();
         
         string dir = Application.persistentDataPath + _directory;
         
         GUIUtility.systemCopyBuffer = dir;
         
         if (!Directory.Exists(dir))
             Directory.CreateDirectory(dir);
         
         string json = JsonUtility.ToJson(data, true);
         File.WriteAllText(dir + _fileName, json);
         
         Debug.Log(dir + _fileName + " saved");

         return true;
    }

    public static SaveData LoadGame()
    {
        string fullPath = Application.persistentDataPath + _directory + _fileName;
        
        SaveData data = new SaveData();

        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            data = JsonUtility.FromJson<SaveData>(json);
            
            OnLoadGame?.Invoke(data);
        }
        else
        {
            Debug.Log("Save file doesn't exist");
        }

        return data;
    }

    public static void DeleteSaveData()
    {
        string fullPath = Application.persistentDataPath + _directory + _fileName;
        
        if (File.Exists(fullPath)) 
            File.Delete(fullPath);
    }
}

