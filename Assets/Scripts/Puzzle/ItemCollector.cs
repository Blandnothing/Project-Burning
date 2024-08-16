using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemCollector : MonoBehaviour
{
    public PuzzlePiece piece;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            if (InventoryManager.Instance.puzzleDictionary.Contains(piece))
            {
                return;
            }
            InventoryManager.Instance.AddObject(piece);
        }
    }
}
