using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathState : StateBase
{
    public EnemyDeathState(HSM hsm) : base(hsm)
    {
        
    }
    
    private float deathTime = 3f;

    public override void OnEnter()
    {
        hsm.EnemyDriver.animator.SetTrigger("Death");
    }

    public override void OnUpdate()
    {
        deathTime -= Time.deltaTime;

        if (deathTime <= 0f)
        {
            hsm.EnemyDriver.gameObject.SetActive(false);
            
            ObjectsPool.Instance.ReturnObjectToPool(hsm.EnemyDriver.gameObject, (go) =>
            {
                go.SetActive(true);
                hsm.EnemyDriver.death = false;
            });
        }
    }

    public override void OnExit()
    {
        
    }
}
