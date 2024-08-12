using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PuzzleCollection : MonoBehaviour
{
    public string puzzleName;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            if (InventoryManager.Instance.puzzleDictionary.ContainsKey(puzzleName))
            {
                return;
            }
            InventoryManager.Instance.AddObject(puzzleName, 1);
        }
    }
}
