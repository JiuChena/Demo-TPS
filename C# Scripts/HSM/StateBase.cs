using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase
{
    protected HSM hsm;
    
    public StateBase(HSM hsm)
    {
        this.hsm = hsm;
    }
    
    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();
}
