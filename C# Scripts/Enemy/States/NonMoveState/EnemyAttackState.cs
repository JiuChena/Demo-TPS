using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyNonMoveState
{
    public EnemyAttackState(HSM hsm) : base(hsm)
    {
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        
        hsm.EnemyDriver.animator.SetBool("Attack", true);
    }

    public override void OnUpdate()
    {
        if(hsm.EnemyDriver.death) hsm.SwitchState<EnemyDeathState>();
        
        if (!hsm.EnemyDriver.enemyAIData.stateOccupy)
        {
            if (hsm.EnemyDriver.assetInfo.ammunitionCapacity != -1)
            {
                if (hsm.EnemyDriver.actualData.bulletCount == 0)
                {
                    hsm.SwitchState<EnemyReloadState>();
                    return;
                }
            }
            
            if (hsm.EnemyDriver.targetGo != null)
            {
                //技能判定
                if (hsm.EnemyDriver.actualData.talentCooltimer == 0)
                {
                    //冷却完毕，尝试释放技能
                    Debug.Log("冷却完毕尝试释放技能");
                    float nowDistance = Vector3.Distance(hsm.EnemyDriver.transform.position, hsm.EnemyDriver.targetGo.transform.position);
                    float maxSkillDistance = hsm.EnemyDriver.assetInfo.skillDataTable.skillMaxDistance;

                    if (maxSkillDistance <= nowDistance)
                    {
                        //距离超出，切换到移动状态开始靠近目标
                        Debug.Log("超出技能释放距离，切换移动状态尝试靠近");
                        hsm.SwitchState<EnemyMoveState>();
                    }
                    else
                    {
                        Debug.Log("切换至技能状态");
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
                        hsm.EnemyDriver.animator.SetBool("Attack", true);
                    }
                }
            }
            else
            {
                hsm.SwitchState<EnemyIdleState>();
            }
        }
    }

    public override void OnExit()
    {
        hsm.EnemyDriver.animator.SetBool("Attack", false);
    }
}
