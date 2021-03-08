using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenIdleBehaviour : StateMachineBehaviour
{
    private float timerIndex;
    private Citizen citizenAI;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        citizenAI = animator.GetComponent<Citizen>();
        timerIndex = citizenAI.GetRandomFloat(10, 50);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timerIndex -= 1 * Time.deltaTime;
        
        if(timerIndex <= 0)
        {
            citizenAI.SetRandomTarget();
            timerIndex = citizenAI.GetRandomFloat(10, 50);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
