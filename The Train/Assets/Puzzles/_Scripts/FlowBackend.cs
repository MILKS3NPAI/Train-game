using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FlowGrid;

/// <summary>
/// Backend functions; Should ONLY be accessible to FlowLogic
/// </summary>
public class FlowBackend : MonoBehaviour
{
    #region ResetFunctions

    /// <summary>
    /// Reset all points within a list starting at the removal index (pathID is the key to access the list)
    /// </summary>
    public static void ResetPoints(int pathID, int removalStartIndex)
    {
        // pathID keeps a temporary constant ID as you would be seting it to 0 below, which is not a valid path ID
        // removalStartIndex: Index of the element that has been looped back to in your path;
        // Makes sure it is accessing a constant count
        int tempCount = FlowTileTraversal[pathID].Count;
        for (int i = removalStartIndex; i < tempCount; i++)
        {
            if (FlowTileTraversal[pathID][removalStartIndex].PointType == FlowTileType.Completed)
            {
                FlowTileTraversal[pathID][removalStartIndex].PointType = FlowTileType.Target;
                //FlowTileTraversal[pathID][removalStartIndex].PathID = DEFAULT_PATH_ID;
            }
            else if (!CheckIfTargetOrSource(FlowTileTraversal[pathID][removalStartIndex]))
            {
                FlowTileTraversal[pathID][removalStartIndex].PointType = FlowTileType.None;
                FlowTileTraversal[pathID][removalStartIndex].PathID = DEFAULT_PATH_ID;
            }
            FlowTileTraversal[pathID].RemoveAt(removalStartIndex);
        }
    }

    /// <summary>
    /// Reset everything beyond, including, the parameter point
    /// </summary>
    public static void IReset(FlowTile flowTile)
    {
        ResetPoints(flowTile.PathID, FlowTileTraversal[flowTile.PathID].IndexOf(flowTile));
    }

    /// <summary>
    /// Reset everything beyond, excluding, the parameter point
    /// </summary>
    public static void EReset(FlowTile flowTile)
    {
        ResetPoints(flowTile.PathID, FlowTileTraversal[StartPartialID].IndexOf(flowTile) + 1);
    }
    #endregion

    #region AddFunctions
    public static void AddPoint(FlowTile flowTile, FlowTileType pointType)
    {
        flowTile.PathID = PreviousPartial.PathID;
        SetPoint(flowTile, pointType);
        FlowTileTraversal[StartPartialID].Add(flowTile);
    }

    /// <summary>
    /// Add the parameter point as the path End
    /// </summary>
    public static void NAdd(FlowTile flowTile)
    {
        AddPoint(flowTile, FlowTileType.End);
    }

    /// <summary>
    /// Set the previous point as a Path and set the parameter point as the new path End
    /// </summary>
    public static void SAdd(FlowTile flowTile)
    {
        SetPoint(PreviousPartial, FlowTileType.Path);
        NAdd(flowTile);
    }

    /// <summary>
    /// Set the previous point as a Path and set the parameter point as Completed if it's a Target
    /// </summary>
    public static void TAdd(FlowTile flowTile)
    {
        SetPoint(PreviousPartial, FlowTileType.Path);
        AddPoint(flowTile, FlowTileType.Completed);
    }
    #endregion

    #region SetFunctions
    public static void SetPoint(FlowTile flowTile, FlowTileType pointType)
    {
        flowTile.PointType = pointType;
    }

    /// <summary>
    /// Set the point previous to the parameter point to the path End
    /// </summary>
    public static void PSet(FlowTile flowTile)
    {
        if (FlowTileTraversal[flowTile.PathID].Contains(flowTile) && FlowTileTraversal[flowTile.PathID].IndexOf(flowTile) > 0 &&
            FlowTileTraversal[flowTile.PathID][FlowTileTraversal[flowTile.PathID].IndexOf(flowTile) - 1].PointType == FlowTileType.Path)
        {
            SetPoint(FlowTileTraversal[flowTile.PathID][FlowTileTraversal[flowTile.PathID].IndexOf(flowTile) - 1], FlowTileType.End);
        }
    }
    #endregion

    private static bool CheckIfTargetOrSource(FlowTile flowTile)
    {
        return flowTile.PointType == FlowTileType.Source || flowTile.PointType == FlowTileType.Target || flowTile.PointType == FlowTileType.Completed;
    }
}
