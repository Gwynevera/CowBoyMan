using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnparentOnStart : MonoBehaviour
{
    
    void Start()
    {
        this.transform.SetParent(null);
    }

    
}
