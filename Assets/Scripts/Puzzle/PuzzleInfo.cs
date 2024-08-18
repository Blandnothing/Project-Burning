using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Puzzle Info", menuName = "Puzzle/Puzzle Info")]
public class PuzzleInfo : ScriptableObject
{
    public int id;
    public PuzzleEffect effect;
    public int p_height = 5, p_width = 5;
    public string puzzlePieces;
    public List<Vector2Int> connectedPieces;

    public void Init()
    {
        connectedPieces = new List<Vector2Int>();
        for (int i = 0; i < p_height; i++)
            for (int j = 0; j < p_width; j++)
                if (puzzlePieces[i * p_width + j] == '1')
                    connectedPieces.Add(new Vector2Int(j, p_height - 1 - i));
        Debug.Log("Init");
    }
}
