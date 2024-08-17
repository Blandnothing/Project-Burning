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
    public List<PuzzlePiece> puzzleDictionary;  //�洢ƴͼ���ֵ䣬value��int������Ϊռλ��
    public InventoryManager()
    {
        List<PuzzlePiece> localInventoryData =SaveAndLoad.LoadInventoryData<List<PuzzlePiece>>(LocalPath.inventoryData);     //��ȡ���ش洢�ı������ݳ�ʼ��
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
