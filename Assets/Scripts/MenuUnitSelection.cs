using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuUnitSelection : MonoBehaviour
{
    [Header("Unit")]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TileMap.UnitType unitType;
    [SerializeField] private int maxValue = 10;

    private void Start()
    {
        TileMap.CreateUnitCount();
    }

    public void SetUnitCount(string count)
    {
        TileMap.unitCount[(int)unitType] = Mathf.Clamp(int.Parse(count), 0, maxValue);
    }

    public void UpdateUnitCount()
    {
        inputField.text = TileMap.unitCount[(int)unitType].ToString();
    }
}
