using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static FlowGrid;
using static FlowLogic;

public class FlowTile : BaseTile, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
    private delegate void Status();
    private Status _updateStatus, _pointStatus;

    public FlowTileType PointType;
    public int PathID;

    [SerializeField] private GameObject _activeIndicator, _currentIndicator;
    private static bool _isPointerDown;
    private FlowGrid _flowGrid;

    private void Awake()
    {
        _flowGrid = FindObjectOfType<FlowGrid>();
    }
    private void OnEnable()
    {
        _updateStatus += UpdateVisuals;
        _updateStatus += UpdateIndicators;
        _pointStatus += ContinuePath;
    }
    private void OnDisable()
    {
        _updateStatus -= UpdateVisuals;
        _updateStatus -= UpdateIndicators;
        _pointStatus -= ContinuePath;
    }
    private void Update()
    {
        _updateStatus?.Invoke();
        name = _flowGrid.ShowTestVisuals ? PathID + " (" + Coordinates.x + ", " + Coordinates.y + ")" : name;
    }

    #region UpdateStatus
    private void UpdateVisuals()
    {
        GetComponent<Image>().color = CheckIfPathEndpoint(this) || PointType == FlowTileType.Path ? GetColor() : PointType == FlowTileType.Target ? GetIncompleteColor() : Color.white;
    }
    // Tiles that are "active" (Source, Path, Target, Completed)
    private Color GetColor()
    {
        return PathID == 1 ? Color.blue : PathID == 2 ? Color.green : Color.red;
    }
    // For Target tiles that are not yet Completed
    private Color GetIncompleteColor()
    {
        return PathID == 1 ? Color.cyan : PathID == 2 ? Color.yellow : Color.magenta;
    }
    private void UpdateIndicators()
    {
        if (!_flowGrid.ShowTestVisuals)
        {
            _activeIndicator.SetActive(false);
            _currentIndicator.SetActive(false);
            return;
        }

        _activeIndicator.SetActive(PathID != DEFAULT_PATH_ID);
        if (PointType == FlowTileType.Path)
            //_activeIndicator.transform.eulerAngles = Vector3.zero;
            _activeIndicator.SetActive(false);
        else if (CheckIfPathEndpoint(this) || PointType == FlowTileType.Target)
            _activeIndicator.transform.eulerAngles = new Vector3(0, 0, 45);
        else
            _activeIndicator.SetActive(false);
        _currentIndicator.SetActive(this == StartPartial);
    }
    #endregion

    #region MouseInput

    // For multiple tiles in 1 mouse click (click and hold)
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isPointerDown)
            _pointStatus?.Invoke();
    }

    // Same as OnMouseDown, except used to account for clicking and holding
    public void OnPointerDown(PointerEventData eventData)
    {
        _isPointerDown = true;
        SetAsStartPartial(this);
    }

    // Releasing mouse after dragging
    public void OnPointerUp(PointerEventData eventData)
    {
        _isPointerDown = false;
        ResetStartPartial();
    }
    #endregion

    private void ContinuePath()
    {
        if (StartPartial && StartPartialID != DEFAULT_PATH_ID && PreviousPartial)
        {
            // Prevent illegal path drawing (drawing off screen, then back onto grid)
            if (CheckIfNotAdjacentPoints(PreviousPartial, this))
                return;
            // Multiple action cases
            else
            {
                switch (PreviousPartial.PointType)
                {
                    case FlowTileType.Source:
                        SourceLogic(this);
                        break;
                    case FlowTileType.End:
                        EndLogic(this);
                        break;
                    case FlowTileType.Completed:
                        CompletedLogic(this);
                        break;
                    default:
                        break;
                }
                PreviousPartial = this;
            }
        }
    }
}
