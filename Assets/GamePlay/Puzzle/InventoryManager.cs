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
    Dictionary<string, int> puzzleDictionary;  //�洢ƴͼ���ֵ䣬value��int������Ϊռλ��
    public InventoryManager()
    {
        Dictionary<string, int> localInventoryData=LocalPlayerInventoryDada.LoadInventoryData();     //��ȡ���ش洢�ı������ݳ�ʼ��
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
