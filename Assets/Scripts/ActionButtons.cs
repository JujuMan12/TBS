using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtons : MonoBehaviour
{
    [HideInInspector] private TileMap tileMap;

    [Header("Button")]
    [SerializeField] private Button button;

    [Header("Action")]
    [SerializeField] private TileMap.ActionStates action;

    private void Start()
    {
        tileMap = GameObject.FindGameObjectWithTag("TileMap").GetComponent<TileMap>();
    }

    private void FixedUpdate()
    {
        if (tileMap.actionState != action)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }

    public void SelectThisAction()
    {
        tileMap.SetActionState(action);
        button.interactable = false;
    }
}
