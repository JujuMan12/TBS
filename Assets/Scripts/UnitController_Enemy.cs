using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController_Enemy : UnitController
{
    override public void OnMouseUp()
    {
        UnitController selectedUnit = tileMap.selectedUnit;

        if (selectedUnit != null && tileMap.actionState == TileMap.ActionStates.attack && selectedUnit.actionPoints >= selectedUnit.attackCost)
        {
            selectedUnit.AttackTarget(this);
        }
    }
}
