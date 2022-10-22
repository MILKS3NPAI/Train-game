using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static ArrangeGrid;

public class ArrangeTile : BaseTile, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public ArrangePoint ClosestArrangePoint;
    [Tooltip("List of all position offsets of tiles from its coordinates")] public List<Vector2Int> AllTilePositions;

    private ArrangeGrid _arrangeGrid;
    private bool _isMoving, _shouldCheckClosest;
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _arrangeGrid = FindObjectOfType<ArrangeGrid>();
        _canvas = FindObjectOfType<BaseGrid>().GridPointCanvas;
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
        StartCoroutine(SetNewPosition());
    }
    private void FixedUpdate()
    {
        transform.position = !_isMoving && ClosestArrangePoint ? ClosestArrangePoint.transform.position : transform.position;
    }
    private IEnumerator SetNewPosition()
    {
        while (true)
        {
            // Only set the closest tile after being dropped/released from player touch
            yield return new WaitUntil(() => _shouldCheckClosest && !_isMoving);
            _shouldCheckClosest = false;

            // Reset current occupancy to allow for the points it was just on to be considered again when finding the closest, valid point
            SetCurrentOccupancy(false);
            ClosestArrangePoint = FindClosestArrangePoint();
            Coordinates = ClosestArrangePoint.Coordinates;
            SetCurrentOccupancy(true);
        }
    }
    private ArrangePoint FindClosestArrangePoint()
    {
        ArrangePoint closestArrangePoint = null;
        float distance = Mathf.Infinity;
        foreach (ArrangePoint gridPoint in ArrangePointArray)
        {
            if (Vector2.Distance(gridPoint.transform.position, transform.position) < distance && _arrangeGrid.CheckIfPointIsValid(gridPoint, this))
            {
                closestArrangePoint = gridPoint;
                distance = Vector2.Distance(gridPoint.transform.position, transform.position);
            }
        }
        return closestArrangePoint;
    }
    public void SetCurrentOccupancy(bool occupancyStatus)
    {
        foreach (Vector2Int offset in AllTilePositions)
        {
            Vector2Int oldPoint = ClosestArrangePoint.Coordinates + offset;
            if (_arrangeGrid.CheckForValidIndexes(oldPoint.x, oldPoint.y))
                ArrangePointArray[oldPoint.x, oldPoint.y].IsOccupied = occupancyStatus;
        }
    }

    #region Movement
    public void OnBeginDrag(PointerEventData eventData)
    {
        _isMoving = true;
        _canvasGroup.blocksRaycasts = false;
        transform.SetParent(_arrangeGrid.HoverCanvas.transform);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        _isMoving = false;
        _canvasGroup.blocksRaycasts = true;
        transform.SetParent(_arrangeGrid.GridPointCanvas.transform);
        _shouldCheckClosest = true;
    }
    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }
    #endregion
}
