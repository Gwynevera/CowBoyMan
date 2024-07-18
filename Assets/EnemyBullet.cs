using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public Vector2 direction;
    public float speed;
    private GameObject sparkPrefab;  
    private GameObject sphereSparkPrefab;  

   
    private void Start()
    {
        float rotationZ = Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ - 180);
        sparkPrefab = Resources.Load("Instanciables/Spark") as GameObject;
        sphereSparkPrefab = Resources.Load("Instanciables/SphereSpark") as GameObject;
    }

    void FixedUpdate()
    {
        direction.Normalize();
        transform.position += new Vector3(direction.x * speed * Time.fixedDeltaTime, direction.y * speed * Time.fixedDeltaTime, 0);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        destroyBullet(false);

    }


    public void destroyBullet(bool destroyedByBullet)
    {
        if (destroyedByBullet)
        {
            GameObject tmp = Instantiate(sphereSparkPrefab);
            tmp.transform.position = transform.position;
            tmp.transform.SetParent(null);
        }
        else
        {
            GameObject tmp = Instantiate(sparkPrefab);

            float rotationZ = Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg;
            tmp.transform.rotation = Quaternion.Euler(0f, 0f, rotationZ - 180);

            tmp.transform.position = transform.position;
            tmp.transform.SetParent(null);
        }
        Destroy(this.gameObject);
    }


}
