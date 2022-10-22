using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGrid : MonoBehaviour
{
    public const string WIN_MESSAGE = "You win :(";
    public bool ShowTestVisuals;
    public Canvas GridPointCanvas;
    public Vector2Int GridSize;

    [SerializeField] protected float ScreenHeightLeeway, CameraOrthographicSize;
    // This value is likely correlated to the sprite size of the arrange tiles (currently 30x30)
    [Header("(Arrange only: CHANGE WITH CAUTION)")] [SerializeField] protected float GridPointDistance;
    [SerializeField] protected GameObject SpawnablePrefab;
    [SerializeField] protected Transform GridPointParent, GridTileParent;

    protected void Awake()
    {
        // Set to preset values if given values are not valid
        GridSize = GridSize.x <= 0 && GridSize.y <= 0 ? 5 * Vector2Int.one : GridSize;
        GridPointDistance = GridPointDistance <= 0 ? 1f : GridPointDistance;
        //Camera.main.orthographicSize = GridPointDistance * 4;
        ScreenHeightLeeway = ScreenHeightLeeway <= 0 ? 0.05f : ScreenHeightLeeway;

        // Make grid height fit exactly onto screen
        CameraOrthographicSize = CameraOrthographicSize <= 0 ? GridSize.y * (0.5f + ScreenHeightLeeway) : CameraOrthographicSize;
        Camera.main.orthographicSize = CameraOrthographicSize;
    }
    protected abstract void CreateGrid();
    protected abstract void CreateGridPoint(int x, int y);
    protected abstract void CreateGridTile(int x, int y);
    public bool CheckForValidIndexes(int x, int y)
    {
        return x > -1 && x < GridSize.x && y > -1 && y < GridSize.y;
    }
}
