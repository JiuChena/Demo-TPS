using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackingAnimation : StateMachineBehaviour
{
    private EnemyAnimatorDriver driver;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Attack", false);
        driver = animator.GetComponent<EnemyAnimatorDriver>();
        driver.enemyAIData.moveAllowed = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.3f)
        {
            Vector3 taregtDir = Vector3.ProjectOnPlane(driver.targetGo.transform.position - driver.transform.position, Vector3.up).normalized;
            Quaternion ro = Quaternion.LookRotation(taregtDir, Vector3.up);
            driver.transform.rotation = Quaternion.Lerp(driver.transform.rotation, ro, Time.deltaTime * 15f);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<EnemyAnimatorDriver>().enemyAIData.moveAllowed = true;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
