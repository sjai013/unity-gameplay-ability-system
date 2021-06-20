using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem;
using Gamekit3D;
using GameplayTag.Authoring;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour, IGameplayTagProvider
{
    [SerializeField] AbilitySystemCharacter abilitySystemCharacter;
    [SerializeField] Animator animator;
    Dictionary<GameplayTagScriptableObject, AnimationTagScriptableObject[]> m_AnimationTags = new Dictionary<GameplayTagScriptableObject, AnimationTagScriptableObject[]>();


    // Start is called before the first frame update
    void Start()
    {
        SceneLinkedSMB<PlayerBehaviour>.Initialise(animator, this);
        abilitySystemCharacter.RegisterTagSource(this);
    }

#if UNITY_EDITOR
    [SerializeField] List<AnimationTagStruct> animationTags = new List<AnimationTagStruct>();

    [Serializable]
    public struct AnimationTagStruct
    {
        public GameplayTagScriptableObject key;
        public AnimationTagScriptableObject[] animationTags;
    }
    void Update()
    {
        animationTags = new List<AnimationTagStruct>();
        foreach (var item in m_AnimationTags)
        {
            animationTags.Add(new AnimationTagStruct
            {
                key = item.Key,
                animationTags = item.Value
            });
        }
    }
#endif

    public void SetAnimationTags(AnimationTagScriptableObject[] animationTags, GameplayTagScriptableObject idTag)
    {
        this.m_AnimationTags[idTag] = animationTags;
    }

    public AnimationTagScriptableObject[] GetState(GameplayTagScriptableObject idTag)
    {
        return this.m_AnimationTags[idTag];
    }

    public List<GameplayTagScriptableObject> ListTags()
    {
        var gameplayTags = new List<GameplayTagScriptableObject>();
        foreach (var item in m_AnimationTags)
        {
            if (item.Value == null) continue;
            gameplayTags.AddRange(item.Value);
        }

        return gameplayTags;

    }
}
