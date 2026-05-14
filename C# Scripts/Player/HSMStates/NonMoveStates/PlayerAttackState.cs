using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PLayerNonMoveState
{
    public PlayerAttackState(HSM hsm) : base(hsm)
    {
        
    }

    public override void OnEnter()
    {
        hsm.CHDriver.animator.SetBool("Attack", true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
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
            hsm.CHDriver.animator.SetBool("Attack", true);
        }
        
        if (!hsm.CHDriver.inputData.attack && !hsm.CHDriver.animator.GetBool("StateLocked"))
        {
            if (hsm.CHDriver.inputData.crouch || hsm.CHDriver.inputData.moveDirection == Vector3.zero)
            {
                hsm.SwitchState<PlayerIdleState>();
            }
            else if (hsm.CHDriver.inputData.jump || hsm.CHDriver.inputData.moveDirection != Vector3.zero)
            {
                hsm.SwitchState<PlayerMoveState>();
            }
        }
    }

    public override void OnExit()
    {
        hsm.CHDriver.animator.SetBool("Attack", false);
    }
}
