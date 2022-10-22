using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideGrid : BaseGrid
{
    public static bool HasFinishedScrambling;
    public static List<SlideTile> MovableTiles = new List<SlideTile>(), AllTiles = new List<SlideTile>();
    [HideInInspector] public SlideTile EmptySlideTile;

    [SerializeField] private float _tileMovementSpeed;
    [Header("To prevent being auto-solved, make this odd")] [SerializeField] private int _slideTileSwapAmount;
    [SerializeField] private List<Sprite> _slideTileNumbers = new List<Sprite>();
    private bool _isTileMoving;
    private int _tileNumber = 0;
    private Queue<SlideTile> _playerInputs;

    private new void Awake()
    {
        ScreenHeightLeeway = ScreenHeightLeeway <= 0 ? 0.05f : ScreenHeightLeeway;
        _tileMovementSpeed = _tileMovementSpeed <= 0 ? 0.3f : _tileMovementSpeed;
        HasFinishedScrambling = false;
        _playerInputs = new Queue<SlideTile>();
        Camera.main.orthographicSize = GridSize.y * (0.5f + ScreenHeightLeeway);
        CreateGrid();
        UpdateTileMovability();
        StartCoroutine(ScrambleTiles());
    }
    private void Update()
    {
        if (CheckIfComplete() && HasFinishedScrambling)
            print(WIN_MESSAGE);
    }
    protected override void CreateGrid()
    {
        for (int j = GridSize.y - 1; j > -1; j--)
            for (int i = 0; i < GridSize.x; i++)
                CreateGridTile(i, j);
    }
    protected override void CreateGridPoint(int x, int y)
    {
        throw new System.NotImplementedException();
    }
    protected override void CreateGridTile(int x, int y)
    {
        GameObject newTile = Instantiate(SpawnablePrefab);
        newTile.name = "Slide Tile #" + (_tileNumber + 1) + " (" + x + ", " + y + ")";
        newTile.transform.position = -0.5f * Vector2.one * (GridSize - Vector2.one) + new Vector2(x, y);
        newTile.transform.SetParent(GridTileParent);

        newTile.GetComponent<SpriteRenderer>().sprite = _slideTileNumbers[_tileNumber];
        newTile.GetComponent<BoxCollider2D>().size = Vector2.one;
        newTile.GetComponent<Image>().sprite = _slideTileNumbers[_tileNumber++];

        SlideTile newSlideTile = newTile.GetComponent<SlideTile>();
        newSlideTile.OnTileClicked += AddTileToQueue;
        newSlideTile.OnMovementFinished += OnTileMovementFinished;
        newSlideTile.Coordinates = new Vector2Int(x, y);
        newSlideTile.InitialCoordinates = newSlideTile.Coordinates;
        AllTiles.Add(newSlideTile);

        // Tile 16
        if (x == GridSize.x - 1 && y == 0)
        {
            newTile.SetActive(false);
            EmptySlideTile = newSlideTile;
        }
    }

    #region Movement
    private void AddTileToQueue(SlideTile tileToMove)
    {
        _playerInputs.Enqueue(tileToMove);
        MakeNextPlayerMove();
    }
    // Reads player input to move tiles
    private void MoveSlideTile(SlideTile tileToMove)
    {
        // Allows game to work even when scaling tiles (do not use transform.position here)
        if ((tileToMove.Coordinates - EmptySlideTile.Coordinates).sqrMagnitude == 1)
        {
            Vector2Int emptyTileCoordinates = EmptySlideTile.Coordinates;
            EmptySlideTile.Coordinates = tileToMove.Coordinates;
            tileToMove.Coordinates = emptyTileCoordinates;

            Vector2 targetPosition = EmptySlideTile.transform.position;
            EmptySlideTile.transform.position = tileToMove.transform.position;

            if (HasFinishedScrambling)
            {
                tileToMove.StartMovement(targetPosition, _tileMovementSpeed);
                _isTileMoving = true;
            }
            else
                tileToMove.transform.position = targetPosition;
        }
    }
    private void OnTileMovementFinished()
    {
        _isTileMoving = false;
        MakeNextPlayerMove();
    }
    private void MakeNextPlayerMove()
    {
        while (_playerInputs.Count > 0 && !_isTileMoving)
            MoveSlideTile(_playerInputs.Dequeue());
    }
    #endregion

    #region Scramble
    private IEnumerator ScrambleTiles()
    {
        for (int i = 0; i < _slideTileSwapAmount; i++)
        {
            yield return new WaitUntil(() => !_isTileMoving);
            SlideTile tileToMove = MovableTiles[Random.Range(0, MovableTiles.Count)];
            //print((i + 1) + ": Moving " + tileToMove.name);
            MoveSlideTile(tileToMove);
            UpdateTileMovability();
        }
        HasFinishedScrambling = true;
    }
    private void UpdateTileMovability()
    {
        MovableTiles = new List<SlideTile>();
        foreach (SlideTile slideTile in AllTiles)
            slideTile.UpdateMovability();
    }
    private bool CheckIfComplete()
    {
        foreach (SlideTile slideTile in AllTiles)
            if (slideTile.Coordinates != slideTile.InitialCoordinates)
                return false;
        return true;
    }
    #endregion
}
