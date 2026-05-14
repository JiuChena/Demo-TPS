using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNonMoveState : StateBase
{
    public EnemyNonMoveState(HSM hsm) : base(hsm)
    {
    }

    public override void OnEnter()
    {
        hsm.EnemyDriver.agent.Warp(hsm.EnemyDriver.transform.position);
        hsm.EnemyDriver.agent.isStopped = true;
    }

    public override void OnUpdate()
    {
        //判断是否允许蹲下【角色特殊能力是否支持？是否进入遮蔽物范围？】
        
        //技能>攻击>换弹（攻击失败）>>>>
        //1-检查是否满足技能释放条件（冷却）
        //2-如果技能冷却未结束则尝试进行攻击行为，如果载弹量不足则进行3
        //3-换弹
        
        //每次行为满足条件进行执行时先进行查验，如果不满足目标行为最大距离则都切换到Move先进行移动

        // if (hsm.EnemyDriver.targetGo != null)
        // {
        //     if (hsm.EnemyDriver.actualData.talentCooltimer == 0)
        //     {
        //         Debug.Log("尝试释放技能");
        //         //检查技能最大释放距离
        //         float nowDistance = Vector3.Distance(hsm.EnemyDriver.transform.position, hsm.EnemyDriver.enemyAIData.targetPos);
        //         float maxDistance = hsm.EnemyDriver.assetInfo.skillDataTable.skillMaxDistance;
        //
        //         if (nowDistance > maxDistance)
        //         {
        //             Debug.Log("释放距离超出");
        //             hsm.EnemyDriver.agent.SetDestination(hsm.EnemyDriver.enemyAIData.targetPos - hsm.EnemyDriver.assetInfo.skillDataTable.minDistance * (hsm.EnemyDriver.enemyAIData.targetPos - hsm.EnemyDriver.transform.position).normalized);
        //             hsm.SwitchState<EnemyMoveState>();
        //         }
        //         else hsm.SwitchState<EnemySkillState>();
        //     }
        //     else
        //     {
        //         //检查技能最大释放距离
        //         float nowDistance = Vector3.Distance(hsm.EnemyDriver.transform.position, hsm.EnemyDriver.enemyAIData.targetPos);
        //         float maxDistance = hsm.EnemyDriver.assetInfo.skillDataTable.attackMaxDistance;
        //
        //         if (nowDistance > maxDistance)
        //         {
        //             hsm.EnemyDriver.agent.SetDestination(hsm.EnemyDriver.enemyAIData.targetPos - hsm.EnemyDriver.assetInfo.skillDataTable.minDistance * (hsm.EnemyDriver.enemyAIData.targetPos - hsm.EnemyDriver.transform.position).normalized);
        //             hsm.SwitchState<EnemyMoveState>();
        //         }
        //         else hsm.SwitchState<EnemyAttackState>();
        //     }
        // }
        // else
        // {
        //     hsm.EnemyDriver.agent.SetDestination(hsm.EnemyDriver.enemyAIData.startPos);
        //     hsm.SwitchState<EnemyIdleState>();
        // }
    }

    public override void OnExit()
    {
        
    }
}
