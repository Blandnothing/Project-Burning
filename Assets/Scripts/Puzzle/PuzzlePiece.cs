using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PuzzlePiece : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int id;
    PuzzleEffect effect;
    public int p_height = 4, p_width = 4;
    private Vector3 base_scale;
    public List<Vector2Int> connectedPieces;
    private RectTransform rectTrans;
    [SerializeField] private Canvas canvas;

    private CanvasGroup canvasGroup;

    private Vector3 startPos;
    private PuzzleBoard father = null;

    void Start()
    {
        rectTrans = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
        for (int i = 0; i < p_height; i++)
            for (int j = 0; j < p_width; j++)
                connectedPieces.Add(new Vector2Int(i, j));
        base_scale = rectTrans.localScale;
        // rectTrans.localScale = base_scale * Mathf.Min(1.0f * height / PuzzleData.layoutSize, 1);
        startPos = rectTrans.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        rectTrans.SetAsLastSibling();
        if (father != null)
            father.removePiece(this);
        father = null;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        if (father == null)
            moveBack();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTrans.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void moveBack()
    {
        rectTrans.anchoredPosition = startPos;
    }

    public void moveTo(PuzzleBoard _father)
    {
        father = _father;
    }
}
