using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLayerNonMoveState : StateBase
{
    public PLayerNonMoveState(HSM hsm) : base(hsm)
    {
    }

    public override void OnEnter()
    {
        
    }

    public override void OnUpdate()
    {
        if(hsm.CHDriver.death) hsm.SwitchState<PlayerDeathState>();
    }

    public override void OnExit()
    {
        
    }
}
