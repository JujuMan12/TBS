using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    public Unit(Tile tile, bool isPlayerOwned, int unitType)
    {
        this.tile = tile;
        tile.unit = this;
        this.isPlayerOwned = isPlayerOwned;
        this.unitType = unitType;
    }

    public Tile tile;
    public UnitController unitController;
    readonly public bool isPlayerOwned;
    readonly public int unitType;

    public void SetTile(Tile newTile)
    {
        tile.unit = null;

        tile = newTile;
        tile.unit = this;
    }
}
