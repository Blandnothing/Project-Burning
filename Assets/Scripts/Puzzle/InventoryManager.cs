using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager 
{
    static InventoryManager instance;
    public static InventoryManager Instance
    {
        get {
            if (instance == null)
            {
                instance = new InventoryManager();
            }
            return instance; 
        }
    }
    public Dictionary<int,PuzzleInfo> puzzleDictionary;  //閿熻姤鍌ㄦ嫾鍥鹃敓鏂ゆ嫹閿熻鍏革紝value閿熸枻鎷穒nt閿熸枻鎷烽敓鏂ゆ嫹閿熸枻鎷蜂负鍗犱綅閿熸枻鎷�
    public InventoryManager()
    {
        Dictionary<int, PuzzleInfo> localInventoryData =SaveAndLoad.LoadInventoryData<Dictionary<int, PuzzleInfo>>(LocalPath.inventoryData);     //閿熸枻鎷峰彇閿熸枻鎷烽敓鎴瓨鍌ㄩ敓渚ユ唻鎷烽敓鏂ゆ嫹閿熸枻鎷烽敓鎹风鎷峰閿熸枻鎷�
        if (localInventoryData==null)
        {
            puzzleDictionary = new();
        }
        else
        {
            puzzleDictionary=localInventoryData;
        }
        foreach (var item in puzzleDictionary)
        {
            //Debug.Log(item);
        }
    }
    public void AddObject(PuzzleInfo value)
    {
        puzzleDictionary[value.id] = value;
        SaveInventoryData();
    }
    public void SaveInventoryData()
    {
        SaveAndLoad.SaveInventoryData<Dictionary<int, PuzzleInfo>>(LocalPath.inventoryData,puzzleDictionary);
    }
}
