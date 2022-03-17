using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public Tile(int posX, int posZ)
    {
        this.posX = posX;
        this.posZ = posZ;

        neighbours = new List<Tile>();
    }

    readonly public int posX;
    readonly public int posZ;
    public float height = 0;
    public bool passable = true;
    public TileComponent tileComponent;
    public List<Tile> neighbours;
    public Unit unit;

    public float DistanceTo(Tile tile)
    {
        return Vector2.Distance(new Vector2(posX, posZ), new Vector2(tile.posX, tile.posZ));
    }
}
