using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("Map")]
    [SerializeField] private TileMap tileMap;

    [Header("UI Elements")]
    [SerializeField] private GameObject actionButtons;
    [SerializeField] private GameObject unitInfoWindow;

    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel") && tileMap.selectedUnit == null)
        {
            Application.Quit(); //TODO: make main menu
        }
    }

    private void FixedUpdate()
    {
        bool selectedUnitIsAvailable = tileMap.selectedUnit != null && tileMap.selectedUnit.currentPath == null;

        if (selectedUnitIsAvailable && !actionButtons.activeSelf)
        {
            actionButtons.SetActive(true);
            unitInfoWindow.SetActive(true);
        }
        else if (!selectedUnitIsAvailable && actionButtons.activeSelf)
        {
            actionButtons.SetActive(false);
            unitInfoWindow.SetActive(false);
        }
    }
}
