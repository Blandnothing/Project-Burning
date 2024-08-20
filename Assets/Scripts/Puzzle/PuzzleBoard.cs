using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PuzzleBoard : MonoBehaviour, IDropHandler
{
    public int width = 5;
    public int height = 5;
    // public GameObject slots;
    private float slotSize;
    private int[,] board;
    private Vector3 basePos;
    private static Dictionary<KeyCode, Dictionary<string, Vector2Int>> _pieceData;
    public static Dictionary<KeyCode, Dictionary<string, Vector2Int>> pieceData
    {
        get
        {
            if (_pieceData == null)
                _pieceData = new Dictionary<KeyCode, Dictionary<string, Vector2Int>>();
            return _pieceData;
        }
    }
    private Dictionary<string, Vector2Int> pieces;
    public KeyCode key;

    void Awake()
    {
        if (!pieceData.ContainsKey(key)) pieceData[key] = new Dictionary<string, Vector2Int>();
        pieces = pieceData[key];
    }

    void Start()
    {
        board = new int[height, width];
        Vector3[] worldCorners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(worldCorners);
        basePos = worldCorners[0];
        slotSize = (worldCorners[2].y - worldCorners[0].y) / height;
        foreach (var (name, pos) in pieces)
            PutPiece(pos.x, pos.y, GameObject.Find(name).GetComponent<PuzzlePiece>());
    }

    public void PutPiece(int x, int y, PuzzlePiece piece)
    {
        piece.moveTo(this);
        var corners = new Vector3[4];
        piece.rectTrans.GetWorldCorners(corners);
        piece.rectTrans.position = new Vector3(x * slotSize, y * slotSize)
                             + (piece.rectTrans.position - corners[0]) + basePos;
        for (int i = 0; i < piece.info.connectedPieces.Count; i++)
            board[y + piece.info.connectedPieces[i].y, x + piece.info.connectedPieces[i].x] = 1;
    }

    private bool TryPut(int x, int y, PuzzlePiece piece)
    {
        for (int i = 0; i < piece.info.connectedPieces.Count; i++)
            if (y + piece.info.connectedPieces[i].y >= height || x + piece.info.connectedPieces[i].x >= width
                || y + piece.info.connectedPieces[i].y < 0 || x + piece.info.connectedPieces[i].x < 0
                || board[y + piece.info.connectedPieces[i].y, x + piece.info.connectedPieces[i].x] != 0)
                return false;
        for (int i = 0; i < piece.info.connectedPieces.Count; i++)
            board[y + piece.info.connectedPieces[i].y, x + piece.info.connectedPieces[i].x] = 1;
        return true;
    }

    public void RemovePiece(PuzzlePiece piece)
    {
        var pos = pieces[piece.name];
        for (int i = 0; i < piece.info.connectedPieces.Count; i++)
            board[pos.y + piece.info.connectedPieces[i].y, pos.x + piece.info.connectedPieces[i].x] = 0;
        pieces.Remove(piece.name);
        DisablePuzzle(piece);
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        var pieceTrans = eventData.pointerDrag.GetComponent<RectTransform>();
        var corners = new Vector3[4];
        pieceTrans.GetWorldCorners(corners);
        var pos = corners[0] - basePos;
        var x_point = Mathf.RoundToInt(pos.x / slotSize);
        var y_point = Mathf.RoundToInt(pos.y / slotSize);
        var piece = eventData.pointerDrag.GetComponent<PuzzlePiece>();
        if (TryPut(x_point, y_point, piece))
        {
            PutPiece(x_point, y_point, piece);
            pieces[piece.name] = new Vector2Int(x_point, y_point);
            EnablePuzzle(piece);
        }
        else Debug.Log("fail");
    }
    public void EnablePuzzle(PuzzlePiece piece)
    {
        PuzzleEffectManager.Instance.AddPuzzle(piece.info.effect, key);
    }
    public void DisablePuzzle(PuzzlePiece piece)
    {
        PuzzleEffectManager.Instance.RemovePuzzle(piece.info.effect, key);
    }
}
