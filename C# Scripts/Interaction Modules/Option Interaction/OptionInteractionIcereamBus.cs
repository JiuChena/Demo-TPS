using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionInteractionIcereamBus : OptionInteractionBase
{
    public string txet;
    protected override void ActionTrigger()
    {
        Debug.Log(txet);
    }

    protected override bool FilterDetection(GameObject target)
    {
        if(target.layer == LayerMask.NameToLayer("Player")) return true;
        else return false;
    }
}
