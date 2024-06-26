using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rb;
    Collider2D c;
    SpriteRenderer s;
    GameObject p;

    public int health;
    public int damage = 1;
    int dir = 1;
    bool canDir = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        c = GetComponent<Collider2D>();
        s = GetComponent<SpriteRenderer>();

        p = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (canDir)
        {
            if (p.transform.position.x < transform.position.x)
            {
                dir = -1;
            }
            else if (p.transform.position.x > transform.position.x)
            {
                dir = 1;
            }
        }
        s.flipX = dir == 1;
    }

    private void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.tag == "Player")
        {
            Player p = c.gameObject.GetComponent<Player>();
            if (!p.damage)
            {
                p.Damage(damage, dir);
            }
        }
    }

    public void Damage(int d)
    {
        health -= d;

        if (health <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        Destroy(this.gameObject);
    }
}
