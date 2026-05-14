using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMoveState : StateBase
{
    public EnemyMoveState(HSM hsm) : base(hsm)
    {
    }

    public override void OnEnter()
    {
        hsm.EnemyDriver.animator.SetBool("Move", true);
        
        hsm.EnemyDriver.animator.SetBool("Crouch", false);
        
        hsm.EnemyDriver.agent.isStopped = false;
    }

    private Vector3 pos;
    private bool reset = false;

    public override void OnUpdate()
    {
        if(hsm.EnemyDriver.death) hsm.SwitchState<EnemyDeathState>();
        
        if(!hsm.EnemyDriver.enemyAIData.moveAllowed) return;
        
        if (hsm.EnemyDriver.targetGo != null)
        {
            Debug.Log("有目标追踪");
            //用球形范围检测检测出附近的遮挡掩体，检测到遮挡掩体之后根据掩体位置和角色位置计算出掩体和角色之间的距离，掩体位置 + 角色->掩体向量 在 掩体transform.forward方向投影.normalized * 0.2f
            
            if (hsm.EnemyDriver.actualData.talentCooltimer == 0)
            {
                //检查技能最大释放距离
                float nowDistance = Vector3.Distance(hsm.EnemyDriver.transform.position, hsm.EnemyDriver.enemyAIData.targetPos);
                float maxDistance = hsm.EnemyDriver.assetInfo.skillDataTable.skillMaxDistance;
                
                SetTargetPoint(maxDistance, nowDistance);

                if (nowDistance < maxDistance) hsm.SwitchState<EnemySkillState>();
            }
            else
            {
                //检查技能最大释放距离
                float nowDistance = Vector3.Distance(hsm.EnemyDriver.transform.position, hsm.EnemyDriver.enemyAIData.targetPos);
                float maxDistance = hsm.EnemyDriver.assetInfo.skillDataTable.attackMaxDistance;
                
                SetTargetPoint(maxDistance, nowDistance);

                if (nowDistance < maxDistance) hsm.SwitchState<EnemyAttackState>();
            }
        }
        else
        {
            Debug.Log("无目标追踪");
            
            hsm.EnemyDriver.agent.SetDestination(hsm.EnemyDriver.enemyAIData.startPos);
            Vector3 moveDir = hsm.EnemyDriver.agent.desiredVelocity.normalized;
            hsm.EnemyDriver.enemyAIData.navMoveDir = moveDir;
            if(!hsm.EnemyDriver.enemyAIData.jump) hsm.EnemyDriver.enemyAIData.moveDir = moveDir;
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            hsm.EnemyDriver.transform.rotation = Quaternion.Slerp(hsm.EnemyDriver.transform.rotation, targetRot, Time.deltaTime * 10);
                    
            if(Vector3.Distance(hsm.EnemyDriver.agent.transform.position, hsm.EnemyDriver.transform.position) > 0.3f)
            {
                hsm.EnemyDriver.agent.Warp(hsm.EnemyDriver.transform.position);
                hsm.EnemyDriver.agent.transform.rotation = Quaternion.identity;
            }

            if ((hsm.EnemyDriver.assetInfo.specialAction == SpecialActionAcpability.Both || hsm.EnemyDriver.assetInfo.specialAction == SpecialActionAcpability.Jump) && hsm.EnemyDriver.enemyAIData.JCAllowed && !hsm.EnemyDriver.enemyAIData.jump)
            {
                //如果进入障碍物范围并且移动方向与障碍物方向或反方向基本重合，则进行跳跃
                float angle = Vector3.Angle(hsm.EnemyDriver.enemyAIData.moveDir,
                    hsm.EnemyDriver.enemyAIData.unitToBunkerDir);
                bool wantToJumpOver = angle <= 30 ? true : false;

                if (wantToJumpOver)
                {
                    hsm.EnemyDriver.enemyAIData.jump = true;
                    hsm.EnemyDriver.animator.SetBool("Jump", true);
                        
                    hsm.EnemyDriver.enemyAIData.stateOccupy = true;
                }
            }

            if (hsm.EnemyDriver.enemyAIData.jump)
            {
                hsm.EnemyDriver.cc.SimpleMove(hsm.EnemyDriver.enemyAIData.moveDir * 0.8f);
            }
            else
            {
                hsm.EnemyDriver.cc.SimpleMove(hsm.EnemyDriver.enemyAIData.moveDir * 4);
            }
            
            if (Vector3.Distance(hsm.EnemyDriver.transform.position, hsm.EnemyDriver.enemyAIData.startPos) < 0.2f)
            {
                hsm.SwitchState<EnemyIdleState>();
            }
        }

        //卡地形处理
        if (!reset)
        {
            if (pos != hsm.EnemyDriver.transform.position)
            {
                pos = hsm.EnemyDriver.transform.position;
            }
            else
            {
                reset = true;
            }
        }
        else
        {
            if(pos == hsm.EnemyDriver.transform.position) hsm.EnemyDriver.agent.Warp(hsm.EnemyDriver.transform.position);
            
            reset = false;
            pos = hsm.EnemyDriver.transform.position;
        }
        
        //处理Nav移速
        // hsm.EnemyDriver.agent.speed = hsm.EnemyDriver.actualData.speed;
    }

    public override void OnExit()
    {
        hsm.EnemyDriver.animator.SetBool("Move", false);
    }

    private void SetTargetPoint(float skillMaxDistance, float nowDistance)
    {
        if(hsm.EnemyDriver.agent.isStopped) hsm.EnemyDriver.agent.isStopped = false;
        
        if (hsm.EnemyDriver.assetInfo.specialAction != SpecialActionAcpability.Neither)
        {
            Debug.Log("可利用遮蔽物");
            Collider[] colliders = Physics.OverlapSphere(hsm.EnemyDriver.transform.position, 5, 1 << LayerMask.NameToLayer("Passable"));

            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    Vector3 passablePos = colliders[i].transform.position;
                    Vector3 targetToPassableDir = passablePos - hsm.EnemyDriver.targetGo.transform.position;
                    Vector3 vecProjector = Vector3.ProjectOnPlane(targetToPassableDir, colliders[i].transform.forward).normalized;
                    Vector3 realTargetPos = colliders[i].transform.position + vecProjector * 0.2f;
                
                    float bunkerToTargetDistance = Vector3.Distance(realTargetPos, hsm.EnemyDriver.targetGo.transform.position);

                    if (bunkerToTargetDistance < skillMaxDistance - 0.5f && bunkerToTargetDistance - nowDistance < 1f)
                    {
                        Debug.Log("决定优先导航至遮蔽物处");
                        float pathLength = GetPathLength(hsm.EnemyDriver.agent);
                        if (pathLength > hsm.EnemyDriver.maxFindBunkerPathLength) hsm.EnemyDriver.agent.SetDestination(hsm.EnemyDriver.targetGo.transform.position);
                        else hsm.EnemyDriver.agent.SetDestination(realTargetPos);
                        
                        Vector3 moveDir = hsm.EnemyDriver.agent.desiredVelocity.normalized;
                        hsm.EnemyDriver.enemyAIData.navMoveDir = moveDir;
                        if(!hsm.EnemyDriver.enemyAIData.jump) hsm.EnemyDriver.enemyAIData.moveDir = moveDir;
                        Quaternion targetRot = Quaternion.LookRotation(moveDir);
                        hsm.EnemyDriver.transform.rotation = Quaternion.Slerp(hsm.EnemyDriver.transform.rotation, targetRot, Time.deltaTime * 10);
                        
                        if(Vector3.Distance(hsm.EnemyDriver.agent.transform.position, hsm.EnemyDriver.transform.position) > 0.3f)
                        {
                            hsm.EnemyDriver.agent.Warp(hsm.EnemyDriver.transform.position);
                            hsm.EnemyDriver.agent.transform.rotation = Quaternion.identity;
                        }
                        
                        hsm.EnemyDriver.cc.SimpleMove(hsm.EnemyDriver.enemyAIData.moveDir * 4);
                        
                        break;
                    }
                    else
                    {
                        Debug.Log("决定追踪锁定单位");
                        hsm.EnemyDriver.agent.SetDestination(hsm.EnemyDriver.targetGo.transform.position);
                        Vector3 moveDir = hsm.EnemyDriver.agent.desiredVelocity.normalized;
                        hsm.EnemyDriver.enemyAIData.navMoveDir = moveDir;
                        if(!hsm.EnemyDriver.enemyAIData.jump) hsm.EnemyDriver.enemyAIData.moveDir = moveDir;
                        Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
                        hsm.EnemyDriver.transform.rotation = Quaternion.Slerp(hsm.EnemyDriver.transform.rotation, targetRot, Time.deltaTime * 10);
                        
                        if(Vector3.Distance(hsm.EnemyDriver.agent.transform.position, hsm.EnemyDriver.transform.position) > 0.3f)
                        {
                            hsm.EnemyDriver.agent.Warp(hsm.EnemyDriver.transform.position);
                            hsm.EnemyDriver.agent.transform.rotation = Quaternion.identity;
                        }

                        if (hsm.EnemyDriver.enemyAIData.JCAllowed && !hsm.EnemyDriver.enemyAIData.jump)
                        {
                            //如果进入障碍物范围并且移动方向与障碍物方向或反方向基本重合，则进行跳跃
                            float angle = Vector3.Angle(hsm.EnemyDriver.enemyAIData.moveDir, hsm.EnemyDriver.enemyAIData.unitToBunkerDir);
                            bool wantToJumpOver = angle <= 30 ? true : false;

                            if (wantToJumpOver)
                            {
                                hsm.EnemyDriver.enemyAIData.jump = true;
                                hsm.EnemyDriver.animator.SetBool("Jump", true);
                            
                                hsm.EnemyDriver.enemyAIData.stateOccupy = true;
                            }
                        }


                        if (hsm.EnemyDriver.enemyAIData.jump)
                        {
                            hsm.EnemyDriver.cc.SimpleMove(hsm.EnemyDriver.enemyAIData.moveDir * 0.8f);
                        }
                        else
                        {
                            hsm.EnemyDriver.cc.SimpleMove(hsm.EnemyDriver.enemyAIData.moveDir * 4);
                        }
                        
                        break;
                    }
                }
            }
            else
            {
                hsm.EnemyDriver.agent.SetDestination(hsm.EnemyDriver.targetGo.transform.position);
                Vector3 moveDir = hsm.EnemyDriver.agent.desiredVelocity.normalized;
                hsm.EnemyDriver.enemyAIData.navMoveDir = moveDir;
                if(!hsm.EnemyDriver.enemyAIData.jump) hsm.EnemyDriver.enemyAIData.moveDir = moveDir;
                hsm.EnemyDriver.cc.SimpleMove(hsm.EnemyDriver.enemyAIData.moveDir * 4);
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                hsm.EnemyDriver.transform.rotation = Quaternion.Slerp(hsm.EnemyDriver.transform.rotation, targetRot, Time.deltaTime * 10);
            
                if(Vector3.Distance(hsm.EnemyDriver.agent.transform.position, hsm.EnemyDriver.transform.position) > 0.3f)
                {
                    hsm.EnemyDriver.agent.Warp(hsm.EnemyDriver.transform.position);
                    hsm.EnemyDriver.agent.transform.rotation = Quaternion.identity;
                }
            }
        }
        else
        {
            Debug.Log("不可利用遮蔽物");
            hsm.EnemyDriver.agent.SetDestination(hsm.EnemyDriver.targetGo.transform.position);
            Vector3 moveDir = hsm.EnemyDriver.agent.desiredVelocity.normalized;
            hsm.EnemyDriver.enemyAIData.navMoveDir = moveDir;
            if(!hsm.EnemyDriver.enemyAIData.jump) hsm.EnemyDriver.enemyAIData.moveDir = moveDir;
            hsm.EnemyDriver.cc.SimpleMove(hsm.EnemyDriver.enemyAIData.moveDir * 4);
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            hsm.EnemyDriver.transform.rotation = Quaternion.Slerp(hsm.EnemyDriver.transform.rotation, targetRot, Time.deltaTime * 10);
            
            if(Vector3.Distance(hsm.EnemyDriver.agent.transform.position, hsm.EnemyDriver.transform.position) > 0.3f)
            {
                hsm.EnemyDriver.agent.Warp(hsm.EnemyDriver.transform.position);
                hsm.EnemyDriver.agent.transform.rotation = Quaternion.identity;
            }
        }
    }

    private float GetPathLength(NavMeshAgent agent)
    {
        if (agent.path == null || agent.path.corners.Length < 2)
        {
            return 0f;
        }

        Vector3[] corners = agent.path.corners;
        float totalDistance = 0f;

        // 遍历所有拐点，累加相邻两点间的距离
        for (int i = 0; i < corners.Length - 1; i++)
        {
            totalDistance += Vector3.Distance(corners[i], corners[i + 1]);
        }

        return totalDistance;
    }
}
