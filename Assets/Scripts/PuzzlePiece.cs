using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class PuzzlePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Identity")]
    public int pieceID;
    public Image image;

    [Header("Settings")]
    public bool returnToStartOnFail = true;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas parentCanvas;

    private Vector3 startPosition;
    private Vector2 startSize;
    private Transform startParent;

    public bool IsLocked { get; private set; } = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
        parentCanvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        startPosition = rectTransform.position;
        startSize = rectTransform.sizeDelta;
        startParent = transform.parent;

        if (PuzzleManager.Instance != null)
            PuzzleManager.Instance.RegisterPiece(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsLocked) return;

        // Record state for Undo before moving
        PuzzleManager.Instance.RecordMove(this, rectTransform.position, transform.parent, IsLocked);

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        // Ensure it draws on top
        if (parentCanvas != null)
            transform.SetParent(parentCanvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsLocked) return;
        rectTransform.anchoredPosition += eventData.delta / parentCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsLocked) return;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        CheckSnap();
    }

    private void CheckSnap()
    {
        SnapArea bestSnap = null;
        float closestDist = float.MaxValue;

        // Find all SnapAreas in scene
        SnapArea[] snapAreas = FindObjectsOfType<SnapArea>();

        foreach (var snap in snapAreas)
        {
            // Simply check distance to the snap center, ignoring IDs
            float d = Vector2.Distance(rectTransform.position, snap.GetSnapPosition());
            if (d <= snap.snapDistance && d < closestDist)
            {
                closestDist = d;
                bestSnap = snap;
            }
        }

        if (bestSnap != null)
        {
            SnapTo(bestSnap);
        }
        else
        {
            if (returnToStartOnFail)
            {
                // Return to original parent to keep hierarchy clean, or stay in world space
                transform.SetParent(startParent);
                rectTransform.position = startPosition;
            }
        }
    }

    public void SnapTo(SnapArea snap)
    {
        // Parenting to the target keeps things organized
        transform.SetParent(snap.transform);

        // Snap to center (0,0,0) relative to parent
        rectTransform.anchoredPosition = Vector2.zero;

        // Resize to match snap area or default
        RectTransform snapRect = snap.GetComponent<RectTransform>();
        if (snapRect != null)
        {
            rectTransform.sizeDelta = snapRect.sizeDelta;
        }
        else
        {
            rectTransform.sizeDelta = new Vector2(840, 1080);
        }

        // Change anchors to middle-center
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        IsLocked = true;
        canvasGroup.blocksRaycasts = false; // Disable partial interaction

      
    }

    // Called by Undo System
    public void ForceMove(Vector3 pos, Transform parent, bool lockedState)
    {
        transform.position = pos;
        transform.SetParent(parent);
        IsLocked = lockedState;
        canvasGroup.blocksRaycasts = !IsLocked;
    }

    // Called by Reset
    public void ResetToStart()
    {
        transform.position = startPosition;
        rectTransform.sizeDelta = startSize;
        transform.SetParent(startParent);
        IsLocked = false;
        canvasGroup.blocksRaycasts = true;
    }
}
