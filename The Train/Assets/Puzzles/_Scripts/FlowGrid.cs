using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowGrid : BaseGrid
{
    public const int DEFAULT_PATH_ID = 0;
    public static Dictionary<int, List<FlowTile>> FlowTileTraversal = new Dictionary<int, List<FlowTile>>();
    [Tooltip("The 1st tile every time you click/click and hold the mouse")] public static FlowTile StartPartial;
    [Tooltip("The last tile your cursor entered when clicking and dragging")] public static FlowTile PreviousPartial;
    public static FlowTile[,] FlowTileArray;
    public static int MAX_PATHS = 0;
    [Tooltip("The key of the list that has StartPartial")] public static int StartPartialID;

    [Header("x,y = Source coordinates | z,w = Target coordinates")] [SerializeField] private List<Vector4> _flowTileEndpoints = new List<Vector4>();
    private int _pathID = 1;

    public enum FlowTileType : int
    {
        // Source is the very start of the Path
        // Target is where the path should be connected to
        // End is the end of the currently forming path from Source
        // Path is the stuff between Source and End
        // Completed is Target except its correspondinh Path is connected to it
        None, Source, Target, End, Path, Completed
    }
    private new void Awake()
    {
        base.Awake();
        MAX_PATHS = _flowTileEndpoints.Count;
        FlowTileArray = new FlowTile[GridSize.x, GridSize.y];
        CreateGrid();
        CreateSourcesAndTargets();
    }

    #region Grid
    protected override void CreateGrid()
    {
        for (int i = 0; i < GridSize.x; i++)
            for (int j = 0; j < GridSize.y; j++)
                CreateGridTile(i, j);
    }
    protected override void CreateGridPoint(int x, int y)
    {
        throw new System.NotImplementedException();
    }
    protected override void CreateGridTile(int x, int y)
    {
        GameObject newPoint = Instantiate(SpawnablePrefab);
        newPoint.name = "Flow Tile (" + x + ", " + y + ")";
        newPoint.transform.position = GridPointDistance * (new Vector2(x, y) - new Vector2(GridSize.x, GridSize.y) / 2 + 0.5f * Vector2.one);
        newPoint.transform.parent = GridPointCanvas.transform;
        newPoint.transform.localScale = SpawnablePrefab.transform.localScale;

        FlowTile newFlowTile = newPoint.GetComponent<FlowTile>();
        newFlowTile.Coordinates = new Vector2Int(x, y);
        newFlowTile.PointType = FlowTileType.None;
        newFlowTile.PathID = DEFAULT_PATH_ID;
        FlowTileArray[x, y] = newFlowTile;
        newPoint.SetActive(true);
    }
    #endregion

    protected void CreateSourcesAndTargets()
    {
        foreach (Vector4 vector4 in _flowTileEndpoints)
        {
            FlowTileTraversal.Add(_pathID, new List<FlowTile>());
            CreateSourceTile((int)vector4.x, (int)vector4.y);
            CreateTargetTile((int)vector4.z, (int)vector4.w);
            _pathID++;
        }
    }
    private void CreateSourceTile(int x, int y)
    {
        FlowTile sourceTile = FlowTileArray[x, y];
        sourceTile.PointType = FlowTileType.Source;
        sourceTile.PathID = _pathID;
        FlowTileTraversal[_pathID].Add(sourceTile);
    }
    private void CreateTargetTile(int x, int y)
    {
        FlowTile targetTile = FlowTileArray[x, y];
        targetTile.PointType = FlowTileType.Target;
        targetTile.PathID = _pathID;
    }
}
