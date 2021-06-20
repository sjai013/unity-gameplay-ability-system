using System.Collections.Generic;
using Gamekit3D;
using GameplayTag.Authoring;
using UnityEngine;

public class PlayerBehaviourSMB : SceneLinkedSMB<PlayerBehaviour>
{
    [SerializeField] GameplayTagScriptableObject animationId;
    [SerializeField] AnimationTagScriptableObject[] AnimationTags;


    public override void OnSLStatePostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!m_MonoBehaviour) return;
        m_MonoBehaviour.SetAnimationTags(AnimationTags, animationId);
    }


    public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!m_MonoBehaviour) return;
        m_MonoBehaviour.SetAnimationTags(default, animationId);
    }


}

