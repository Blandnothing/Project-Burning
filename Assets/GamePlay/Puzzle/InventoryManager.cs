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
    Dictionary<string, int> puzzleDictionary;  //存储拼图的字典，value的int类型暂为占位符
    public InventoryManager()
    {
        Dictionary<string, int> localInventoryData=LocalPlayerInventoryDada.LoadInventoryData();     //读取本地存储的背包数据初始化
        if (localInventoryData==null)
        {
            puzzleDictionary = new();
        }
        else
        {
            puzzleDictionary=localInventoryData;
        }
    }
    public void AddObject(string key,int value)
    {
        puzzleDictionary[key]=value;
    }
    public void SaveInventoryData()
    {
        LocalPlayerInventoryDada.SaveInventoryData(puzzleDictionary);
    }
}
