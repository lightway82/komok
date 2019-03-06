using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseLocalizedComponent : MonoBehaviour, ILocalizedItem
{
   
    virtual public void Start()
    {
        
    }

    public abstract void SetLocalizedData();
    

    virtual public void OnDestroy()
    {
        
    }
}
