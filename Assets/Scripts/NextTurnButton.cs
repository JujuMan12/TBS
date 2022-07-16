using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextTurnButton : MonoBehaviour
{
    [HideInInspector] private string defaultLabel;

    [Header("Map")]
    [SerializeField] private TileMap tileMap;

    [Header("Button")]
    [SerializeField] private Button button;

    [Header("Text")]
    [SerializeField] private Text label;
    [SerializeField] private string waitLabel = "Please, Wait your Turn...";

    private void Start()
    {
        defaultLabel = label.text;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Skip Turn"))
        {
            SkipTurn();
        }
    }

    private void FixedUpdate()
    {
        if (!button.interactable && tileMap.isPlayerTurn)
        {
            button.interactable = true;
            label.text = defaultLabel;
        }
    }

    public void SkipTurn()
    {
        tileMap.SelectUnit(null);
        tileMap.isPlayerTurn = false;
        button.interactable = false;
        label.text = waitLabel;
    }
}
