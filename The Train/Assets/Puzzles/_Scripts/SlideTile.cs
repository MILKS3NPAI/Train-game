using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideTile : BaseTile
{
    public event System.Action<SlideTile> OnTileClicked;
    public event System.Action OnMovementFinished;

    public bool IsMovable;
    public Vector2Int InitialCoordinates;

    private SlideGrid _slideGrid;

    private void Awake()
    {
        _slideGrid = FindObjectOfType<SlideGrid>();
    }
    private void Update()
    {
        // To allow updating movability correctly
        if (SlideGrid.HasFinishedScrambling)
            UpdateMovability();
        if (!_slideGrid.ShowTestVisuals)
            return;

        transform.eulerAngles = IsMovable ? new Vector3(0, 0, 45) : Vector3.zero;
    }
    private void OnMouseDown()
    {
        OnTileClicked?.Invoke(this);
    }
    public void StartMovement(Vector2 targetPosition, float duration)
    {
        StartCoroutine(AnimateMovement(targetPosition, duration));
    }
    private IEnumerator AnimateMovement(Vector2 targetPosition, float duration)
    {
        Vector2 initialPosition = transform.position;
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime / duration;
            transform.position = Vector2.Lerp(initialPosition, targetPosition, percent);
            yield return null;
        }
        OnMovementFinished?.Invoke();
    }
    
    private bool CheckNeighborTile(int x, int y)
    {
        // Check if neighbor tile exists and if it is exactly adjacent to it
        return Coordinates.x + x > -1 && Coordinates.x + x < _slideGrid.GridSize.x && Coordinates.y + y > -1 && Coordinates.y + y < _slideGrid.GridSize.y &&
            Vector2Int.Distance(_slideGrid.EmptySlideTile.Coordinates, Coordinates) == 1;
    }
    public void UpdateMovability() {
        if (CheckNeighborTile(0, 1) || CheckNeighborTile(1, 0) || CheckNeighborTile(0, -1) || CheckNeighborTile(-1, 0))
            SlideGrid.MovableTiles.Add(this);
    }
}
