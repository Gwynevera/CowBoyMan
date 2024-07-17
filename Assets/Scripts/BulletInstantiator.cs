using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BulletInstantiator : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float ShooitngCooldown = 0.25f;
    [SerializeField] private PlayerArm Downarm;
    [SerializeField] private PlayerArm UperArm;

    private Volume postproces;
    private Vignette vignette;
    private float vignetteOrignialIntesity;
    private IEnumerator vignetteCorutine;

    private bool cooldownCharging = false;

    public bool canShoot = true;
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


    [Header("Wind Effect")]
    public float windRange = 10;
    public LayerMask windLayers;
    public float windForce = 5;
    public float windTorqueForce = 1.5f;


    [Header("Particle")]
    public ParticleSystem smokePS;

    private void Start()
    {
        bulletPrefab = Resources.Load("Instanciables/Bullet") as GameObject;
        cm = GameObject.Find("Main Camera").GetComponent<CameraMovement>();

        postproces = GameObject.Find("PostProcessing").GetComponent<Volume>();
        postproces.profile.TryGet(out vignette);
        vignetteOrignialIntesity = vignette.intensity.value;


    }
    private void Update()
    {
        if (bulletInstance == null && !canShoot && !cooldownCharging) StartCoroutine(shootingCooldown());


        if (resetingTime)
        {
            Time.timeScale += (1f / ShooitngCooldown) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0 ,1);
            
            if (Time.timeScale == 1) {
                resetingTime = false;
                Time.fixedDeltaTime = normalFixedDeltaTime;
                
                Downarm.active = true;
                UperArm.lockArm = false;
            }

        }

      
        if (Input.GetKey(KeyCode.Mouse0) && canShoot)
        {
            if (!loadingShoot)
            {

               
                cm.constraintsEnabled = false;
                cm.StopAllCoroutines();
                zoomCorutine = cm.CameraZoom(maxChargingShootTime, cm.chargedShootZoomTarget);
                shakeCorutine = cm.CameraProgresiveShake(maxChargingShootTime, 0.05f, 0.001f);
                StartCoroutine(shakeCorutine);
                StartCoroutine(zoomCorutine);
                vignetteCorutine = VignetteEffect(2, 0.4f);
                StartCoroutine(vignetteCorutine);
            }

            loadingShoot = true;
            if (chargedShootTime < maxChargingShootTime) 
             chargedShootTime = chargedShootTime + Time.deltaTime;

        }

        if (Input.GetKeyUp(KeyCode.Mouse0) && loadingShoot)
        {
            

            Downarm.active = false;
            UperArm.lockArm = true;

            smokePS.Play();
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
        Vector3 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - spawnPoint.position).normalized;
        bl.direction = dir.normalized;
        bl.instantiator = this;

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


        //Wind effect
        Collider2D[] rbs = Physics2D.OverlapCircleAll(spawnPoint.position, windRange, windLayers);

        

        for (int i = 0; i < rbs.Length; i++)
        {
            if (rbs[i].GetComponent<Rigidbody2D>())
            {
                Vector3 windDir = rbs[i].transform.position - spawnPoint.position;
                rbs[i].GetComponent<Rigidbody2D>().AddForce(windDir.normalized * windForce, ForceMode2D.Impulse);
                rbs[i].GetComponent<Rigidbody2D>().AddTorque(windTorqueForce, ForceMode2D.Impulse);
            }
        }

    }

    IEnumerator shootingCooldown()
    {
        
        resetingTime = true;
        cooldownCharging = true;
        yield return new WaitForSecondsRealtime(ShooitngCooldown);
        canShoot = true;
        cooldownCharging = false;
        resetingTime = false;
        smokePS.Stop();

    }

    public void resetVignette()
    {
        StopCoroutine(vignetteCorutine);
        vignetteCorutine = VignetteEffect(0.5f, vignetteOrignialIntesity);
        StartCoroutine(vignetteCorutine);
    }

    public IEnumerator VignetteEffect(float duration, float intensity)
    {


        float elapsed = 0.0f;
        float percentage = 0.0f;

        float startIntensity = vignette.intensity.value;

        while (elapsed < duration)
        {
            percentage = elapsed / duration;

            vignette.intensity.value = Mathf.Lerp(startIntensity, intensity, percentage);

            elapsed += Time.deltaTime;

            yield return null;
        }

    }

    private void OnDrawGizmosSelected()
    {
        if (spawnPoint == null)
            return;
        

        Gizmos.DrawWireSphere(spawnPoint.position, windRange);
    }

}
