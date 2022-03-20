using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextTurnButton : MonoBehaviour
{
    [HideInInspector] private TileMap tileMap;
    [HideInInspector] private string defaultLabel;

    [Header("Button")]
    [SerializeField] private Button button;

    [Header("Text")]
    [SerializeField] private Text label;
    [SerializeField] private string waitLabel = "Please, Wait your Turn...";

    private void Start()
    {
        tileMap = GameObject.FindGameObjectWithTag("TileMap").GetComponent<TileMap>();
        defaultLabel = label.text;
    }

    private void FixedUpdate()
    {
        if (tileMap.isPlayerTurn && !button.interactable)
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
