using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Puzzle Info",menuName ="Puzzle/Puzzle Info")]
public class PuzzleInfo : ScriptableObject
{
    public int id;
    public PuzzleEffect effect;
    public int p_height = 5, p_width = 5;
    public List<Vector2Int> connectedPieces;
}
