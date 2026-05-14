using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyNonMoveState
{
    public EnemyIdleState(HSM hsm) : base(hsm)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        
        hsm.EnemyDriver.animator.SetBool("Idle", true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        if(hsm.EnemyDriver.death) hsm.SwitchState<EnemyDeathState>();
        
        if (hsm.EnemyDriver.targetGo != null)
        {
            //技能判定
            if (hsm.EnemyDriver.actualData.talentCooltimer == 0)
            {
                //冷却完毕，尝试释放技能
                float nowDistance = Vector3.Distance(hsm.EnemyDriver.transform.position, hsm.EnemyDriver.targetGo.transform.position);
                float maxSkillDistance = hsm.EnemyDriver.assetInfo.skillDataTable.skillMaxDistance;

                if (maxSkillDistance <= nowDistance)
                {
                    //距离超出，切换到移动状态开始靠近目标
                    hsm.SwitchState<EnemyMoveState>();
                }
                else
                {
                    hsm.SwitchState<EnemySkillState>();
                }
            }
            else
            {
                //尝试攻击
                float nowDistance = Vector3.Distance(hsm.EnemyDriver.transform.position, hsm.EnemyDriver.targetGo.transform.position);
                float maxAttackDistance = hsm.EnemyDriver.assetInfo.skillDataTable.attackMaxDistance;

                if (maxAttackDistance <= nowDistance)
                {
                    hsm.SwitchState<EnemyMoveState>();
                }
                else
                {
                    hsm.SwitchState<EnemyAttackState>();
                }
            }
        }
        else
        {
            //不存在目标对象，持续呆在Idle即可,判断是否处于待机点
            
            float idleDistance = Vector3.Distance(hsm.EnemyDriver.enemyAIData.startPos, hsm.EnemyDriver.transform.position);

            if (idleDistance >= 0.5f)
            {
                hsm.EnemyDriver.enemyAIData.targetPos = hsm.EnemyDriver.enemyAIData.startPos;
                hsm.SwitchState<EnemyMoveState>();
            }
        }
    }

    public override void OnExit()
    {
        hsm.EnemyDriver.animator.SetBool("Idle", false);
    }
}
