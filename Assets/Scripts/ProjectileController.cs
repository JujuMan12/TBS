using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [HideInInspector] private GameObject target;

    [Header("Rigid Body")]
    [SerializeField] private Rigidbody rb;

    [Header("Parameters")]
    [SerializeField] private float lifeTime = 10f;
    [SerializeField] private float speed = 500f;

    [Header("Effects")]
    [SerializeField] private GameObject hitEffect;

    private void Start()
    {
        rb.AddForce(gameObject.transform.forward * speed);
        Destroy(gameObject, lifeTime);
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject == target)
        {
            //Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
