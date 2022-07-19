using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    [HideInInspector] private List<Unit> playerUnits;
    [HideInInspector] private List<Unit> aiUnits;
    [HideInInspector] private bool isActivated;
    [HideInInspector] private float cooldownTime;
    [HideInInspector] private float timeoutTime;

    [Header("Map")]
    [SerializeField] private TileMap tileMap;

    [Header("AI Turn")]
    [SerializeField] private float turnCooldown = 1f;
    [SerializeField] private float tempTimeout = 3f; //TODO: fix bug

    private void FixedUpdate()
    {
        if (!tileMap.isPlayerTurn)
        {
            if (cooldownTime > 0f)
            {
                cooldownTime -= Time.deltaTime;
            }

            if (!isActivated)
            {
                Activate();
            }
            else if (cooldownTime <= 0f)
            {
                if (!aiUnits.Exists(unit => unit.unitController.actionPoints > 0))
                {
                    isActivated = false;
                    tileMap.ResetPlayerTurn();
                }
                else
                {
                    MakeTurn();
                }
            }

            if (timeoutTime > 0f)
            {
                timeoutTime -= Time.deltaTime;
            }
            else
            {
                tileMap.ResetPlayerTurn();
                return;
            }
        }
    }

    private void Activate()
    {
        playerUnits = new List<Unit>();
        aiUnits = new List<Unit>();
        isActivated = true;

        foreach (Unit unit in tileMap.units)
        {
            if (unit.isPlayerOwned)
            {
                playerUnits.Add(unit);
            }
            else
            {
                aiUnits.Add(unit);
            }
        }

        timeoutTime = tempTimeout;
    }

    private void MakeTurn()
    {
        foreach (Unit unit in aiUnits)
        {
            UnitController unitController = unit.unitController;

            if (unitController.actionPoints > 0 && unitController.currentPath == null)
            {
                if (CanAttack(unit) && unitController.actionPoints >= unitController.attackCost)
                {
                    AttackWeakest(unit);
                }
                else
                {
                    MoveTowardsTarget(unit);
                }
            }
        }

        cooldownTime = turnCooldown;
    }

    private bool CanAttack(Unit unit)
    {
        foreach (Unit target in playerUnits)
        {
            if (unit.tile.DistanceTo(target.tile) <= unit.unitController.attackRange)
            {
                return true;
            }
        }

        return false;
    }

    private void AttackWeakest(Unit unit)
    {
        Unit weakestTarget = playerUnits[0];

        foreach (Unit target in playerUnits)
        {
            bool canAttack = unit.tile.DistanceTo(target.tile) <= unit.unitController.attackRange;
            bool isWeaker = target.unitController.healthPoints < weakestTarget.unitController.healthPoints;

            if (canAttack && isWeaker)
            {
                weakestTarget = target;
            }
        }

        unit.unitController.AttackTarget(weakestTarget.unitController);
    }

    private void MoveTowardsTarget(Unit unit)
    {
        Unit target = null;
        float minDistance = Mathf.Infinity;

        foreach (Unit potentialTarget in playerUnits)
        {
            float distance = potentialTarget.tile.DistanceTo(unit.tile);

            if (distance < minDistance)
            {
                target = potentialTarget;
                minDistance = distance;
            }
        }

        if (unit.tile.neighbours.Contains(target.tile))
        {
            unit.unitController.actionPoints = 0;
        }
        else
        {
            tileMap.GeneratePathTo(target.tile.posX, target.tile.posZ, unit.unitController, true);
        }
    }
}
