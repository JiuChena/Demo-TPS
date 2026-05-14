using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackingAnimation : StateMachineBehaviour
{
    private CharacterAnimatorDriver driver;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        driver = animator.GetComponent<CharacterAnimatorDriver>();
        if (PlayerControlModule.Instance.GetCHAnimatorDriver == driver)
        {
            PlayerControlModule.Instance.inputData.attack = false;
        }

        driver.inputData.attack = false;
        animator.SetBool("Attack", false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Quaternion rotation = animator.transform.rotation;
        Quaternion lookRotation = Quaternion.LookRotation(driver.inputData.attackDir, Vector3.up);
        
        animator.transform.rotation = Quaternion.Lerp(rotation, lookRotation, Time.deltaTime * 5f);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
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
