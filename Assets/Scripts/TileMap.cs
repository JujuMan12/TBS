using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    [HideInInspector] private Tile[,] tiles;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject nullTilePrefab;
    [SerializeField] private int mapSizeX = 10;
    [SerializeField] private int mapSizeY = 10;

    private void Start()
    {
        tiles = new Tile[mapSizeX, mapSizeY];

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                tiles[x, y] = new Tile(this);
            }
        }

        tiles[1, 2].passable = false;

        GenerateTilesVisual();
    }

    private void GenerateTilesVisual()
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                GameObject tile;

                if (tiles[x, y].passable)
                {
                    tile = tilePrefab;
                }
                else
                {
                    tile = nullTilePrefab;
                }

                Instantiate(tile, new Vector3(x, tiles[x, y].height, y), Quaternion.Euler(90f, 0f, 0f), gameObject.transform);
            }
        }
    }
}
