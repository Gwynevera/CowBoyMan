using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum BulletSkill {NONE ,PIERCING, SUBBULLETS}


public class BulletLogic : MonoBehaviour
{
    public Vector2 direction;
    public float controlTime;
    public float speed;
    public float rotationSpeed;
    public float bulletForceOnImpact;
    public bool dismemberment = false;

    public BulletInstantiator instantiator;

    private bool controlling = true;

    private Rigidbody2D rb;

    private GameObject sparkPrefab;
    private GameObject bloodPrefab;
    private GameObject sphereSparkPrefab;
    private GameObject bulletNoControllPrefab;

    private GameObject previousEnemy;

    public int maxPiercing = 2;

    [Header("Wind Effect")]
    public float windRange = 10;
    public LayerMask windLayers;
    public float windForce = 10;
    public float windTorqueForce = 1.5f;


    [Header("Bullet Destroyer")]
    public float bulletDestroyerRange = 5;
    public LayerMask bulletDestroyerLayers;

    [Header("Bullet Skill")]
    public BulletSkill bulletSkill = BulletSkill.NONE;
    private bool skillUsed = false;
    [Header("Bullet Skill - Piercing")]
    private int enemyPiercing = 0;
    public int newMaxPiercing = 5;
    public float speedMult = 1.5f;


    private void Start()
    {

        sparkPrefab = Resources.Load("Instanciables/Spark") as GameObject;
        bloodPrefab = Resources.Load("Instanciables/Blood") as GameObject;
        sphereSparkPrefab = Resources.Load("Instanciables/SphereSpark") as GameObject;
        bulletNoControllPrefab = Resources.Load("Instanciables/BulletNoControll") as GameObject;


        StartCoroutine(ControllWaiter());
        rb = GetComponent<Rigidbody2D>();
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



            float rotationZ = Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg;


            transform.rotation = Quaternion.Euler(0f,0f, rotationZ - 180);

        }

        transform.position += new Vector3(direction.x * speed * Time.fixedDeltaTime, direction.y * speed * Time.fixedDeltaTime, 0);


        if (Input.GetKey(KeyCode.Mouse0) && controlling && !skillUsed)
        {
            skillUsed = true;
            switch (bulletSkill)
            {
                case BulletSkill.NONE:
                    break;
                case BulletSkill.PIERCING:
                    speed *= speedMult;
                    maxPiercing = newMaxPiercing;

                    GameObject ps = Instantiate(sphereSparkPrefab);
                    ps.transform.position = transform.position;

                    Destroy(ps, ps.GetComponent<ParticleSystem>().main.duration);


                    break;
                case BulletSkill.SUBBULLETS:

                    destroyBullet(false);

                    GameObject tmp = Instantiate(bulletNoControllPrefab);
                    tmp.transform.position = transform.position;
                    tmp.GetComponent<Bullet>().direction = transform.right;

                    tmp = Instantiate(bulletNoControllPrefab);
                    tmp.transform.position = transform.position;
                    tmp.GetComponent<Bullet>().direction = -transform.right;



                    break;
            }


        }


        //Wind effect
        Collider2D[] rbs = Physics2D.OverlapCircleAll(transform.position, windRange, windLayers);

        for (int i = 0; i < rbs.Length; i++)
        {
            if (rbs[i].GetComponent<Rigidbody2D>())
            {

                if (rbs[i].transform.tag == "Hat")
                {
                    if(rbs[i].transform.parent.transform.parent.GetComponent<RagdollController>()) rbs[i].transform.parent.transform.parent.GetComponent<RagdollController>().dropHat = true;
                    rbs[i].transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                }

                if (rbs[i].transform.tag == "Physics")
                {
                    rbs[i].transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                }

                
                Vector3 windDir = direction;
                rbs[i].GetComponent<Rigidbody2D>().AddForce(windDir.normalized * windForce, ForceMode2D.Impulse);
                rbs[i].GetComponent<Rigidbody2D>().AddTorque(-windTorqueForce, ForceMode2D.Impulse);

            }
        }

        //Bullet Destroyer

        Collider2D[] bullets = Physics2D.OverlapCircleAll(transform.position, bulletDestroyerRange, bulletDestroyerLayers);

        for (int i = 0; i < bullets.Length; i++)
        {
            if (bullets[i].GetComponent<Rigidbody2D>())
            {
                if (bullets[i].transform.tag == "EnemyBullet")
                {
                    bullets[i].transform.GetComponent<EnemyBullet>().destroyBullet(true);
                }
            } 
        }
    }

    IEnumerator ControllWaiter()
    {
        yield return new WaitForSecondsRealtime(controlTime);
        controlling = false;
        StartCoroutine(DestroyTimer());
    }
    IEnumerator DestroyTimer()
    {
        yield return new WaitForSecondsRealtime(5);
        destroyBullet(false);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Enemy")
        {
            if(previousEnemy == null || previousEnemy.transform.parent != collision.gameObject.transform.parent) { 
                previousEnemy = collision.gameObject;
                enemyPiercing++;
            }

            if (enemyPiercing >= maxPiercing)
            {
                destroyBullet(true);
            }

            if (collision.transform.GetComponent<Rigidbody2D>())
            {
                if(collision.transform.GetComponent<HingeJoint2D>() && dismemberment) collision.transform.GetComponent<HingeJoint2D>().breakForce = 1;
                
                if (collision.transform.parent.transform.parent.GetComponent<RagdollController>())
                    collision.transform.parent.transform.parent.GetComponent<RagdollController>().Die();

                Vector2 dir = direction;
                collision.transform.GetComponent<Rigidbody2D>().AddForce(dir * bulletForceOnImpact * collision.transform.GetComponent<Rigidbody2D>().mass, ForceMode2D.Impulse);


                GameObject ps = Instantiate(bloodPrefab);
                

                float rotationZ = Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg;
                ps.transform.rotation = Quaternion.Euler(0f, 0f, rotationZ - 180);

                ps.transform.parent = collision.transform;
                ps.transform.localPosition = Vector3.zero;
                ps.transform.localScale = Vector3.one;

                Destroy(ps, ps.GetComponent<ParticleSystem>().main.duration);

                if (!controlling)
                {
                    destroyBullet(true);
                }
                
               
                
               
            }

        }

        if(collision.transform.tag == "Level") destroyBullet(false);

        if(collision.transform.tag == "Interactable")
        {
            collision.transform.GetComponent<Interactable>().activate();
            destroyBullet(false);
        }

    }


    IEnumerator ResetTimer()
    {
        yield return new WaitForSecondsRealtime(3);

        CameraMovement cm = GameObject.Find("Main Camera").GetComponent<CameraMovement>();

        cm.cameraTarget = CameraTarget.PLAYER1;
        instantiator.resetingTime = true;
        cm.constraintsEnabled = true;
        instantiator.resetVignette();
        Destroy(this.gameObject);

    }


    public void destroyBullet(bool blood)
    {
        controlling = false;
        speed = 0;
        Destroy(this.GetComponent<SpriteRenderer>());

        if (!blood)
        {

            GameObject ps = Instantiate(sparkPrefab);
            ps.transform.position = transform.position;

            float rotationZ = Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg;
            ps.transform.rotation = Quaternion.Euler(0f, 0f, rotationZ - 180);

            Destroy(ps, ps.GetComponent<ParticleSystem>().main.duration);
        }

        if (GetComponentInChildren<ParticleSystem>())
        {
            GameObject tmp = GetComponentInChildren<ParticleSystem>().gameObject;
            tmp.transform.SetParent(null);
            Destroy(tmp, 1);
        }
       

        instantiator.StopZoom();
        GameObject.Find("Main Camera").GetComponent<CameraMovement>().resetZoom(2);
        StartCoroutine(ResetTimer());
        
    }




   
}
