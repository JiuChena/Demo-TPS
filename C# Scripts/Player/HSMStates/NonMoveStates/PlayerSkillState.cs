using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillState : PLayerNonMoveState
{
    public PlayerSkillState(HSM hsm) : base(hsm)
    {
        
    }

    public override void OnEnter()
    {
        if(hsm.CHDriver.inputData.burst) hsm.CHDriver.animator.SetBool("Burst", true);
        else if(hsm.CHDriver.inputData.talent) hsm.CHDriver.animator.SetBool("Talent", true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        if (!hsm.CHDriver.animator.GetBool("Burst") && hsm.CHDriver.inputData.burst)
        {
            if(hsm.CHDriver.animator.GetBool("Talent")) hsm.CHDriver.animator.SetBool("Talent", false);
            
            hsm.CHDriver.animator.SetBool("Burst", true);
        }
        
        if ((!hsm.CHDriver.inputData.burst && !hsm.CHDriver.inputData.talent))
        {
            if (hsm.CHDriver.inputData.reload)
            {
                hsm.SwitchState<PlayerReloadState>();
            }
            else if (hsm.CHDriver.inputData.attack)
            {
                hsm.SwitchState<PlayerAttackState>();
            }
            else if (hsm.CHDriver.inputData.jump || hsm.CHDriver.inputData.moveDirection != Vector3.zero)
            {
                hsm.SwitchState<PlayerMoveState>();
            }
            else if (hsm.CHDriver.inputData.crouch || hsm.CHDriver.inputData.moveDirection == Vector3.zero)
            {
                hsm.SwitchState<PlayerIdleState>();
            }
        }
    }

    public override void OnExit()
    {
        hsm.CHDriver.animator.SetBool("Burst", false);
        hsm.CHDriver.animator.SetBool("Talent", false);
    }
}
