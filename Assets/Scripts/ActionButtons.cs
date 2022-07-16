using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtons : MonoBehaviour
{
    [Header("Map")]
    [SerializeField] private TileMap tileMap;

    [Header("Controls")]
    [SerializeField] private Button button;
    [SerializeField] private string input;

    [Header("Action")]
    [SerializeField] private TileMap.ActionState action;

    private void Update()
    {
        if (Input.GetButtonDown(input) && button.interactable == true)
        {
            SelectThisAction();
        }
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
