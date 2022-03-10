using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileMap : MonoBehaviour
{
    [HideInInspector] private UnitController selectedUnit;
    [HideInInspector] private Tile[,] tiles;
    [HideInInspector] private Node[,] graph;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject nullTilePrefab;
    [SerializeField] private int mapSizeX = 10;
    [SerializeField] private int mapSizeZ = 10;

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
                tiles[x, z] = new Tile(this);
            }
        }

        tiles[1, 2].passable = false;
    }

    private void GeneratePathfindingGraph()
    {
        graph = new Node[mapSizeX, mapSizeZ];

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int z = 0; z < mapSizeZ; z++)
            {
                graph[x, z] = new Node();
                graph[x, z].posX = x;
                graph[x, z].posZ = z;
            }
        }

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int z = 0; z < mapSizeZ; z++)
            {
                if (x > 0)
                {
                    graph[x, z].neighbours.Add(graph[x - 1, z]);
                }
                if (x < mapSizeX - 1)
                {
                    graph[x, z].neighbours.Add(graph[x + 1, z]);
                }
                if (z > 0)
                {
                    graph[x, z].neighbours.Add(graph[x, z - 1]);
                }
                if (z < mapSizeZ - 1)
                {
                    graph[x, z].neighbours.Add(graph[x, z + 1]);
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
                GameObject tile;

                if (tiles[x, z].passable)
                {
                    tile = tilePrefab;
                }
                else
                {
                    tile = nullTilePrefab;
                }

                GameObject tileGO = Instantiate(tile, new Vector3(x, tiles[x, z].height, z), Quaternion.Euler(90f, 0f, 0f), gameObject.transform);
                tileGO.GetComponent<TileComponent>().posX = x;
                tileGO.GetComponent<TileComponent>().posZ = z;
            }
        }
    }

    public void GeneratePathTo(int posX, int posZ)
    {
        selectedUnit.currentPath = null;

        Dictionary<Node, float> distance = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

        List<Node> uncheckedNodes = new List<Node>();

        Node source = graph[selectedUnit.tilePositionX, selectedUnit.tilePositionZ];
        Node target = graph[posX, posZ];

        distance[source] = 0;
        prev[source] = null;

        foreach (Node node in graph)
        {
            if (node != source)
            {
                distance[source] = Mathf.Infinity;
                prev[source] = null;
            }

            uncheckedNodes.Add(node);
        }

        while (uncheckedNodes.Count > 0)
        {
            Node node = null;

            foreach (Node possibleNode in uncheckedNodes)
            {
                if (node == null || distance[possibleNode] < distance[node])
                {
                    node = possibleNode;
                }
            }

            if (node == target)
            {
                break;
            }

            uncheckedNodes.Remove(node);

            foreach (Node neighbour in node.neighbours)
            {
                float alt = distance[node] + node.DistanceToNeighbour(neighbour);

                if (alt < distance[node])
                {
                    distance[node] = alt;
                    prev[node] = node;
                }
            }
        }

        if (prev[target] == null)
        {
            return;
        }

        List<Node> currentPath = new List<Node>();
        Node currentNode = target;

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
