using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Node()
    {
        neighbours = new List<Node>();
    }

    public List<Node> neighbours;
    public int posX;
    public int posZ;

    public float DistanceToNeighbour(Node neighbour)
    {
        return Vector2.Distance(new Vector2(posX, posZ), new Vector2(neighbour.posX, neighbour.posZ));
    }
}
