using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableTree : DestoyableObject
{
    public override void OnTriggerEnterAction(bool destroy)
    {
        if (destroy)
        {
            gameObject.SetActive(false);
        }
    }

   
}
