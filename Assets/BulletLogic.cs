using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLogic : MonoBehaviour
{
    public Vector2 direction;
    public float controlTime;
    public float speed;
    public float rotationSpeed;
    public float bulletForceOnImpact;
    public bool dismemberment = false;

    private bool controlling = true;

    private void Start()
    {
        StartCoroutine(ControllWaiter());
    }

    void FixedUpdate()
    {
        if (controlling) { 
            Vector2 targetDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            targetDirection.Normalize();
            direction.Normalize();

            if (Mathf.Abs(targetDirection.x - direction.x) > 0.2) { 
                if(targetDirection.x > direction.x) direction.x += rotationSpeed;
                if(targetDirection.x < direction.x) direction.x -= rotationSpeed;
            }
            if (Mathf.Abs(targetDirection.y - direction.y) > 0.2)
            {
                if (targetDirection.y > direction.y) direction.y += rotationSpeed;
                if (targetDirection.y < direction.y) direction.y -= rotationSpeed;
            }

        }
        transform.position += new Vector3(direction.x * speed, direction.y * speed, 0);
    }

    IEnumerator ControllWaiter()
    {
        yield return new WaitForSecondsRealtime(controlTime);
        controlling = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Enemy")
        {
            if (collision.transform.GetComponent<Rigidbody2D>())
            {
                if(collision.transform.GetComponent<HingeJoint2D>() && dismemberment) collision.transform.GetComponent<HingeJoint2D>().breakForce = 1;
                
                if (collision.transform.parent.GetComponentInChildren<SpringJoint2D>())
                    Destroy(collision.transform.parent.GetComponentInChildren<SpringJoint2D>());

                Vector2 dir = direction;
                collision.transform.GetComponent<Rigidbody2D>().AddForce(dir * bulletForceOnImpact * collision.transform.GetComponent<Rigidbody2D>().mass, ForceMode2D.Impulse);
                
               
            }
        }
        Destroy(this.gameObject);
    }
   
}
