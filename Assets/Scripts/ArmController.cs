using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ArmController : MonoBehaviour
{
    public bool active = false;
    public int speed = 300;
    private Rigidbody2D rb;
    public Transform target;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (target != null) { 
            Vector3 diff =  target.position - transform.position;
            float rotationZ = Mathf.Atan2(diff.x, -diff.y) * Mathf.Rad2Deg;

            if (active)
            {
                rb.MoveRotation(Mathf.LerpAngle(rb.rotation, rotationZ, speed * Time.fixedDeltaTime));
            }
        }

    }
}
