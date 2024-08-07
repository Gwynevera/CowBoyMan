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

        for (int i = 0; i < linkedInteractables.Count; i++)
        {
            linkedInteractables[i].activate();
        }

        yield return null;

    }
}
