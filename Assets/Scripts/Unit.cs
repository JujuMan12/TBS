using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    public Unit(Tile tile)
    {
        this.tile = tile;
        tile.unit = this;
    }

    public Tile tile;
    public UnitController unitController;

    public void SetTile(Tile newTile)
    {
        tile.unit = null;

        tile = newTile;
        tile.unit = this;
    }
}
