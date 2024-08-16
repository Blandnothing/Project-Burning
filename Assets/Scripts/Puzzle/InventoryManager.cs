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
    public List<PuzzlePiece> puzzleDictionary;  //存储拼图的字典，value的int类型暂为占位符
    public InventoryManager()
    {
        List<PuzzlePiece> localInventoryData =SaveAndLoad.LoadInventoryData<List<PuzzlePiece>>(LocalPath.inventoryData);     //读取本地存储的背包数据初始化
        if (localInventoryData==null)
        {
            puzzleDictionary = new();
        }
        else
        {
            puzzleDictionary=localInventoryData;
        }
    }
    public void AddObject(PuzzlePiece value)
    {
        puzzleDictionary.Add(value);
        SaveInventoryData();
    }
    public void SaveInventoryData()
    {
        SaveAndLoad.SaveInventoryData<List<PuzzlePiece>>(LocalPath.inventoryData,puzzleDictionary);
    }
}
