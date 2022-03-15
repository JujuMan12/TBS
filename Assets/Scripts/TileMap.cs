using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    [HideInInspector] private UnitController selectedUnit;
    [HideInInspector] private Tile[,] tiles;
    [HideInInspector] private Node[,] nodes;

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
        GeneratePathfindingGraph();
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

        foreach (Vector2 pos in impassableTiles)
        {
            tiles[(int)pos.x, (int)pos.y].passable = false;
        }
    }

    private void GeneratePathfindingGraph()
    {
        nodes = new Node[mapSizeX, mapSizeZ];

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int z = 0; z < mapSizeZ; z++)
            {
                nodes[x, z] = new Node();
                nodes[x, z].posX = x;
                nodes[x, z].posZ = z;
            }
        }

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int z = 0; z < mapSizeZ; z++)
            {
                if (x > 0)
                {
                    nodes[x, z].neighbours.Add(nodes[x - 1, z]);
                }
                if (x < mapSizeX - 1)
                {
                    nodes[x, z].neighbours.Add(nodes[x + 1, z]);
                }
                if (z > 0)
                {
                    nodes[x, z].neighbours.Add(nodes[x, z - 1]);
                }
                if (z < mapSizeZ - 1)
                {
                    nodes[x, z].neighbours.Add(nodes[x, z + 1]);
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

        Dictionary<Node, float> distanceTo = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

        List<Node> uncheckedNodes = new List<Node>();

        Node sourceNode = nodes[selectedUnit.tilePositionX, selectedUnit.tilePositionZ];
        Node targetNode = nodes[posX, posZ];

        distanceTo[sourceNode] = 0;
        prev[sourceNode] = null;

        foreach (Node node in nodes)
        {
            if (node != sourceNode)
            {
                distanceTo[node] = Mathf.Infinity;
                prev[node] = null;
            }

            uncheckedNodes.Add(node);
        }

        while (uncheckedNodes.Count > 0)
        {
            Node node = uncheckedNodes[0];

            foreach (Node possibleNode in uncheckedNodes)
            {
                if (distanceTo[possibleNode] < distanceTo[node])
                {
                    node = possibleNode;
                }
            }

            if (node == targetNode)
            {
                break;
            }

            uncheckedNodes.Remove(node);

            foreach (Node neighbour in node.neighbours)
            {
                float alt = distanceTo[node] + node.DistanceToNeighbour(neighbour);

                if (alt < distanceTo[neighbour])
                {
                    distanceTo[neighbour] = alt;
                    prev[neighbour] = node;
                }
            }
        }

        if (prev[targetNode] == null)
        {
            return;
        }

        List<Node> currentPath = new List<Node>();
        Node currentNode = targetNode;

        while (currentNode != null)
        {
            currentPath.Add(currentNode);
            currentNode = prev[currentNode];
        }

        currentPath.Reverse();

        selectedUnit.currentPath = currentPath;

        //selectedUnit.SetNewPosition(new Vector3(posX, 0, posZ));
    }
}
