using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [HideInInspector] private TileMap tileMap;

    [Header("Elements")]
    //[SerializeField] private GameObject nextTurnButton;
    [SerializeField] private GameObject actionButtons;

    private void Start()
    {
        tileMap = GameObject.FindGameObjectWithTag("TileMap").GetComponent<TileMap>();
    }

    private void FixedUpdate()
    {
        if (tileMap.selectedUnit != null && tileMap.selectedUnit.currentPath == null)
        {
            actionButtons.SetActive(true);
        }
        else
        {
            actionButtons.SetActive(false);
        }
    }
}
