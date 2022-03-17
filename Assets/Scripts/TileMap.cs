using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    [HideInInspector] public UnitController selectedUnit;
    [HideInInspector] private List<Tile> availableTiles;
    [HideInInspector] private Transform unitFolder;
    [HideInInspector] private Tile[,] tiles;
    [HideInInspector] private List<Unit> units;

    [Header("Map")]
    [SerializeField] private int mapSizeX = 10;
    [SerializeField] private int mapSizeZ = 10;

    [Header("Tiles")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Vector2[] impassableTiles;

    [Header("Units")]
    [SerializeField] private GameObject unitPrefab;

    private void Start()
    {
        unitFolder = GameObject.FindGameObjectWithTag("UnitFolder").transform;

        GenerateTilesData();
        GenerateTilesVisual();
        GenerateUnitsData();

        //SelectUnit(GameObject.FindGameObjectWithTag("PlayerUnit").GetComponent<UnitController>());
    }

    private void GenerateTilesData()
    {
        tiles = new Tile[mapSizeX, mapSizeZ];
        availableTiles = new List<Tile>();

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int z = 0; z < mapSizeZ; z++)
            {
                tiles[x, z] = new Tile(x, z);
            }
        }

        foreach (Vector2 pos in impassableTiles)
        {
            tiles[(int)pos.x, (int)pos.y].passable = false;
        }

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int z = 0; z < mapSizeZ; z++)
            {
                if (x > 0 && tiles[x - 1, z].passable)
                {
                    tiles[x, z].neighbours.Add(tiles[x - 1, z]);
                }
                if (x < mapSizeX - 1 && tiles[x + 1, z].passable)
                {
                    tiles[x, z].neighbours.Add(tiles[x + 1, z]);
                }
                if (z > 0 && tiles[x, z - 1].passable)
                {
                    tiles[x, z].neighbours.Add(tiles[x, z - 1]);
                }
                if (z < mapSizeZ - 1 && tiles[x, z + 1].passable)
                {
                    tiles[x, z].neighbours.Add(tiles[x, z + 1]);
                }
            }
        }
    }

    private void GenerateTilesVisual()
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int z = 0; z < mapSizeZ; z++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(x, tiles[x, z].height, z), Quaternion.Euler(90f, 0f, 0f), gameObject.transform);
                TileComponent tileComponent = tile.GetComponent<TileComponent>();

                tileComponent.tileData = tiles[x, z];
                tiles[x, z].tileComponent = tileComponent;

                if (!tiles[x, z].passable)
                {
                    tileComponent.colorState = TileComponent.ColorState.impassable;
                }
            }
        }
    }

    private void GenerateUnitsData()
    {
        units = new List<Unit>();

        for (int i = 0; i < 2; i++) //TODO: unit generation
        {
            units.Add(new Unit(tiles[i * 5, i]));

            GameObject unit = Instantiate(unitPrefab, new Vector3(i * 5, 0, i), Quaternion.identity, unitFolder);
            unit.GetComponent<UnitController>().SetUnitData(units[i]);

            units[i].tile.tileComponent.colorState = TileComponent.ColorState.ally;
        }
    }

    public void GeneratePathTo(int posX, int posZ) //TODO
    {
        if (selectedUnit.currentPath != null)
        {
            ClearCurrentPath();
        }
        if (availableTiles != null)
        {
            ClearAvailableTiles();
        }

        Dictionary<Tile, float> distanceTo = new Dictionary<Tile, float>();
        Dictionary<Tile, Tile> prevTile = new Dictionary<Tile, Tile>();
        List<Tile> uncheckedTiles = new List<Tile>();

        foreach (Tile tile in tiles)
        {
            distanceTo[tile] = Mathf.Infinity;
            prevTile[tile] = null;
            uncheckedTiles.Add(tile);
        }

        Tile sourceTile = tiles[selectedUnit.unitData.tile.posX, selectedUnit.unitData.tile.posZ];
        Tile targetTile = tiles[posX, posZ];
        distanceTo[sourceTile] = 0;

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
                if (distanceTo[tile] + 1 < distanceTo[neighbour] && neighbour.unit == null)
                {
                    distanceTo[neighbour] = distanceTo[tile] + 1;
                    prevTile[neighbour] = tile;
                }
            }
        }

        if (prevTile[targetTile] == null)
        {
            return;
        }

        List<Tile> currentPath = new List<Tile>();
        Tile currentTile = targetTile;

        while (currentTile != null)
        {
            currentPath.Add(currentTile);
            currentTile = prevTile[currentTile];
        }

        currentPath.Reverse();

        selectedUnit.currentPath = currentPath;
        HighlightCurrentPath();
    }

    private void ClearCurrentPath()
    {
        foreach (Tile tile in selectedUnit.currentPath)
        {
            tile.tileComponent.colorState = TileComponent.ColorState.none;
        }

        selectedUnit.currentPath = null;
        selectedUnit.currentPathId = 0;
    }

    public void ClearAvailableTiles()
    {
        foreach (Tile tile in availableTiles)
        {
            tile.tileComponent.colorState = TileComponent.ColorState.none;
        }

        availableTiles.Clear();
    }

    public void HighlightSelectedUnit()
    {
        if (selectedUnit.currentPath == null)
        {
            selectedUnit.unitData.tile.tileComponent.colorState = TileComponent.ColorState.selected;
            HighlightAvailableTiles(selectedUnit.unitData.tile, selectedUnit.actionPoints);
        }
    }

    private void HighlightAvailableTiles(Tile currentTile, int actionPoints)
    {
        availableTiles.Add(currentTile);

        if (actionPoints > 0)
        {
            foreach (Tile neighbour in currentTile.neighbours)
            {
                if (!availableTiles.Contains(neighbour) && neighbour.unit == null)
                {
                    neighbour.tileComponent.colorState = TileComponent.ColorState.available;
                    HighlightAvailableTiles(neighbour, actionPoints - 1);
                }
            }
        }
    }

    private void HighlightCurrentPath()
    {
        foreach (Tile tile in selectedUnit.currentPath)
        {
            tile.tileComponent.colorState = TileComponent.ColorState.path;
        }
    }

    public void SelectUnit(UnitController unit)
    {
        if (selectedUnit != null)
        {
            ClearAvailableTiles();
            selectedUnit.unitData.tile.tileComponent.colorState = TileComponent.ColorState.ally;
        }

        selectedUnit = unit;
        HighlightSelectedUnit();
    }
}
