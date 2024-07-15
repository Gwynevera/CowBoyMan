using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Loading;
using UnityEngine;
using UnityEngine.UI;

public enum CameraTarget {PLAYER1, BULLET}


public class CameraMovement : MonoBehaviour
{
    [Header("Camera Target")]
    public CameraTarget cameraTarget = CameraTarget.PLAYER1;

    [Header("Misc Variables")]
    public GameObject Player;
    public GameObject Bullet;
    private float originalZ;

    [Header("Traveling")]
    [SerializeField] private float initialZoom = 10.0f;
    [SerializeField] private float timeTraveling = 2.0f;
    private float deltaTime = 0;


    [Header("Camera Follow Constrains")]
    public bool constraintsEnabled = true;
    [SerializeField] float minY = 0;
    [SerializeField] float yOffset = 0;

    
   

    [Header("Bullet Charge")]
    public float chargedShootZoomTarget = 2;

    [Header("Bullet")]
    [SerializeField] float zoomSpeed = 10;
    [SerializeField] float timeToMaxZoom = 3;
    [SerializeField] float targetZoom = 3;


    private Camera mainCam;
    private Transform cameraHolder;

    private Vector3 CameraTargetPos = new Vector3();
    private Vector3 posFinal = new Vector3();
    [NonSerialized]public bool resetingCamera = false;
    [NonSerialized]public IEnumerator resetingCameraCorutine;

    private void Start()
    {

        mainCam = this.GetComponent<Camera>();
        cameraHolder = this.transform.parent.transform;
        originalZ = cameraHolder.transform.position.z;
        

        StartCoroutine(CameraZoom(2, initialZoom));
      
    }

    private void Update()
    {
        Vector3 posOriginal = new Vector3();

        //Camera Target
        if (cameraTarget == CameraTarget.PLAYER1) {
   
            CameraTargetPos.x = Player.transform.position.x + (Player.transform.position.x - Player.transform.position.x) / 2;
            CameraTargetPos.y = Player.transform.position.y + (Player.transform.position.y - Player.transform.position.y) / 2;
            if(constraintsEnabled) posFinal = new Vector3(CameraTargetPos.x, CameraTargetPos.y + yOffset, originalZ);
            else posFinal = new Vector3(CameraTargetPos.x, CameraTargetPos.y , originalZ);

            if (posFinal.y < minY && constraintsEnabled)
            {
                posFinal.y = minY;
            }

        }
        else if (cameraTarget == CameraTarget.BULLET)
        {

            CameraTargetPos.x = Bullet.transform.position.x + (Bullet.transform.position.x - Bullet.transform.position.x) / 2;
            CameraTargetPos.y = Bullet.transform.position.y + (Bullet.transform.position.y - Bullet.transform.position.y) / 2;
            if (constraintsEnabled)  posFinal = new Vector3(CameraTargetPos.x, CameraTargetPos.y + yOffset, originalZ);
            else posFinal = new Vector3(CameraTargetPos.x, CameraTargetPos.y , originalZ);

            if (posFinal.y < minY && constraintsEnabled)
            {
                posFinal.y = minY;
            }

        }

        
        posOriginal = cameraHolder.transform.position;
        posOriginal.z = originalZ;

        if (posFinal != posOriginal && !resetingCamera)
        {
            deltaTime += Time.deltaTime;
            float percentage = deltaTime / timeTraveling;

          

            cameraHolder.transform.position = Vector3.Lerp(posOriginal, posFinal, percentage);

            if (deltaTime >= 1 || posOriginal == posFinal)
            {
                deltaTime = 0;
            }
        }
        
       
       
    }


    public IEnumerator CameraBulletZoom(GameObject bullet)
    {
        
        float originalZoom = mainCam.orthographicSize;
        
        float elapsed = 0.0f;
        float percentage = 0.0f;

        while (bullet != null)
        {
            percentage = elapsed / timeToMaxZoom;
            Vector3 zoomLerp = Vector3.Lerp(new Vector3(originalZoom, 0, 0), new Vector3(targetZoom, 0, 0), percentage);

            mainCam.orthographicSize = zoomLerp.x;

            elapsed += Time.fixedDeltaTime;

            yield return null;
        }

        resetingCameraCorutine = ResetCamera(1f);
        StartCoroutine(resetingCameraCorutine);

    }

    public IEnumerator CameraShake(float duration, float magnitude)
    {

        
        Vector3 originalPos = this.transform.position;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x,y,originalPos.z);


            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;

    }

    public IEnumerator CameraProgresiveShake(float duration, float intialMagnitude, float magnitudeProgression)
    {


        Vector3 originalPos = this.transform.position;

        float elapsed = 0.0f;
        float magnitude = 0;

        while (true)
        {
            
            if (elapsed < duration)
            {
                magnitude = intialMagnitude + magnitudeProgression;
            }
           

            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);


            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = Vector3.zero;

    }

    public IEnumerator CameraZoom(float duration, float zoom)
    {
       

        float originalZoom = mainCam.orthographicSize;

        float elapsed = 0.0f;
        float percentage = 0.0f;

        while (elapsed < duration)
        {
            percentage = elapsed / duration;
            Vector3 zoomLerp = Vector3.Lerp(new Vector3(originalZoom,0,0), new Vector3(zoom,0,0), percentage);

            mainCam.orthographicSize = zoomLerp.x;

            elapsed += Time.deltaTime;

            yield return null;
        }
    }

    public IEnumerator ResetCamera(float duration)
    {
        
        resetingCamera = true;
        float StartZoom = mainCam.orthographicSize;

        float elapsed = 0.0f;
        float percentage = 0.0f;

        Vector3 posOriginal = this.transform.position;
        Vector3 _posFinal = this.transform.position;

        _posFinal.x = Player.transform.position.x + (Player.transform.position.x - Player.transform.position.x) / 2;
        _posFinal.y = Player.transform.position.y + (Player.transform.position.y - Player.transform.position.y) / 2;

        if(constraintsEnabled) _posFinal = new Vector3(_posFinal.x, _posFinal.y + yOffset, originalZ);
        else _posFinal = new Vector3(_posFinal.x, _posFinal.y, originalZ);

        if (_posFinal.y < minY && constraintsEnabled)
        {
            _posFinal.y = minY;
        }
        
        cameraTarget = CameraTarget.PLAYER1;


        posOriginal.z = originalZ;
        posFinal.z = originalZ;


        while (elapsed < duration)
        {
            percentage = elapsed / duration;
            Vector3 zoomLerp = Vector3.Lerp(new Vector3(StartZoom, 0, 0), new Vector3(initialZoom, 0, 0), percentage);

            cameraHolder.transform.position = Vector3.Lerp(posOriginal, _posFinal, percentage);

            mainCam.orthographicSize = zoomLerp.x;

            elapsed += Time.fixedDeltaTime;

            yield return null;
        }

        

        CameraTargetPos.x = Player.transform.position.x + (Player.transform.position.x - Player.transform.position.x) / 2;
        CameraTargetPos.y = Player.transform.position.y + (Player.transform.position.y - Player.transform.position.y) / 2;

        if (constraintsEnabled)
            posFinal = new Vector3(CameraTargetPos.x, CameraTargetPos.y + yOffset, originalZ);
        else
            posFinal = new Vector3(CameraTargetPos.x, CameraTargetPos.y, originalZ);

        if (posFinal.y < minY && constraintsEnabled)
        {
            posFinal.y = minY;
        }

        resetingCamera = false;
        
    }

}
