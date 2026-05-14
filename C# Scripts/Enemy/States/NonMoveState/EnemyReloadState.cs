using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyReloadState : EnemyNonMoveState
{
    public EnemyReloadState(HSM hsm) : base(hsm)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        
        hsm.EnemyDriver.animator.SetBool("Reload", true);

        hsm.EnemyDriver.enemyAIData.stateOccupy = true;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        if(hsm.EnemyDriver.death) hsm.SwitchState<EnemyDeathState>();
        
        if (!hsm.EnemyDriver.enemyAIData.stateOccupy)
        {
            if (hsm.EnemyDriver.targetGo != null)
            {
                Quaternion ro = Quaternion.LookRotation(hsm.EnemyDriver.targetGo.transform.position - hsm.EnemyDriver.transform.position, Vector3.up);
                hsm.EnemyDriver.transform.rotation = Quaternion.Lerp(hsm.EnemyDriver.transform.rotation, ro, Time.deltaTime * 10f);
                
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
                        hsm.SwitchState<EnemyAttackState>();
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
        base.OnExit();
        
        hsm.EnemyDriver.animator.SetBool("Reload", false);
        
        hsm.EnemyDriver.actualData.bulletCount = hsm.EnemyDriver.assetInfo.ammunitionCapacity;
    }
}
