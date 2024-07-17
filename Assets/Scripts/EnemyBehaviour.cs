using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState{UNNOTICED,STILL, PATROLLING, CHASINGPLAYER, LOOKINGFORPLAYER, PASIVE};

public class EnemyBehaviour : MonoBehaviour
{
    public EnemyState state;
    public bool aiming;
    public GameObject player;

    private bool awake = false;
    private RagdollController ragdollController;
    private NavMeshAgent agent;

    [Header("Components")]
    [SerializeField] private Transform aimTarget;
    [SerializeField] private Transform head;
   

    [Header("Sight")]
    [SerializeField] private LayerMask sightLayers;
    [SerializeField] private float range = 10f;
    private Vector3 lastSeenPosition;


    [Header("Shooting")]
    public float timeToShoot = 2;
    public Transform bulletStartPoint;
    private GameObject enemyBullet;
    private bool shooting = false;


    void Start()
    {
        player = GameObject.FindWithTag("Player");
        ragdollController = GetComponent<RagdollController>();
        agent = GetComponent<NavMeshAgent>();
        enemyBullet = Resources.Load("Instanciables/EnemyBullet") as GameObject;
    }

    
    void Update()
    {
        if (state != EnemyState.PASIVE) { 
            if (aiming)
            {
                aimTarget.position = player.transform.position;
                ragdollController.ragdollLeftArm = true;
            }
            else
            {
                ragdollController.ragdollLeftArm = false;
            }

            switch (state) { 
            
                case EnemyState.UNNOTICED:
                    aiming = false;
                    awake = false;
                    break;
                case EnemyState.STILL:
                    agent.isStopped = true;
                    break;
                case EnemyState.PATROLLING:
                    awake = false;
                    aiming = false;
                    break ;
                case EnemyState.CHASINGPLAYER:
                    agent.isStopped = false;
                    agent.SetDestination(player.transform.position);
                    break;
                case EnemyState.LOOKINGFORPLAYER:
                    aimTarget.position = lastSeenPosition;
                    agent.isStopped = false;
                    agent.SetDestination(lastSeenPosition);
                    break;
            }

            //Vision

            RaycastHit2D hit = Physics2D.Raycast(head.transform.position, (player.transform.position - head.transform.position).normalized, range, ~sightLayers);
            //Debug.DrawRay(head.transform.position, (player.transform.position - head.transform.position).normalized * range, Color.red);

            if (hit.collider != null) { 
                if (hit.transform.tag == "Player")
                {
                   lastSeenPosition = hit.transform.position;
                   aiming = true;
                   awake = true;
                   state = EnemyState.STILL;
                    if (!shooting)
                    {
                        StartCoroutine(shootingTMP());
                    }
                }
                else
                {
                    if (awake == true)
                    {
                        state = EnemyState.LOOKINGFORPLAYER;
                    }
                }
            }
        }
    }


    IEnumerator shootingTMP()
    {
        shooting = true;
        yield return new WaitForSeconds(timeToShoot);
        Shoot();
        shooting = false;
    }


    public void Shoot()
    {
        GameObject bullet = Instantiate(enemyBullet);
        bullet.transform.position = bulletStartPoint.position;
        bullet.GetComponent<EnemyBullet>().direction = (player.transform.position -  bulletStartPoint.position).normalized;
    }


}
