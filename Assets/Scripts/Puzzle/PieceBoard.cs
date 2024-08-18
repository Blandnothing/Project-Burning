using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceBoard : MonoBehaviour
{
    void Start()
    {
        foreach (Transform child in transform)
            if (InventoryManager.Instance.puzzleDictionary.ContainsKey(child.GetComponent<PuzzlePiece>().info.id))
                child.gameObject.SetActive(true);
            else
                child.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
