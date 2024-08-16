using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using static LocalConfig;

public class SaveAndLoad
{
    public static void SaveInventoryData<T>(string path, T data)
    {
        if (!File.Exists(Application.persistentDataPath))
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath);
        }
        if (!File.Exists(Application.persistentDataPath + string.Format("/SaveData")))
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + string.Format("/SaveData"));
        }
        string jsonData = JsonConvert.SerializeObject(data,new JsonSerializerSettings{ ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        File.WriteAllText(Application.persistentDataPath + string.Format("/SaveData",path), jsonData);
    }
    public static T LoadInventoryData<T>(string path)
    {
        string truePath = Application.persistentDataPath + "/SaveData"+path;
        if (File.Exists(truePath))
        {
                string jsonData = File.ReadAllText(path);
                T data = JsonConvert.DeserializeObject<T>(jsonData);
                return data;

        }
        else
        {
            return default;
        }
    }
}
public static class LocalPath
{
    public const string inventoryData = "/InventoryData.json";
}
