using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem;
using GameplayTag.Authoring;
using UnityEngine;

[RequireComponent(typeof(AbilitySystemCharacter))]
public class GameplayTagAggregator : MonoBehaviour
{
    public AbilitySystemCharacter m_AbilitySystemCharacter { get; private set; }
    public Dictionary<GameplayTagScriptableObject.GameplayTag, List<GameplayEffectContainer>> GameplayEffectContainers { get; private set; } = new Dictionary<GameplayTagScriptableObject.GameplayTag, List<GameplayEffectContainer>>();
    void Start()
    {
        m_AbilitySystemCharacter = GetComponent<AbilitySystemCharacter>();
    }

    // This is used to keep a reference to tags that have a count of 0.
    // If we create a new list every frame, we create a lot of GC
    // By clearing lists instead, we avoid the GC, but still have to clean
    // up cases where the list size is 0 (and so unused).
    private int m_UnusedTags;
    private int m_TotalTags;

    public bool TryGetGameplayEffectsForTag(GameplayTagScriptableObject.GameplayTag tag, out List<GameplayEffectContainer> gameplayEffectContainers)
    {
        return GameplayEffectContainers.TryGetValue(tag, out gameplayEffectContainers);
    }

    void Update()
    {
        if (m_UnusedTags >= m_TotalTags*2 && m_UnusedTags > 0 && m_TotalTags > 0)
        {
            GameplayEffectContainers.Clear();
        }

        m_UnusedTags = 0;
        m_TotalTags = 0;

        var appliedGe = m_AbilitySystemCharacter.AppliedGameplayEffects;

        foreach (var item in GameplayEffectContainers)
        {
            if (item.Value.Count == 0)
            {
                m_UnusedTags++;
            }
            item.Value.Clear();
        }


        for (var i = 0; i < appliedGe.Count; i++)
        {
            var tags = appliedGe[i].spec.GameplayEffect.GetGameplayEffectTags().GrantedTags;
            AddOrUpdateTags(appliedGe[i], tags);
        }

    }

    private void AddOrUpdateTags(GameplayEffectContainer geContainer, GameplayTagScriptableObject.GameplayTag[] tags)
    {
        for (var iTag = 0; iTag < tags.Length; iTag++)
        {
            if (!GameplayEffectContainers.TryGetValue(tags[iTag], out var geContainers))
            {
                GameplayEffectContainers[tags[iTag]] = new List<GameplayEffectContainer>();
            }

            GameplayEffectContainers[tags[iTag]].Add(geContainer);
            m_TotalTags++;
        }
    }

}
