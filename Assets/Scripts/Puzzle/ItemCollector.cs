using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemCollector : MonoBehaviour
{
    public PuzzleInfo piece;
    private void Awake()
    {
        if (InventoryManager.Instance.puzzleDictionary.ContainsKey(piece.id))
            {
                Destroy(gameObject);
            }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            if (InventoryManager.Instance.puzzleDictionary.ContainsKey(piece.id))
            {
                Debug.LogError("Repeat Puzzle don't destroy");
                return;
            }
            InventoryManager.Instance.AddObject(piece);
            Destroy(gameObject);
        }
    }
}
