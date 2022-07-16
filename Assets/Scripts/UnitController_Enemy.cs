using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController_Enemy : UnitController
{
    override public void ApplyAction()
    {
        UnitController selectedUnit = tileMap.selectedUnit;

        if (tileMap.actionState == TileMap.ActionState.attack && selectedUnit.CanAttack(this))
        {
            selectedUnit.AttackTarget(this);
        }
        else
        {
            //TODO: wrong sound
            print("wrong");
        }
    }
}
