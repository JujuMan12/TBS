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
    [HideInInspector] public enum AnimationState { idle, moving };
    [HideInInspector] private Animator animator;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float tileStopSpeed = 0.05f;

    private void Start()
    {
        currentTile = unitData.tile;
        targetPosition = transform.position;
        animator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if (currentPath != null)
        {
            HandleMovement();
            currentPath[0].tileComponent.ResetColor(); //TODO
        }
    }

    private void HandleMovement()
    {
        if (Mathf.Abs(transform.position.x - targetPosition.x) < tileStopSpeed && Mathf.Abs(transform.position.z - targetPosition.z) < tileStopSpeed)
        {
            if (currentPathId != currentPath.Count - 1)
            {
                currentTile.tileComponent.ResetColor();
                currentPathId++;
                currentTile = currentPath[currentPathId];
                targetPosition = new Vector3(currentTile.posX, currentTile.height, currentTile.posZ);
            }
            else
            {
                currentPath = null;
                currentPathId = 0;
                animator.SetInteger("state", (int)AnimationState.idle);
            }
        }
        else
        {
            animator.SetInteger("state", (int)AnimationState.moving);
            transform.position = Vector3.Lerp(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        }
    }
}
