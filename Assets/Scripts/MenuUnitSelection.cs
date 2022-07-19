using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUnitSelection : MonoBehaviour
{
    [Header("Unit")]
    [SerializeField] private TileMap.UnitType unitType;

    private void Start()
    {
        TileMap.unitCount = new int[4];
    }

    public void SetUnitCount(string count)
    {
        TileMap.unitCount[(int)unitType] = int.Parse(count);
    }
}
