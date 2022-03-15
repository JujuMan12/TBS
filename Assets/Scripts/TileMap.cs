using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    [HideInInspector] private UnitController selectedUnit;
    [HideInInspector] private Tile[,] tiles;

    [Header("Map")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int mapSizeX = 10;
    [SerializeField] private int mapSizeZ = 10;

    [Header("Impassable")]
    [SerializeField] private Color impassableColor = Color.red;
    [SerializeField] private Vector2[] impassableTiles;

    private void Start()
    {
        GenerateTilesData();
        GenerateTilesVisual();

        selectedUnit = GameObject.FindGameObjectWithTag("PlayerUnit").GetComponent<UnitController>();
    }

    private void GenerateTilesData()
    {
        tiles = new Tile[mapSizeX, mapSizeZ];

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int z = 0; z < mapSizeZ; z++)
            {
                tiles[x, z] = new Tile(this, x, z);
            }
        }

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int z = 0; z < mapSizeZ; z++)
            {
                if (x > 0)
                {
                    tiles[x, z].neighbours.Add(tiles[x - 1, z]);
                }
                if (x < mapSizeX - 1)
                {
                    tiles[x, z].neighbours.Add(tiles[x + 1, z]);
                }
                if (z > 0)
                {
                    tiles[x, z].neighbours.Add(tiles[x, z - 1]);
                }
                if (z < mapSizeZ - 1)
                {
                    tiles[x, z].neighbours.Add(tiles[x, z + 1]);
                }
            }
        }

        foreach (Vector2 pos in impassableTiles)
        {
            tiles[(int)pos.x, (int)pos.y].passable = false;
        }
    }

    private void GenerateTilesVisual()
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int z = 0; z < mapSizeZ; z++)
            {
                GameObject tile = tilePrefab;
                GameObject tileGO = Instantiate(tile, new Vector3(x, tiles[x, z].height, z), Quaternion.Euler(90f, 0f, 0f), gameObject.transform);

                TileComponent tileComponent = tileGO.GetComponent<TileComponent>();
                tileComponent.tileData = tiles[x, z];

                if (!tiles[x, z].passable)
                {
                    tileGO.GetComponent<SpriteRenderer>().color = impassableColor;
                }
            }
        }
    }

    public void GeneratePathTo(int posX, int posZ)
    {
        selectedUnit.currentPath = null;

        Dictionary<Tile, float> distanceTo = new Dictionary<Tile, float>();
        Dictionary<Tile, Tile> prev = new Dictionary<Tile, Tile>();

        List<Tile> uncheckedTiles = new List<Tile>();

        Tile sourceTile = tiles[selectedUnit.tileX, selectedUnit.tileZ];
        Tile targetTile = tiles[posX, posZ];

        distanceTo[sourceTile] = 0;
        prev[sourceTile] = null;

        foreach (Tile tile in tiles)
        {
            if (tile != sourceTile)
            {
                distanceTo[tile] = Mathf.Infinity;
                prev[tile] = null;
            }

            uncheckedTiles.Add(tile);
        }

        while (uncheckedTiles.Count > 0)
        {
            Tile tile = uncheckedTiles[0];

            foreach (Tile possibleTile in uncheckedTiles)
            {
                if (distanceTo[possibleTile] < distanceTo[tile])
                {
                    tile = possibleTile;
                }
            }

            if (tile == targetTile)
            {
                break;
            }

            uncheckedTiles.Remove(tile);

            foreach (Tile neighbour in tile.neighbours)
            {
                float alt = distanceTo[tile] + tile.DistanceTo(neighbour);

                if (alt < distanceTo[neighbour])
                {
                    distanceTo[neighbour] = alt;
                    prev[neighbour] = tile;
                }
            }
        }

        if (prev[targetTile] == null)
        {
            return;
        }

        List<Tile> currentPath = new List<Tile>();
        Tile currentTile = targetTile;

        while (currentTile != null)
        {
            currentPath.Add(currentTile);
            currentTile = prev[currentTile];
        }

        currentPath.Reverse();

        selectedUnit.currentPath = currentPath;

        //selectedUnit.SetNewPosition(new Vector3(posX, 0, posZ));
    }
}
