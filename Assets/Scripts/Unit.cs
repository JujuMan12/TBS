using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    public Unit(Tile tile, bool isPlayerOwned)
    {
        this.tile = tile;
        tile.unit = this;
        this.isPlayerOwned = isPlayerOwned;
    }

    public Tile tile;
    public UnitController unitController;
    readonly public bool isPlayerOwned;

    public void SetTile(Tile newTile)
    {
        tile.unit = null;

        tile = newTile;
        tile.unit = this;
    }
}
