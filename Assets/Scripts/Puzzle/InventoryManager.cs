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
    public Dictionary<int,PuzzleInfo> puzzleDictionary;  //�洢ƴͼ���ֵ䣬value��int������Ϊռλ��
    public InventoryManager()
    {
        Dictionary<int, PuzzleInfo> localInventoryData =SaveAndLoad.LoadInventoryData<Dictionary<int, PuzzleInfo>>(LocalPath.inventoryData);     //��ȡ���ش洢�ı������ݳ�ʼ��
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
