using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [HideInInspector] public Unit unitData;
    [HideInInspector] public List<Tile> currentPath;
    [HideInInspector] public int currentPathId;
    [HideInInspector] private TileMap tileMap;
    [HideInInspector] private Vector3 targetPosition;
    [HideInInspector] private Quaternion targetRotation;
    [HideInInspector] public int actionPoints;
    [HideInInspector] public enum AnimationState { idle, moving };
    [HideInInspector] private Animator animator;

    [Header("Characteristics")]
    [SerializeField] public int maxActionPoints = 4;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float tileStopSpeed = 0.05f;
    [SerializeField] private float rotationSpeed = 50f;

    private void Start()
    {
        InitVars();
    }

    private void InitVars()
    {
        tileMap = GameObject.FindGameObjectWithTag("TileMap").GetComponent<TileMap>();
        targetPosition = transform.position;
        targetRotation = transform.rotation;
        actionPoints = maxActionPoints;
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

    private void HandleMovement()
    {
        if (Mathf.Abs(transform.position.x - targetPosition.x) < tileStopSpeed && Mathf.Abs(transform.position.z - targetPosition.z) < tileStopSpeed)
        {
            unitData.tile.tileComponent.colorState = TileComponent.ColorState.none;

            if (currentPathId != currentPath.Count - 1)
            {
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
                tileMap.HighlightSelectedUnit();
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

    private void OnMouseUp()
    {
        if (tileMap.selectedUnit != this)
        {
            tileMap.SelectUnit(this);
        }
    }
}
