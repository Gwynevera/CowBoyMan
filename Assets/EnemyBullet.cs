using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public Vector2 direction;
    public float speed;

   
    private void Start()
    {
        float rotationZ = Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ - 180);
    }

    void FixedUpdate()
    {
        direction.Normalize();
        transform.position += new Vector3(direction.x * speed * Time.fixedDeltaTime, direction.y * speed * Time.fixedDeltaTime, 0);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        destroyBullet();

    }


    public void destroyBullet()
    {
        Destroy(this.gameObject);
    }


}
