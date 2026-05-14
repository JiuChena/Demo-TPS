using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkillState : EnemyNonMoveState
{
    public EnemySkillState(HSM hsm) : base(hsm)
    {
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        
        hsm.EnemyDriver.animator.SetBool("Skill", true);
        hsm.EnemyDriver.actualData.talentCooltimer = hsm.EnemyDriver.assetInfo.skillDataTable.skillCoolTime;
        hsm.EnemyDriver.enemyAIData.stateOccupy = true;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (!hsm.EnemyDriver.enemyAIData.stateOccupy)
        {
            if (hsm.EnemyDriver.targetGo != null)
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
                Debug.Log("尝试攻击");
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
        
        hsm.EnemyDriver.animator.SetBool("Skill", false);
    }
}
