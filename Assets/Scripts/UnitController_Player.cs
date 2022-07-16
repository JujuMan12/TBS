using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController_Player : UnitController
{
    override public void ApplyAction()
    {
        if (tileMap.actionState == TileMap.ActionState.defence && tileMap.selectedUnit == this && actionPoints >= defenceCost)
        {
            SetDefenceStance();
        }
        else
        {
            //TODO: wrong sound
            print("wrong");
        }
    }
}
