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
    public Sprite[] puzzleSprites;
    // Start is called before the first frame update
    private float slotSize;
    private int[,] board;
    private Vector3 basePos;
    public Dictionary<PuzzlePiece, Vector2Int> pieces = new Dictionary<PuzzlePiece, Vector2Int>();
    public KeyCode key;

    void Start()
    {
        board = new int[height, width];
        Vector3[] worldCorners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(worldCorners);
        basePos = worldCorners[0];
        slotSize = (worldCorners[2].y - worldCorners[0].y) / height;
        
        foreach (var (piece, pos) in pieces)
        {
            var corners = new Vector3[4];
            piece.rectTrans.GetWorldCorners(corners);
            piece.rectTrans.position = new Vector3(pos.x * slotSize, pos.y * slotSize)
                + piece.rectTrans.position - corners[0] + basePos;
            piece.moveTo(this);
        }
    }

    private bool TryPut(int x, int y, List<Vector2Int> connectedPieces)
    {
        for (int i = 0; i < connectedPieces.Count; i++)
            if (y + connectedPieces[i].y >= height || x + connectedPieces[i].x >= width
                || y + connectedPieces[i].y < 0 || x + connectedPieces[i].x < 0
                || board[y + connectedPieces[i].y, x + connectedPieces[i].x] != 0)
                return false;
        for (int i = 0; i < connectedPieces.Count; i++)
            board[y + connectedPieces[i].y, x + connectedPieces[i].x] = 1;
        return true;
    }

    public void removePiece(PuzzlePiece piece)
    {
        var pos = pieces[piece];
        for (int i = 0; i < piece.info.connectedPieces.Count; i++)
            board[pos.y + piece.info.connectedPieces[i].y, pos.x + piece.info.connectedPieces[i].x] = 0;
        pieces.Remove(piece);
        DisablePuzzle(piece);
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        var transform = eventData.pointerDrag.GetComponent<RectTransform>();
        var corners = new Vector3[4];
        transform.GetWorldCorners(corners);
        var pos = corners[0] - basePos;
        var x_point = Mathf.RoundToInt(pos.x / slotSize);
        var y_point = Mathf.RoundToInt(pos.y / slotSize);
        var piece = eventData.pointerDrag.GetComponent<PuzzlePiece>();
        if (TryPut(x_point, y_point, piece.info.connectedPieces))
        {
            pieces[piece] = new Vector2Int(x_point, y_point);
            Debug.Log(x_point + " " + y_point);
            piece.moveTo(this);
            transform.position = new Vector3(x_point * slotSize, y_point * slotSize)
                                 + (transform.position - corners[0]) + basePos;

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
