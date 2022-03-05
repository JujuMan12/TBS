using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public Tile(TileMap tileMap)
    {
        this.tileMap = tileMap;
    }

    private TileMap tileMap;
    public float height = 0;
    public bool passable = true;
}
