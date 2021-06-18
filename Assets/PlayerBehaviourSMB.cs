using System.Collections.Generic;
using Gamekit3D;
using GameplayTag.Authoring;
using UnityEngine;

public class PlayerBehaviourSMB : SceneLinkedSMB<PlayerBehaviour>
{
    [SerializeField] int index;
    [SerializeField] AnimationTagScriptableObject State;


    public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!m_MonoBehaviour) return;
        m_MonoBehaviour.SetNextState(State, index);
    }

    public override void OnSLStatePostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!m_MonoBehaviour) return;
        m_MonoBehaviour.SetCurrentState(State, index);
        m_MonoBehaviour.SetNextState(null, index);
    }

    public override void OnSLStatePreExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!m_MonoBehaviour) return;
    }


}

