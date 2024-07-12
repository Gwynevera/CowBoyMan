using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletInstantiator : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float ShooitngCooldwon = 0.25f;
    [SerializeField] private PlayerArm[] arms; 

    private bool cooldownCharging = false;

    private bool canShoot = true;
    private bool loadingShoot = false;
    private bool resetingTime = false;
    public float chargedShootTime = 0;
    public float maxChargingShootTime = 0;
    public float minBulletmultiplier = 0.5f; 
    public float maxBulletmultiplier = 2.0f; 
    private GameObject bulletInstance;
    private CameraMovement cm;
    private IEnumerator zoomCorutine;
    private IEnumerator shakeCorutine;
    private float normalFixedDeltaTime;

    private void Start()
    {
        bulletPrefab = Resources.Load("Instanciables/Bullet") as GameObject;
        cm = GameObject.Find("Main Camera").GetComponent<CameraMovement>();
    }
    private void Update()
    {
        if (bulletInstance == null && !canShoot && !cooldownCharging) StartCoroutine(shootingCooldown());


        if (resetingTime)
        {
            Time.timeScale += (1f / ShooitngCooldwon) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0 ,1);


            if (Time.timeScale == 1) {
                resetingTime = false;
                Time.fixedDeltaTime = normalFixedDeltaTime;
                for (int i = 0; i < arms.Length; i++)
                {
                    arms[i].enabled = true;
                }
            }

        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && canShoot)
        {
            cm.constraintsEnabled = false;
            cm.StopAllCoroutines();
            zoomCorutine = cm.CameraZoom(maxChargingShootTime, cm.chargedShootZoomTarget);
            shakeCorutine = cm.CameraProgresiveShake(maxChargingShootTime, 0.05f, 0.001f);
            StartCoroutine(shakeCorutine);
            StartCoroutine(zoomCorutine);
        }
        if (Input.GetKey(KeyCode.Mouse0) && canShoot)
        {

            loadingShoot = true;
            if (chargedShootTime < maxChargingShootTime) 
             chargedShootTime = chargedShootTime + Time.deltaTime;

        }

        if (Input.GetKeyUp(KeyCode.Mouse0) && loadingShoot)
        {

            for (int i = 0; i < arms.Length; i++ )
            {
                arms[i].enabled = false;
            }

            instantiateBullet();
            loadingShoot = false;
            canShoot = false;
            StopCoroutine(zoomCorutine);
            StopCoroutine(shakeCorutine);
            
        }
        
    }
    private void instantiateBullet()
    {

        Time.timeScale = 0.05f;
        normalFixedDeltaTime = Time.fixedDeltaTime;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        bulletInstance = Instantiate(bulletPrefab);
        bulletInstance.transform.position = spawnPoint.position;
        BulletLogic bl = bulletInstance.GetComponent<BulletLogic>();
        bl.direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - spawnPoint.position).normalized;

        if(cm.resetingCameraCorutine != null) {
            StopCoroutine(cm.resetingCameraCorutine);
            cm.resetingCamera = false;
        }

        cm.constraintsEnabled = false;
        cm.Bullet = bulletInstance;
        cm.cameraTarget = CameraTarget.BULLET;

        
        StartCoroutine(cm.CameraBulletZoom(bulletInstance));

        float chargeTimePercentage = chargedShootTime / maxChargingShootTime;

        float multiplier = ((maxBulletmultiplier - minBulletmultiplier) * chargedShootTime) + minBulletmultiplier;



        bl.speed *= multiplier;
        bl.bulletForceOnImpact *= multiplier;

        

        chargedShootTime = 0;
    }

    IEnumerator shootingCooldown()
    {
        resetingTime = true;
        cooldownCharging = true;
        yield return new WaitForSecondsRealtime(ShooitngCooldwon);
        canShoot = true;
        cooldownCharging = false;
        resetingTime = true;

        
    }
}
