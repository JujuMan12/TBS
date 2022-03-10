using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [HideInInspector] private Vector3 targetWorldPosition;
    [HideInInspector] public int tilePositionX;
    [HideInInspector] public int tilePositionZ;
    [HideInInspector] public List<Node> currentPath;
    [HideInInspector] public enum AnimationState { idle, moving };
    [HideInInspector] private Animator animator;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 10f;

    private void Start()
    {
        targetWorldPosition = transform.position;
        tilePositionX = (int)transform.position.x;
        tilePositionZ = (int)transform.position.z;
        animator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if (Mathf.Abs(transform.position.x - targetWorldPosition.x) > 0.01f && Mathf.Abs(transform.position.z - targetWorldPosition.z) > 0.01f)
        {
            animator.SetInteger("state", (int)AnimationState.moving);
            transform.position = Vector3.Lerp(transform.position, targetWorldPosition, movementSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetInteger("state", (int)AnimationState.idle);
        }
    }

    //public void SetNewPosition(Vector3 newPosition)
    //{
    //    targetWorldPosition = newPosition;
    //}
}
