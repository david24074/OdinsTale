using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenBuildBehaviour : StateMachineBehaviour
{
    private ConstructionBuilding targetBuilding;
    private float mineInterval = 1, currentTimer;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        targetBuilding = animator.GetComponent<Citizen>().GetCurrentTarget().GetComponent<ConstructionBuilding>();
        currentTimer = mineInterval;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!targetBuilding)
        {
            animator.GetComponent<Citizen>().QuitJob();
        }

        currentTimer -= 1 * Time.deltaTime;

        if (currentTimer <= 0)
        {
            currentTimer = mineInterval;
            if (targetBuilding) { targetBuilding.BuildObject(1); }
        }
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
