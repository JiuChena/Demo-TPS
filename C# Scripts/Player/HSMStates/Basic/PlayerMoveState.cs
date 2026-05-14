using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : StateBase
{
    public PlayerMoveState(HSM hsm) : base(hsm)
    {
    }
    
    private float stateTime = 0f;

    public override void OnEnter()
    {
        hsm.CHDriver.animator.SetBool("Move", true);
        stateTime = 0f;
    }

    public override void OnUpdate()
    {
        StateChecker();

        MoveHandle();
    }

    public override void OnExit()
    {
        hsm.CHDriver.animator.SetBool("Move", false);
    }

    /// <summary>
    /// 状态检查
    /// </summary>
    private void StateChecker()
    {
        if(hsm.CHDriver.death) hsm.SwitchState<PlayerDeathState>();
        
        if (hsm.CHDriver.inputData.jump)
        {
            hsm.CHDriver.animator.SetBool("Jump", true);
        }
        else
        {
            //优先级：技能>换弹>攻击>蹲起/跳跃>行走/待机
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
            else if (hsm.CHDriver.inputData.crouch || hsm.CHDriver.inputData.moveDirection == Vector3.zero)
            {
                hsm.SwitchState<PlayerIdleState>();
            }
        }
    }

    /// <summary>
    /// 移动处理
    /// </summary>
    private void MoveHandle()
    {
        bool isJumping = hsm.CHDriver.inputData.jump;
        float moveSpeed = isJumping ? 1 : MoveStateTimer() * hsm.CHDriver.CHAssetInfo.dataBase.moveSpeed;
        
        Vector3 moveDirection = isJumping ? 
            hsm.CHDriver.inputData.crouchJumpDir : hsm.CHDriver.inputData.moveDirection;
        
        Quaternion rotation = hsm.CHDriver.transform.rotation;
        Quaternion lookRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        
        hsm.CHDriver.cc.transform.rotation = Quaternion.Lerp(rotation ,lookRotation, Time.deltaTime * 10);
        hsm.CHDriver.cc.SimpleMove(moveSpeed * moveDirection);
    }

    private float MoveStateTimer()
    {
        stateTime = Mathf.Clamp(0.3f, 1, stateTime + Time.deltaTime);

        return stateTime;
    }
}
