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
        //print(currentPath);
        if (currentPath != null)
        {
            int currentNodeId = 0;

            while (currentNodeId < currentPath.Count - 1)
            {
                Vector3 startPos = new Vector3(currentPath[currentNodeId].posX, 0f, currentPath[currentNodeId].posZ);
                Vector3 endPos = new Vector3(currentPath[currentNodeId + 1].posX, 0f, currentPath[currentNodeId + 1].posZ);

                Debug.DrawLine(startPos, endPos, Color.red);
                currentNodeId++;
            }
        }

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
