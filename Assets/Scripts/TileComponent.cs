using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileComponent : MonoBehaviour
{
    [HideInInspector] public Tile tileData;

    public void OnMouseUp()
    {
        tileData.tileMap.GeneratePathTo(tileData.posX, tileData.posZ);
    }
}
