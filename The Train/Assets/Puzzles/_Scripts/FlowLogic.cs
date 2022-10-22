using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FlowGrid;
using static FlowBackend;

// "A -> B" format in logic functions = PreviousPartial -> new FlowTile being "drawn" onto
public class FlowLogic : MonoBehaviour
{
    private FlowGrid _flowGrid;

    private void Awake()
    {
        _flowGrid = FindObjectOfType<FlowGrid>();
    }
    private void Update()
    {
        if (CheckIfGridFilled())
            print(BaseGrid.WIN_MESSAGE);
    }

    #region LogicFunctions
    public static void SourceLogic(FlowTile flowTile)
    {
        if (flowTile.PointType == FlowTileType.Path)
        {
            // Source A -> Path A
            if (CheckSameIDs(flowTile))
            {
                EReset(PreviousPartial);
                NAdd(flowTile);
            }
            // Source A -> Path B
            else
            {
                // Reset any path A beforehand
                EReset(PreviousPartial);

                PSet(flowTile);
                IReset(flowTile);
                NAdd(flowTile);
            }
        }
        else if (flowTile.PointType == FlowTileType.End)
        {
            // Source A -> End A
            if (CheckSameIDs(flowTile))
            {
                EReset(PreviousPartial);
                NAdd(flowTile);
            }
            // Source A -> End B
            else
            {
                // Reset any path A beforehand
                EReset(PreviousPartial);

                PSet(flowTile);
                IReset(flowTile);
                NAdd(flowTile);
            }
        }
        // Source A -> None
        else if (flowTile.PointType == FlowTileType.None)
        {
            // Reset any made paths beforehand
            EReset(PreviousPartial);
            NAdd(flowTile);
        }
        else if (flowTile.PointType == FlowTileType.Target)
        {
            // Source A -> Target A
            if (CheckSameIDs(flowTile))
            {
                NAdd(flowTile);
                SetPoint(flowTile, FlowTileType.Completed);
            }
        }
    }
    public static void EndLogic(FlowTile flowTile)
    {
        if (flowTile.PointType == FlowTileType.Source)
        {
            // End A -> Source A
            if (CheckSameIDs(flowTile))
            {
                EReset(flowTile);
            }
            // End A -> Source B
            else
            {
                StartPartial = flowTile;
                StartPartialID = flowTile.PathID;
            }
        }
        else if (flowTile.PointType == FlowTileType.Path)
        {
            // End A -> Path A
            if (CheckSameIDs(flowTile))
            {
                EReset(flowTile);
                SetPoint(flowTile, FlowTileType.End);
            }
            // End A -> Path B
            else
            {
                PSet(flowTile);
                IReset(flowTile);
                SAdd(flowTile);
            }
        }
        // End A -> End B
        else if (flowTile.PointType == FlowTileType.End)
        {
            PSet(flowTile);
            IReset(flowTile);
            SAdd(flowTile);
        }
        // End A -> None
        else if (flowTile.PointType == FlowTileType.None)
        {
            SAdd(flowTile);
        }
        else if (flowTile.PointType == FlowTileType.Target)
        {
            // End A -> Target A
            if (CheckSameIDs(flowTile))
            {
                TAdd(flowTile);
            }
        }
    }
    public static void CompletedLogic(FlowTile flowTile)
    {
        if (flowTile.PointType == FlowTileType.Source)
        {
            // Completed A -> Source A
            if (CheckSameIDs(flowTile))
            {
                EReset(flowTile);
                // No need to SetPoint(flowTile, FlowTileType.End); when doing EReset (see ResetPoints())
            }
        }
        else if (flowTile.PointType == FlowTileType.Path)
        {
            // Completed A -> Path A
            if (CheckSameIDs(flowTile))
            {
                SetPoint(PreviousPartial, FlowTileType.Target);
                SetPoint(flowTile, FlowTileType.End);
            }
        }
    }
    #endregion

    #region PartialFunctions
    public static void SetAsStartPartial(FlowTile flowTile)
    {
        if (CheckIfPathEndpoint(flowTile) || flowTile.PointType == FlowTileType.Path)
        {
            foreach (KeyValuePair<int, List<FlowTile>> kv in FlowTileTraversal)
            {
                if (kv.Value.Contains(flowTile))
                {
                    StartPartial = flowTile;
                    StartPartialID = flowTile.PathID;
                    PreviousPartial = flowTile;
                    //print(flowTile.name + ": Partials set!");
                    InitialResetPath();
                    return;
                }
            }
            // If start is a Path type, you would need to cut off the path beyond this new start
        }
        //print(flowTile.name + ": Cannot set partials");
    }
    // If starting to draw a path on a Path type point, reset progress and set the StartPartial to the End
    private static void InitialResetPath()
    {
        if (StartPartial.PointType == FlowTileType.Path)
        {
            EReset(StartPartial);
            SetPoint(StartPartial, FlowTileType.End);
        }
    }
    public static void ResetStartPartial()
    {
        StartPartial = null;
        StartPartialID = DEFAULT_PATH_ID;
        PreviousPartial = null;
    }
    #endregion

    public static bool CheckSameIDs(FlowTile flowTile)
    {
        return PreviousPartial.PathID == flowTile.PathID;
    }
    public static bool CheckIfNotAdjacentPoints(FlowTile fp1, FlowTile fp2)
    {
        return Vector2Int.Distance(fp1.Coordinates, fp2.Coordinates) != 1;
    }

    /// <summary>
    /// A path endpoint is classified as either Source, End, or Completed (these will always mark the ends of any path)
    /// </summary>
    public static bool CheckIfPathEndpoint(FlowTile flowTile)
    {
        return flowTile.PointType == FlowTileType.Source || flowTile.PointType == FlowTileType.End || flowTile.PointType == FlowTileType.Completed;
    }
    private bool CheckIfGridFilled()
    {
        int total = 0;
        for (int pathID = 1; pathID <= MAX_PATHS; pathID++)
            total += FlowTileTraversal[pathID].Count;
        return total == _flowGrid.GridSize.x * _flowGrid.GridSize.y;
    }
}
