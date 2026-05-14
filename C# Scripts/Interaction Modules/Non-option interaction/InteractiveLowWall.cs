using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveLowWall : InteractionBasicModule
{
    private JumpDirection jumpDirection;
    
    private CharacterAnimatorDriver CHDriver;
    private EnemyAnimatorDriver ENDriver;

    protected override void OnUpdateFunction()
    {
        base.OnUpdateFunction();
    }

    protected override bool FilterDetection(GameObject target)
    {
        if(!target.CompareTag("Player") || !target.CompareTag("Enemy")) return true;
        else return false;
    }

    protected override void TriggerEnterPerformance(GameObject target)
    {
        if (target.CompareTag("Player"))
        {
            PlayerControlModule.Instance.inputData.isAllowedCrouchOrJump = true;
            CHDriver = target.GetComponent<CharacterAnimatorDriver>();
        }
        else if (target.CompareTag("Enemy"))
        {
            ENDriver = target.GetComponent<EnemyAnimatorDriver>();
            ENDriver.enemyAIData.JCAllowed = true;
        }
    }

    protected override void TriggerStayPerformance(GameObject target)
    {
        if (target.CompareTag("Player"))
        {
            if (!PlayerControlModule.Instance.inputData.jump)
            {
                Vector3 wallToPlayer = Vector3.ProjectOnPlane(target.transform.position - this.transform.position, this.transform.up);
        
                jumpDirection = Vector3.Angle(wallToPlayer, this.transform.forward) > 90 ? JumpDirection.PositiveZ_Aixs : JumpDirection.NegativeZ_Aixs;

                if (jumpDirection == JumpDirection.PositiveZ_Aixs)
                {
                    PlayerControlModule.Instance.inputData.crouchJumpDir = this.transform.forward;
                }
                else
                {
                    PlayerControlModule.Instance.inputData.crouchJumpDir = -this.transform.forward;
                }
            }
        }    
        else if (target.CompareTag("Enemy"))
        {
            Vector3 wallToPlayer = Vector3.ProjectOnPlane(target.transform.position - this.transform.position, this.transform.up);
        
            jumpDirection = Vector3.Angle(wallToPlayer, this.transform.forward) > 90 ? JumpDirection.PositiveZ_Aixs : JumpDirection.NegativeZ_Aixs;

            if (jumpDirection == JumpDirection.PositiveZ_Aixs)
            {
                ENDriver.enemyAIData.JCDir = this.transform.forward;
            }
            else
            {
                ENDriver.enemyAIData.JCDir = -this.transform.forward;
            }
            
            ENDriver.enemyAIData.unitToBunkerDir = (this.transform.position - ENDriver.transform.position).normalized;
        }
    }

    protected override void TriggerExitPerformance(GameObject target)
    {
        if(target.CompareTag("Player")) PlayerControlModule.Instance.inputData.isAllowedCrouchOrJump = false;
        else if (target.CompareTag("Enemy"))
        {
            ENDriver.enemyAIData.JCAllowed = false;
        }
    }
}

public enum JumpDirection
{
    PositiveZ_Aixs,
    NegativeZ_Aixs,
}
