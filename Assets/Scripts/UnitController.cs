using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [HideInInspector] public Unit unitData;
    [HideInInspector] public List<Tile> currentPath;
    [HideInInspector] public int currentPathId;
    [HideInInspector] public Tile currentTile;
    [HideInInspector] private Vector3 targetPosition;
    [HideInInspector] private Quaternion targetRotation;
    [HideInInspector] public int remainingActionPoints;
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
        currentTile = unitData.tile;
        targetPosition = transform.position;
        targetRotation = transform.rotation;
        remainingActionPoints = maxActionPoints;
        animator = gameObject.GetComponent<Animator>();
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
            currentTile.tileComponent.colorState = TileComponent.ColorState.none;

            if (currentPathId != currentPath.Count - 1)
            {
                currentPathId++;
                currentTile = currentPath[currentPathId];
                targetPosition = new Vector3(currentTile.posX, currentTile.height, currentTile.posZ);
                SetNewRotation();
            }
            else
            {
                currentPath = null;
                currentPathId = 0;
                animator.SetInteger("state", (int)AnimationState.idle);
                currentTile.tileMap.HighlightAvailableTiles(currentTile, remainingActionPoints);
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
}
