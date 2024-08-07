using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    [SerializeField] private Boolean singleAction = false;
    [SerializeField] private Boolean active = true;
    [SerializeField] private float duration = 3;
    private GameObject destination;

    // Start is called before the first frame update
    void Start()
    {
        destination = GameObject.Find("Destination");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void activate()
    {
        if (active)
        {
            Debug.Log("Esta vaina se ha activao");
            // LERP
            // Crear objeto vacío dentro del prefab

            
            if (singleAction)
            {
                active = false;
            } else
            {
                StartCoroutine(doorMovement());
            }

        }
        
    }

    public IEnumerator doorMovement() {
        float elapsed = 0.0f;
        float percentage = 0.0f;
        destination.transform.SetParent(null);
        Vector3 initialPos = transform.position;

        while (elapsed < duration)
        {
            percentage = elapsed / duration;
            Vector3 moveLerp = Vector3.Lerp(initialPos, destination.transform.position, percentage);

            transform.position = moveLerp;

            elapsed += Time.deltaTime;
            yield return null;
        }

        destination.transform.SetParent(transform);
    }
}
