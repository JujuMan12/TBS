using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    [HideInInspector] public UnitController selectedUnit;
    [HideInInspector] public enum ActionStates { movement, attack, defence }
    [HideInInspector] public ActionStates actionState = ActionStates.movement;
    [HideInInspector] public List<Tile> highlightedTiles;
    [HideInInspector] private Transform unitFolder;
    [HideInInspector] public Tile[,] tiles;
    [HideInInspector] public List<Unit> units;

    [Header("Map")]
    [SerializeField] private int mapSizeX = 20;
    [SerializeField] private int mapSizeZ = 20;

    [Header("Tiles")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Vector2[] impassableTiles;

    [Header("Units")]
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int playerUnitNumber = 10;
    [SerializeField] private int enemyUnitNumber = 10;
    [SerializeField] private int unitSpawnZMax = 6;

    [Header("AI")]
    [SerializeField] public bool isPlayerTurn = true;

    private void Start()
    {
        unitFolder = GameObject.FindGameObjectWithTag("UnitFolder").transform;

        GenerateTilesData();
        GenerateTilesVisual();
        GenerateUnitsData();
        GenerateUnitsVisual();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel") && selectedUnit != null)
        {
            UnselectUnit();
        }
    }

    private void FixedUpdate()
    {
        if (!units.Exists(unit => unit.isPlayerOwned) || !units.Exists(unit => !unit.isPlayerOwned)) //TODO: rework
        {
            Application.Quit();
        }
    }

    private void GenerateTilesData()
    {
        tiles = new Tile[mapSizeX, mapSizeZ];
        highlightedTiles = new List<Tile>();

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

        for (int i = 0; i < playerUnitNumber; i++)
        {
            while (true)
            {
                int randomX = Random.Range(0, mapSizeX);
                int randomZ = Random.Range(0, unitSpawnZMax);

                if (tiles[randomX, randomZ].unit == null && tiles[randomX, randomZ].passable)
                {
                    units.Add(new Unit(tiles[randomX, randomZ], true));
                    break;
                }
            }
        }

        for (int i = 0; i < enemyUnitNumber; i++)
        {
            while (true)
            {
                int randomX = Random.Range(0, mapSizeX);
                int randomZ = Random.Range(mapSizeZ - unitSpawnZMax, mapSizeZ);

                if (tiles[randomX, randomZ].unit == null && tiles[randomX, randomZ].passable)
                {
                    units.Add(new Unit(tiles[randomX, randomZ], false));
                    break;
                }
            }
        }
    }

    private void GenerateUnitsVisual()
    {
        foreach (Unit unit in units)
        {
            GameObject prefab;
            Quaternion rotation;
            TileComponent.ColorState colorState;

            if (unit.isPlayerOwned)
            {
                prefab = unitPrefab;
                rotation = Quaternion.Euler(0f, 0f, 0f);
                colorState = TileComponent.ColorState.ally;
            }
            else
            {
                prefab = enemyPrefab;
                rotation = Quaternion.Euler(0f, 180f, 0f);
                colorState = TileComponent.ColorState.enemy;
            }

            GameObject unitGO = Instantiate(prefab, new Vector3(unit.tile.posX, unit.tile.height, unit.tile.posZ), rotation, unitFolder);
            unitGO.GetComponent<UnitController>().SetUnitData(unit);
            unit.tile.tileComponent.colorState = colorState;
        }
    }

    public void GeneratePathTo(int posX, int posZ, UnitController unit, bool canIgnoreActionPoints)
    {
        Dictionary<Tile, float> distanceTo = new Dictionary<Tile, float>();
        Dictionary<Tile, Tile> prevTile = new Dictionary<Tile, Tile>();
        List<Tile> uncheckedTiles = new List<Tile>();

        foreach (Tile tile in tiles)
        {
            distanceTo[tile] = Mathf.Infinity;
            prevTile[tile] = null;
            uncheckedTiles.Add(tile);
        }

        Tile sourceTile = tiles[unit.unitData.tile.posX, unit.unitData.tile.posZ];
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
                if (distanceTo[tile] + 1 < distanceTo[neighbour] && (neighbour.unit == null || neighbour == targetTile))
                {
                    distanceTo[neighbour] = distanceTo[tile] + 1;
                    prevTile[neighbour] = tile;
                }
            }
        }

        if (prevTile[targetTile] == null || distanceTo[targetTile] > unit.actionPoints && !canIgnoreActionPoints)
        {
            //TODO: special sound
            return;
        }

        if (unit.currentPath != null)
        {
            ClearCurrentPath();
        }
        if (highlightedTiles != null)
        {
            ClearHighlightedTiles();
        }

        List<Tile> currentPath = new List<Tile>();
        Tile currentTile = targetTile;

        while (currentTile != null)
        {
            currentPath.Add(currentTile);
            currentTile = prevTile[currentTile];
        }

        currentPath.Reverse();

        unit.currentPath = currentPath;
        unit.unitData.tile.tileComponent.colorState = TileComponent.ColorState.path; //TODO
        unit.HighlightCurrentPath();
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

    public void ClearHighlightedTiles()
    {
        foreach (Tile tile in highlightedTiles)
        {
            tile.tileComponent.colorState = TileComponent.ColorState.none;
        }

        highlightedTiles.Clear();
    }

    public void HighlightSelectedUnit()
    {
        if (selectedUnit != null && selectedUnit.currentPath == null)
        {
            ClearHighlightedTiles();
            Tile unitTile = selectedUnit.unitData.tile;

            highlightedTiles.Add(unitTile);
            unitTile.tileComponent.colorState = TileComponent.ColorState.selected;

            if (actionState == ActionStates.movement)
            {
                HighlightNeighbourTiles(unitTile, selectedUnit.actionPoints, TileComponent.ColorState.available);
            }
            else if (actionState == ActionStates.attack)
            {
                HighlightNeighbourTiles(unitTile, selectedUnit.attackRange, TileComponent.ColorState.enemy);
            }
            else
            {
                highlightedTiles.Add(unitTile);
                unitTile.tileComponent.colorState = TileComponent.ColorState.available;
            }
        }
    }

    private void HighlightNeighbourTiles(Tile currentTile, int range, TileComponent.ColorState colorState)
    {
        if (range > 0)
        {
            foreach (Tile neighbour in currentTile.neighbours)
            {
                if ( neighbour.unit == null)
                {
                    highlightedTiles.Add(neighbour);
                    neighbour.tileComponent.colorState = colorState;
                    HighlightNeighbourTiles(neighbour, range - 1, colorState);
                }
            }
        }
    }

    public void SelectUnit(UnitController unit)
    {
        UnitController prevSelectedUnit = selectedUnit;

        selectedUnit = unit;
        SetActionState(ActionStates.movement);

        if (prevSelectedUnit != null && prevSelectedUnit.currentPath == null)
        {
            prevSelectedUnit.unitData.tile.tileComponent.colorState = TileComponent.ColorState.ally;
        }
    }

    private void UnselectUnit()
    {
        ClearHighlightedTiles();

        if (selectedUnit.currentPath == null)
        {
            selectedUnit.unitData.tile.tileComponent.colorState = TileComponent.ColorState.ally;
        }

        selectedUnit = null;
    }

    public void SetActionState(ActionStates newState)
    {
        actionState = newState;
        HighlightSelectedUnit();
    }

    public void ResetPlayerTurn()
    {
        isPlayerTurn = true;

        foreach (Unit unit in units)
        {
            unit.unitController.ResetActionPoints();
        }
    }
}
