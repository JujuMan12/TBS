using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    public Unit(Tile tile, int unitId)
    {
        this.tile = tile;
        this.unitId = unitId;
    }

    public Tile tile;
    readonly public int unitId;
}
