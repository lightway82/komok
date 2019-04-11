using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableCommon : DestoyableObject
{
    public override void OnTriggerEnterAction(bool destroy)
    {
        if (destroy)
        {
            gameObject.SetActive(false);
        }
    }

   
}
