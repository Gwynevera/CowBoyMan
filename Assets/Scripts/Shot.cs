using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
    public int dir;
    public float vel;

    public int damage = 1;

    GameObject cam;
    Rigidbody2D rb;

    public GameObject shot_hit;

    int zRot = 45;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Camera");
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = new Vector2(dir * vel, 0);

        if (dir == 1 && transform.position.x > cam.transform.position.x + cam.GetComponent<Camera>().orthographicSize ||
            dir == -1 && transform.position.x < cam.transform.position.x - cam.GetComponent<Camera>().orthographicSize)
        {
            UpdatePlayerShots();
        }
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        CheckCollision(c);
    }
    void OnCollisionStay2D(Collision2D c)
    {
        CheckCollision(c);
    }

    void CheckCollision(Collision2D c)
    {
        bool enemyHit = false;
        if (c.gameObject.tag == "Enemy")
        {
            c.gameObject.GetComponent<Enemy>().Damage(damage);
            enemyHit = true;
        }
        UpdatePlayerShots(enemyHit);
    }

    void UpdatePlayerShots(bool enemyHit = false)
    {
        GameObject.Find("Player").GetComponent<Player>().shots--;

        GameObject i = null;
        int r = Random.Range(0, 2);
        if (r==0)
        {
            i = Instantiate(shot_hit, transform.position, transform.rotation, null);
        }
        else
        {
            Vector3 rot = new Vector3(0, 0, zRot);
            i = Instantiate(shot_hit, transform.position, Quaternion.Euler(rot), null);
        }

        if (enemyHit)
        {
            i.GetComponent<AudioSource>().Play();
        }

        Destroy(this.gameObject);
    }
}
