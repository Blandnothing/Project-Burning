using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class PuzzlePiece : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 base_scale;
    public PuzzleInfo info;
    public RectTransform rectTrans;
    [SerializeField] private Canvas canvas;

    private CanvasGroup canvasGroup;

    // private Vector3 startPos;

    [SerializeField] private PuzzleBoard father = null;

    [SerializeField] private Transform group;

    void Awake()
    {
        group = transform.parent;
        rectTrans = GetComponent<RectTransform>();
        rectTrans.sizeDelta = new Vector2(info.gridSize * info.p_width, info.gridSize * info.p_height);
        canvasGroup = GetComponent<CanvasGroup>();
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
        info.Init();
        base_scale = rectTrans.localScale;
        // startPos = rectTrans.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(canvas.transform);
        rectTrans.localScale = Vector3.one;
        canvasGroup.blocksRaycasts = false;
        rectTrans.SetAsLastSibling();
        if (father != null)
            father.RemovePiece(this);
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
        rectTrans.localScale = base_scale;
        // rectTrans.anchoredPosition = startPos;
        transform.SetParent(group);
    }

    public void moveTo(PuzzleBoard _father)
    {
        transform.SetParent(canvas.transform);
        rectTrans.localScale = Vector3.one;
        father = _father;
    }
}
