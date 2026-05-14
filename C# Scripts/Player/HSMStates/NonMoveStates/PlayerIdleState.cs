using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PLayerNonMoveState
{
    public PlayerIdleState(HSM hsm) : base(hsm)
    {
        
    }

    public override void OnEnter()
    {
        hsm.CHDriver.animator.SetBool("Idle", true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        hsm.CHDriver.animator.SetBool("Crouch", hsm.CHDriver.inputData.crouch);

        if (hsm.CHDriver.inputData.burst || hsm.CHDriver.inputData.talent)
        {
            hsm.SwitchState<PlayerSkillState>();
        }
        else if (hsm.CHDriver.inputData.reload)
        {
            hsm.SwitchState<PlayerReloadState>();
        }
        else if (hsm.CHDriver.inputData.attack)
        {
            hsm.SwitchState<PlayerAttackState>();
        }
        else if (!hsm.CHDriver.inputData.crouch && (hsm.CHDriver.inputData.jump || hsm.CHDriver.inputData.moveDirection != Vector3.zero))
        {
            hsm.SwitchState<PlayerMoveState>();
        }
        
        if(!hsm.CHDriver.animator.GetBool("Active")) hsm.CHDriver.gameObject.SetActive(false);
    }

    public override void OnExit()
    {
        hsm.CHDriver.animator.SetBool("Idle", false);
    }
}
