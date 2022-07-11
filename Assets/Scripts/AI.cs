using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    [HideInInspector] private TileMap tileMap;
    [HideInInspector] private List<Unit> playerUnits;
    [HideInInspector] private List<Unit> enemyUnits;
    [HideInInspector] private bool isActivated;
    [HideInInspector] private float cooldownTime;
    [HideInInspector] private float timeoutTime;

    [Header("AI Turn")]
    [SerializeField] private float turnCooldown = 1f;
    [SerializeField] private float tempTimeout = 3f; //TODO: fix bug

    private void Start()
    {
        tileMap = GameObject.FindGameObjectWithTag("TileMap").GetComponent<TileMap>();
    }

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
                if (!enemyUnits.Exists(unit => unit.unitController.actionPoints > 0))
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
        enemyUnits = new List<Unit>();
        isActivated = true;

        foreach (Unit unit in tileMap.units)
        {
            if (unit.isPlayerOwned)
            {
                playerUnits.Add(unit);
            }
            else
            {
                enemyUnits.Add(unit);
            }
        }

        timeoutTime = tempTimeout;
    }

    private void MakeTurn()
    {
        foreach (Unit unit in enemyUnits)
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
        foreach (Tile neighbour in unit.tile.neighbours)
        {
            if (neighbour.unit != null && neighbour.unit.isPlayerOwned)
            {
                return true;
            }
        }

        return false;
    }

    private void AttackWeakest(Unit unit)
    {
        List<Unit> targets = new List<Unit>();

        foreach (Tile neighbour in unit.tile.neighbours)
        {
            if (neighbour.unit != null && neighbour.unit.isPlayerOwned)
            {
                targets.Add(neighbour.unit);
            }
        }

        Unit weakestTarget = targets[0];

        foreach (Unit target in targets)
        {
            if (target.unitController.healthPoints < weakestTarget.unitController.healthPoints)
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
