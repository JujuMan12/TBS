using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitController : MonoBehaviour
{
    [HideInInspector] public Unit unitData;
    [HideInInspector] public List<Tile> currentPath;
    [HideInInspector] public int currentPathId;
    [HideInInspector] protected TileMap tileMap;
    [HideInInspector] private Vector3 targetPosition;
    [HideInInspector] private Quaternion targetRotation;
    [HideInInspector] public int healthPoints;
    [HideInInspector] public int armorPoints;
    [HideInInspector] public int actionPoints;
    [HideInInspector] public bool inDefenceStance;
    [HideInInspector] public enum AnimationState { idle, moving, death };
    [HideInInspector] private Animator animator;

    [Header("Characteristics")]
    [SerializeField] public int maxHealthPoints = 1;
    [SerializeField] public int maxArmorPoints = 1;
    [SerializeField] public int maxActionPoints = 4;
    [SerializeField] private int minProtection = 0;
    [SerializeField] private int maxProtection = 1;
    [SerializeField] private TextMeshPro healthPointsText;
    [SerializeField] private TextMeshPro armorPointsText;
    [SerializeField] private TextMeshPro actionPointsText;

    [Header("Ability - Attack")]
    [SerializeField] public int attackRange = 1;
    [SerializeField] public int attackCost = 2;
    [SerializeField] private int minDamage = 0;
    [SerializeField] private int maxDamage = 1;

    [Header("Ability - Defence")]
    [SerializeField] private int defenceMinBonus = 0;
    [SerializeField] private int defenceMaxBonus = 1;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float tileMovementSpeed = 0.1f;
    [SerializeField] private float rotationSpeed = 500f;

    private void Start()
    {
        tileMap = GameObject.FindGameObjectWithTag("TileMap").GetComponent<TileMap>();

        targetPosition = transform.position;
        targetRotation = transform.rotation;

        healthPoints = maxHealthPoints;
        armorPoints = maxArmorPoints;
        actionPoints = maxActionPoints;
        UpdateText();

        animator = gameObject.GetComponent<Animator>();
    }

    public void SetUnitData(Unit unit)
    {
        unitData = unit;
        unit.unitController = this;
    }

    private void Update()
    {
        if (currentPath != null)
        {
            HandleMovement();
        }

        if (transform.rotation != targetRotation)
        {
            UpdateRotation();
        }
    }

    private void UpdateText()
    {
        healthPointsText.text = healthPoints.ToString();
        actionPointsText.text = actionPoints.ToString();

        if (armorPoints > 0)
        {
            armorPointsText.enabled = true;
            armorPointsText.text = "+" + armorPoints.ToString();
        }
        else
        {
            armorPointsText.enabled = false;
        }
    }

    private void HandleMovement()
    {
        if (Mathf.Abs(transform.position.x - targetPosition.x) < tileMovementSpeed && Mathf.Abs(transform.position.z - targetPosition.z) < tileMovementSpeed)
        {
            transform.position = targetPosition;
            Tile unitTile = unitData.tile;

            if (unitTile.tileComponent.colorState == TileComponent.ColorState.path)
            {
                unitTile.tileComponent.colorState = TileComponent.ColorState.none;
            }

            if (currentPathId != currentPath.Count - 1 && currentPath[currentPathId + 1].unit == null && actionPoints > 0)
            {
                actionPoints--;
                UpdateText();
                currentPathId++;
                unitData.SetTile(currentPath[currentPathId]);
                targetPosition = new Vector3(unitData.tile.posX, unitData.tile.height, unitData.tile.posZ);
                SetNewRotation();
            }
            else
            {
                currentPath = null;
                currentPathId = 0;
                animator.SetInteger("state", (int)AnimationState.idle);

                if (tileMap.selectedUnit == this)
                {
                    tileMap.HighlightSelectedUnit();
                }
                else if (unitData.isPlayerOwned)
                {
                    unitTile.tileComponent.colorState = TileComponent.ColorState.ally;
                }
                else
                {
                    unitTile.tileComponent.colorState = TileComponent.ColorState.enemy;
                }
            }
        }
        else
        {
            animator.SetInteger("state", (int)AnimationState.moving);
            transform.position = Vector3.Lerp(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        }
    }

    private void UpdateRotation()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void SetNewRotation()
    {
        float angleY = 0f;

        if (targetPosition.x - transform.position.x > 0.1f)
        {
            angleY = 90f;
        }
        else if (targetPosition.x - transform.position.x < -0.1f)
        {
            angleY = -90f;
        }
        else if (targetPosition.z - transform.position.z < -0.1f)
        {
            angleY = 180f;
        }

        targetRotation = Quaternion.Euler(0f, angleY, 0f);
    }

    public void HighlightCurrentPath()
    {
        for (int i = 0; i < Mathf.Min(currentPath.Count, actionPoints + 1); i++)
        {
            if (currentPath[i].tileComponent.colorState == TileComponent.ColorState.none)
            {
                currentPath[i].tileComponent.colorState = TileComponent.ColorState.path;
            }
        }
    }

    virtual public void OnMouseUp()
    {
        if (tileMap.isPlayerTurn)
        {
            if (tileMap.selectedUnit != this)
            {
                tileMap.SelectUnit(this);
            }
            else if (tileMap.actionState == TileMap.ActionStates.defence)
            {
                SetDefenceStance();
            }
        }
    }

    public void AttackTarget(UnitController target)
    {
        if (!unitData.tile.neighbours.Contains(target.unitData.tile)) //TODO: range
        {
            return;
        }

        Vector3 targetPosition = target.transform.position;
        float angle;
        if (targetPosition.x > transform.position.x) //TODO: rework
        {
            angle = 90f;
        }
        else if (targetPosition.x < transform.position.x)
        {
            angle = -90f;
        }
        else if (targetPosition.z > transform.position.z)
        {
            angle = 0f;
        }
        else
        {
            angle = 180f;
        }

        targetRotation = Quaternion.Euler(new Vector3(0f, angle, 0f));
        target.targetRotation = Quaternion.Euler(new Vector3(0f, angle - 180f, 0f));

        animator.SetTrigger("attack");

        actionPoints -= attackCost;
        UpdateText();
        int damage = Random.Range(minDamage, maxDamage);
        int protection = Random.Range(target.minProtection, target.maxProtection);

        if (target.inDefenceStance)
        {
            protection += Random.Range(target.defenceMinBonus, target.defenceMaxBonus);
        }

        damage -= protection;

        if (damage > 0)
        {
            target.TakeDamage(damage);
        }
    }

    virtual public void TakeDamage(int damage)
    {
        healthPoints -= damage - armorPoints;
        UpdateText();

        if (healthPoints <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("damage");
        }
    }

    public void SetDefenceStance()
    {
        print("defence");
    }

    private void Die()
    {
        UpdateText();
        animator.SetInteger("state", (int)AnimationState.death);
        unitData.tile.tileComponent.colorState = TileComponent.ColorState.none;

        Destroy(gameObject, 2f);
        unitData.tile.unit = null;
        tileMap.units.Remove(unitData);
        this.enabled = false;
    }
}
