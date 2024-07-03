using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletInstantiator : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject bulletPrefab;
    private bool canShoot = true;
    private GameObject bulletInstance;

    private void Start()
    {
        bulletPrefab = Resources.Load("Instanciables/Bullet") as GameObject;
    }
    private void Update()
    {
        if (bulletInstance == null) canShoot = true;

        if (Input.GetKeyDown(KeyCode.Mouse0) && canShoot)
        {
            instantiateBullet();
        }
    }
    private void instantiateBullet()
    {
        canShoot = false;
        bulletInstance = Instantiate(bulletPrefab);
        bulletInstance.transform.position = spawnPoint.position;
        bulletInstance.GetComponent<BulletLogic>().direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - spawnPoint.position).normalized;
    }
}
