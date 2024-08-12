using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using static LocalConfig;

public class LocalPlayerInventoryDada
{
    public static void SaveInventoryData(Dictionary<string, int> playerInventoryManager)
    {
        if (!File.Exists(Application.persistentDataPath))
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath);
        }
        if (!File.Exists(Application.persistentDataPath + string.Format("/SaveData")))
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + string.Format("/SaveData"));
        }
        string jsonData = JsonConvert.SerializeObject(playerInventoryManager,new JsonSerializerSettings{ ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        File.WriteAllText(Application.persistentDataPath + string.Format("/SaveData/InventoryData.json"), jsonData);
    }
    public static Dictionary<string, int> LoadInventoryData()
    {
        string path = Application.persistentDataPath + "/SaveData/InventoryData.json";
        if (File.Exists(path))
        {
                string jsonData = File.ReadAllText(path);
                Dictionary<string, int> playerinventorydata = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonData);
                return playerinventorydata;

        }
        else
        {
            return null;
        }
    }
}
