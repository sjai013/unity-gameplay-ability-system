using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    public class GrenadierSMBTurning : SceneLinkedSMB<GrenadierBehaviour>
    {
        protected Vector3 originalForward;

        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.controller.applyAnimationRotation = true;
            originalForward = m_MonoBehaviour.transform.forward;

            base.OnSLStateEnter(animator, stateInfo, layerIndex);
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);

            Vector3 v = m_MonoBehaviour.target.transform.position - m_MonoBehaviour.transform.position;
            v.y = 0;

            float angle = Vector3.SignedAngle(originalForward, v, Vector3.up);

            animator.SetFloat(GrenadierBehaviour.hashTurnAngleParam, angle / 180.0f);
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.controller.applyAnimationRotation = true;

            base.OnSLStateExit(animator, stateInfo, layerIndex);
        }
    }
}