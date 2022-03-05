using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [HideInInspector] private float targetAngleY;

    [Header("Movement and Rotation")]
    [SerializeField] private float movementSpeed = 20f;
    [SerializeField] private float rotationSpeed = 10f;

    private void Start()
    {
        targetAngleY = transform.localRotation.eulerAngles.y;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        Vector3 translate = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.Translate(translate * movementSpeed * Time.deltaTime, Space.Self);
    }

    private void HandleRotation()
    {
        if (Input.GetButtonDown("Rotate Left"))
        {
            targetAngleY += 90f;
        }
        else if (Input.GetButtonDown("Rotate Right"))
        {
            targetAngleY -= 90f;
        }

        float currentAngleY = transform.localRotation.eulerAngles.y;

        if (Mathf.Abs(currentAngleY - targetAngleY) > 0.01f)
        {
            if (targetAngleY < 0f && currentAngleY > 180f)
            {
                targetAngleY += 360f;
            }
            else if (targetAngleY >= 360f && currentAngleY < 180f)
            {
                targetAngleY -= 360f;
            }

            float newAngle = Mathf.Lerp(currentAngleY, targetAngleY, rotationSpeed * Time.deltaTime);
            newAngle -= currentAngleY;
            transform.Rotate(Vector3.up, newAngle, Space.Self);
        }

        //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, targetAngleY, 0f), rotationSpeed * Time.deltaTime);
    }
}
