using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : Interactable
{
    [SerializeField] private List<Interactable> linkedInteractables = new List<Interactable>();
    [SerializeField] private Boolean singleAction = false;
    [SerializeField] private Boolean active = true;

    public override void activate()
    {
        Debug.Log("Esta vaina se ha activao");
        if (active)
        {

            StartCoroutine(buttonAction());
            if (singleAction)
            {
                active = false;
            }
        }
        
    }

    public IEnumerator buttonAction()
    {

        Debug.Log("Esta vaina se ha activao 2");
        for (int i = 0; i < linkedInteractables.Count; i++)
        {
            linkedInteractables[i].activate();
        }

        yield return null;

    }
}
