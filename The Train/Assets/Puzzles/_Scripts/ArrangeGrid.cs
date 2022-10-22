using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrangeGrid : BaseGrid
{
    public static ArrangePoint[,] ArrangePointArray;
    // Used to make the selected/dragged tile appear on the layer above the other tiles
    public Canvas HoverCanvas;

    [Header("Accounts for small scaling in camera view")] [Tooltip("Accounts for small scaling in camera view")] [SerializeField] private float _overallScale;
    [Header("If changing GridPointDistance, likely need to change this")] [SerializeField] private float _tileScale;
    [SerializeField] private List<ArrangeTile> _arrangeTilesList = new List<ArrangeTile>();
    [SerializeField] private int _tileSpawnAmount;

    private new void Awake()
    {
        base.Awake();
        ArrangePointArray = new ArrangePoint[GridSize.x, GridSize.y];
        // Set to preset values if given ones are not valid
        _overallScale = _overallScale <= 0 ? 1 : _overallScale;
        _tileScale = _tileScale <= 0 ? 3.25f : _tileScale;
        _tileSpawnAmount = _tileSpawnAmount <= 0 ? 5 : _tileSpawnAmount;
        CreateGrid();
        CreateGridTiles();
    }

    #region Grid
    protected override void CreateGrid()
    {
        for (int j = GridSize.y - 1; j > -1; j--)
            for (int i = 0; i < GridSize.x; i++)
                CreateGridPoint(i, j);
    }
    protected override void CreateGridPoint(int x, int y)
    {
        GameObject newTile = Instantiate(SpawnablePrefab);
        newTile.name = "Arrange Point (" + x + ", " + y + ")";
        newTile.transform.position = _overallScale * GridPointDistance * (new Vector2(x, y) - new Vector2(GridSize.x, GridSize.y) / 2 + 0.5f * Vector2.one);
        newTile.transform.SetParent(GridPointParent);
        newTile.transform.localScale *= _overallScale;

        newTile.GetComponent<ArrangePoint>().Coordinates = new Vector2Int(x, y);
        ArrangePointArray[x, y] = newTile.GetComponent<ArrangePoint>();
        newTile.SetActive(true);
    }
    protected override void CreateGridTile(int x, int y)
    {
        ArrangeTile newArrangeTile = _arrangeTilesList[Random.Range(0, _arrangeTilesList.Count)];

        List<ArrangePoint> possibleSpawns = FindAllValidPoints(FindAllUnoccupiedPoints(), newArrangeTile);
        if (possibleSpawns.Count == 0)
            return;
        ArrangePoint spawnPoint = possibleSpawns[Random.Range(0, possibleSpawns.Count)];

        GameObject newTile = Instantiate(newArrangeTile.gameObject);
        newTile.transform.SetParent(GridTileParent);
        newTile.transform.localScale = _overallScale * _tileScale * Vector3.one;
        newTile.SetActive(true);

        // This must be done or else you are not accessing the instantiated game object
        newArrangeTile = newTile.GetComponent<ArrangeTile>();
        newArrangeTile.ClosestArrangePoint = spawnPoint;
        newArrangeTile.Coordinates = spawnPoint.Coordinates;
        newArrangeTile.SetCurrentOccupancy(true);
    }
    private void CreateGridTiles()
    {
        for (int i = 0; i < _tileSpawnAmount; i++)
            // Dummy ints
            CreateGridTile(-1, -1);
    }
    #endregion

    private List<ArrangePoint> FindAllUnoccupiedPoints()
    {
        List<ArrangePoint> unoccupiedPoints = new List<ArrangePoint>();
        foreach (ArrangePoint arrangePoint in ArrangePointArray)
            if (!arrangePoint.IsOccupied)
                unoccupiedPoints.Add(arrangePoint);
        return unoccupiedPoints;
    }
    // Finds list of all possible unoccupied points where the given parameter tile can spawn (without colliding with other tiles)
    private List<ArrangePoint> FindAllValidPoints(List<ArrangePoint> unoccupiedPoints, ArrangeTile tileToSpawn)
    {
        List<ArrangePoint> validPoints = new List<ArrangePoint>();
        foreach (ArrangePoint arrangePoint in unoccupiedPoints)
            if (CheckIfPointIsValid(arrangePoint, tileToSpawn))
                validPoints.Add(arrangePoint);
        return validPoints;
    }
    public bool CheckIfPointIsValid(ArrangePoint arrangePoint, ArrangeTile arrangeTile)
    {
        foreach (Vector2Int offset in arrangeTile.AllTilePositions)
        {
            Vector2Int position = arrangePoint.Coordinates + offset;
            if (!CheckForValidIndexes(position.x, position.y) || ArrangePointArray[position.x, position.y].IsOccupied)
                return false;
        }
        return true;
    }
}
