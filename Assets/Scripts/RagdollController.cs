using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{

    public bool ragdollLeftArm = false;
    public bool dropHat = false;

    [Header("Ragdoll Model")]
    [SerializeField] public Transform head;
    [SerializeField] public Transform downLegR;
    [SerializeField] public Transform uperLegR;
    [SerializeField] public Transform downLegL;
    [SerializeField] public Transform uperLegL;
    [SerializeField] public Transform bodyDown;
    [SerializeField] public Transform bodyUp;
    [SerializeField] public Transform downArmR;
    [SerializeField] public Transform upArmR;
    [SerializeField] public Transform downArmL;
    [SerializeField] public Transform upArmL;
    [SerializeField] public Transform hat;
    [SerializeField] public GameObject TargetLArm;

    [Header("Animated Model")]
    [SerializeField] private GameObject AnimatedModel;
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _downLegR;
    [SerializeField] private Transform _uperLegR;
    [SerializeField] private Transform _downLegL;
    [SerializeField] private Transform _uperLegL;
    [SerializeField] private Transform _bodyDown;
    [SerializeField] private Transform _bodyUp;
    [SerializeField] private Transform _downArmR;
    [SerializeField] private Transform _upArmR;
    [SerializeField] private Transform _downArmL;
    [SerializeField] private Transform _upArmL;
    [SerializeField] private Transform _hat;

    private void Start()
    {
        IgnoreArmCollisions();
    }

    private void Update()
    {
        head.position = _head.position; 
        head.rotation = _head.rotation; 

        downLegR.position = _downLegR.position; 
        downLegR.rotation = _downLegR.rotation; 

        uperLegR.position = _uperLegR.position; 
        uperLegR.rotation = _uperLegR.rotation; 

        downLegL.position = _downLegL.position;
        downLegL.rotation = _downLegL.rotation;

        uperLegL.position = _uperLegL.position;
        uperLegL.rotation = _uperLegL.rotation;

        bodyDown.position = _bodyDown.position;
        bodyDown.rotation = _bodyDown.rotation;

        bodyUp.position = _bodyUp.position;
        bodyUp.rotation = _bodyUp.rotation;

        downArmR.position = _downArmR.position;
        downArmR.rotation = _downArmR.rotation;

        upArmR.position = _upArmR.position;
        upArmR.rotation = _upArmR.rotation;

        if (!ragdollLeftArm) { 
            downArmL.position = _downArmL.position;
            downArmL.rotation = _downArmL.rotation;

            upArmL.position = _upArmL.position;
            upArmL.rotation = _upArmL.rotation;
        }
        if (!dropHat) { 
            hat.position = _hat.position;
            hat.rotation = _hat.rotation;
        }
        else
        {
            _hat.gameObject.SetActive(false);
        }
    }

    public void DestoyAnimatedModel()
    {
        bodyUp.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        Destroy(TargetLArm.gameObject);
        Destroy(AnimatedModel.gameObject);
        Destroy(this);
    }

    private void IgnoreArmCollisions()
    {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        


        Collider2D upArmCollider = upArmL.GetComponent<Collider2D>();
        Collider2D downArmCollider = downArmL.GetComponent<Collider2D>();

        for (int i = 0; i < colliders.Length; i++ )
        {
            Physics2D.IgnoreCollision(upArmCollider, colliders[i]);
            Physics2D.IgnoreCollision(downArmCollider, colliders[i]);
        
        }

    }

}
