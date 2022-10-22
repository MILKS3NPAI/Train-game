using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Is not a tile, but will inherit from BaseTile only for Coordinates attribute
public class ArrangePoint : BaseTile
{
    public ArrangeTile TileOccupier;
    public bool IsOccupied;

    private void Update()
    {
        GetComponent<SpriteRenderer>().color = IsOccupied ? Color.green : Color.red;
    }
}
