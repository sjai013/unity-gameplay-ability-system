using UnityEngine;

namespace Gamekit3D
{
    public class RandomStateSMB : StateMachineBehaviour
    {
        public int numberOfStates = 3;
        public float minNormTime = 0f;
        public float maxNormTime = 5f;

        protected float m_RandomNormTime;

        readonly int m_HashRandomIdle = Animator.StringToHash("RandomIdle");

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Randomly decide a time at which to transition.
            m_RandomNormTime = Random.Range(minNormTime, maxNormTime);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // If trainsitioning away from this state reset the random idle parameter to -1.
            if (animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).fullPathHash == stateInfo.fullPathHash)
            {
                animator.SetInteger(m_HashRandomIdle, -1);
            }

            // If the state is beyond the randomly decided normalised time and not yet transitioning then set a random idle.
            if (stateInfo.normalizedTime > m_RandomNormTime && !animator.IsInTransition(0))
            {
                animator.SetInteger(m_HashRandomIdle, Random.Range(0, numberOfStates));
            }
        }
    } 
}
