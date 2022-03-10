using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileComponent : MonoBehaviour
{
    [HideInInspector] private TileMap tileMap;
    [HideInInspector] public int posX;
    [HideInInspector] public int posZ;

    private void Start()
    {
        tileMap = GameObject.FindGameObjectWithTag("Map").GetComponent<TileMap>();
    }

    public void OnMouseUp()
    {
        tileMap.GeneratePathTo(posX, posZ);
    }
}
