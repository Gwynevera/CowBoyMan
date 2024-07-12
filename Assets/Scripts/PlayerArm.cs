using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArm : MonoBehaviour
{
    public bool active = false;
    public int speed = 300;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 diff = target - transform.position;
        float rotationZ = Mathf.Atan2(diff.x, -diff.y) * Mathf.Rad2Deg;
        
        if (active)
        {
            rb.MoveRotation(Mathf.LerpAngle(rb.rotation, rotationZ, speed * Time.fixedDeltaTime));
        }
        

    }
}
