using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTriggers : StateMachineBehaviour {
    public bool ResetOnEnter;
    public bool ResetOnExit;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!ResetOnEnter) return;
        ResetAllTriggers(animator);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!ResetOnExit) return;
        ResetAllTriggers(animator);
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

    void ResetAllTriggers(Animator animator) {
        var parameters = animator.parameters;
        for (var i = 0; i < animator.parameterCount; i++) {
            if (parameters[i].type == AnimatorControllerParameterType.Trigger) {
                animator.ResetTrigger(parameters[i].name);
            }
        }
    }
}
