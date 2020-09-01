using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    public class GrenadierSMBRangeAttack : SceneLinkedSMB<GrenadierBehaviour>
    {
        public float growthTime = 2.0f;

        protected float m_GrowthTimer = 0;

        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateEnter(animator, stateInfo, layerIndex);

            m_MonoBehaviour.RememberTargetPosition();
            m_MonoBehaviour.grenadeLauncher.LoadProjectile();

            m_MonoBehaviour.grenadeLauncher.loadedProjectile.transform.up = Vector3.up;
            m_MonoBehaviour.grenadeLauncher.loadedProjectile.transform.forward = m_MonoBehaviour.transform.forward;

            m_GrowthTimer = 0.0f;
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);

            m_GrowthTimer = Mathf.Clamp(m_GrowthTimer + Time.deltaTime, 0.0f, growthTime);
            if (m_MonoBehaviour.grenadeLauncher.loadedProjectile != null)
                m_MonoBehaviour.grenadeLauncher.loadedProjectile.transform.localScale =
                    Vector3.one * (m_GrowthTimer / growthTime);
        }
    }
}