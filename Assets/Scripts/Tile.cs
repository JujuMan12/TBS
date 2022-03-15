using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public Tile(TileMap tileMap, int posX, int posZ)
    {
        this.tileMap = tileMap;
        this.posX = posX;
        this.posZ = posZ;
    }

    public TileMap tileMap;
    public int posX;
    public int posZ;
    public float height = 0;
    public bool passable = true;
}
