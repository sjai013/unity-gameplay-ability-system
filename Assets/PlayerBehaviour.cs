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
    public Animator animator;
    Dictionary<GameplayTagScriptableObject, AnimationTagScriptableObject[]> m_AnimationTags = new Dictionary<GameplayTagScriptableObject, AnimationTagScriptableObject[]>();
    private int attackTriggerHash = Animator.StringToHash("Attack");

    public void TriggerAttack()
    {
        animator.SetTrigger(attackTriggerHash);
    }
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

    public bool HasTag(GameplayTagScriptableObject idTag, AnimationTagScriptableObject tagToFind)
    {
        if (this.m_AnimationTags.TryGetValue(idTag, out var animTags))
        {
            if (animTags == null) return false;
            for (var i = 0; i < animTags.Length; i++)
            {
                if (animTags[i] == tagToFind) return true;
            }
        }
        return false;
    }

    public bool HasAllTags(GameplayTagScriptableObject idTag, AnimationTagScriptableObject[] tagToFind)
    {
        if (this.m_AnimationTags.TryGetValue(idTag, out var animTags))
        {
            if (animTags == null) return false;
            for (var i = 0; i < tagToFind.Length; i++)
            {
                bool matchFound = false;
                for (var j = 0; j < animTags.Length; j++)
                {
                    if (animTags[j] == tagToFind[i])
                    {
                        matchFound = true;
                        break;
                    }
                }
                if (!matchFound) return false;
            }
        }
        return false;
    }

    public bool HasAnyTags(GameplayTagScriptableObject idTag, AnimationTagScriptableObject[] tagsToFind)
    {
        for (var i = 0; i < tagsToFind.Length; i++)
        {
            if (HasTag(idTag, tagsToFind[i])) return true;
        }
        return false;
    }

    public bool HasNoneTags(GameplayTagScriptableObject idTag, AnimationTagScriptableObject[] tagsToFind)
    {
        return !HasAnyTags(idTag, tagsToFind);
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
