using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PuzzleBoard : MonoBehaviour, IDropHandler
{
    public int width = 8;
    public int height = 7;
    // public GameObject slots;
    public Sprite[] puzzleSprites;
    // Start is called before the first frame update
    private float slotSize;
    private int[,] board;
    private Vector3 basePos;
    public Dictionary<PuzzlePiece, Vector2Int> pieces = new Dictionary<PuzzlePiece, Vector2Int>();


    void Start()
    {
        board = new int[height, width];
        Vector3[] worldCorners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(worldCorners);
        basePos = worldCorners[0];
        slotSize = (worldCorners[2].y - worldCorners[0].y) / height;
    }

    // Update is called once per frame
    void Update()
    {

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
        for (int i = 0; i < piece.connectedPieces.Count; i++)
            board[pos.y + piece.connectedPieces[i].y, pos.x + piece.connectedPieces[i].x] = 0;
        pieces.Remove(piece);
    }

    public void OnDrop(PointerEventData eventData)
    {
        var transform = eventData.pointerDrag.GetComponent<RectTransform>();
        var corners = new Vector3[4];
        transform.GetWorldCorners(corners);
        var pos = corners[0] - basePos;
        var x_point = Mathf.RoundToInt(pos.x / slotSize);
        var y_point = Mathf.RoundToInt(pos.y / slotSize);
        var piece = eventData.pointerDrag.GetComponent<PuzzlePiece>();
        if (TryPut(x_point, y_point, piece.connectedPieces))
        {
            pieces[piece] = new Vector2Int(x_point, y_point);
            Debug.Log(x_point + " " + y_point);
            piece.moveTo(this);
            transform.position = new Vector3(x_point * slotSize, y_point * slotSize)
                                 + (transform.position - corners[0]) + basePos;
        }
        else Debug.Log("fail");
    }
}
