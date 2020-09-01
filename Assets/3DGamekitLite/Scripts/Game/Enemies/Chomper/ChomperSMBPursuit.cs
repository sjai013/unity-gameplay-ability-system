using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Gamekit3D
{
    public class ChomperSMBPursuit : SceneLinkedSMB<ChomperBehavior>
    {
        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);

            m_MonoBehaviour.FindTarget();

            if (m_MonoBehaviour.controller.navmeshAgent.pathStatus == NavMeshPathStatus.PathPartial 
                || m_MonoBehaviour.controller.navmeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                m_MonoBehaviour.StopPursuit();
                return;
            }

            if (m_MonoBehaviour.target == null || m_MonoBehaviour.target.respawning)
            {//if the target was lost or is respawning, we stop the pursit
                m_MonoBehaviour.StopPursuit();
            }
            else
            {
                m_MonoBehaviour.RequestTargetPosition();

                Vector3 toTarget = m_MonoBehaviour.target.transform.position - m_MonoBehaviour.transform.position;

                if (toTarget.sqrMagnitude < m_MonoBehaviour.attackDistance * m_MonoBehaviour.attackDistance)
                {
                    m_MonoBehaviour.TriggerAttack();
                }
                else if (m_MonoBehaviour.followerData.assignedSlot != -1)
                {
                    Vector3 targetPoint = m_MonoBehaviour.target.transform.position + 
                        m_MonoBehaviour.followerData.distributor.GetDirection(m_MonoBehaviour.followerData
                            .assignedSlot) * m_MonoBehaviour.attackDistance * 0.9f;

                    m_MonoBehaviour.controller.SetTarget(targetPoint);
                }
                else
                {
                    m_MonoBehaviour.StopPursuit();
                }
            }
        }
    }
}