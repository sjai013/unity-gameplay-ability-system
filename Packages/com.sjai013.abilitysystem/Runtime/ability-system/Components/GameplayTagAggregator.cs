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
    public Dictionary<GameplayTagScriptableObject.GameplayTag, List<GameplayEffectSpec>> GameplayEffectSpecs { get; private set; } = new();
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

    public bool TryGetGameplayEffectsForTag(GameplayTagScriptableObject.GameplayTag tag, out List<GameplayEffectSpec> geSpec)
    {
        return GameplayEffectSpecs.TryGetValue(tag, out geSpec);
    }

    void Update()
    {
        if (m_UnusedTags >= m_TotalTags * 2 && m_UnusedTags > 0 && m_TotalTags > 0)
        {
            GameplayEffectSpecs.Clear();
        }

        m_UnusedTags = 0;
        m_TotalTags = 0;

        var appliedGe = m_AbilitySystemCharacter.AppliedGameplayEffects;

        foreach (var item in GameplayEffectSpecs)
        {
            if (item.Value.Count == 0)
            {
                m_UnusedTags++;
            }
            item.Value.Clear();
        }


        for (var i = 0; i < appliedGe.Count; i++)
        {
            var grantedTags = appliedGe[i].GameplayEffect.GetGameplayEffectTags().GrantedTags;
            var assetTag = appliedGe[i].GameplayEffect.GetGameplayEffectTags().AssetTag;

            AddOrUpdateTags(appliedGe[i], grantedTags);
            AddOrUpdateTag(appliedGe[i], assetTag);
        }

    }

    private void AddOrUpdateTags(GameplayEffectSpec geSpec, GameplayTagScriptableObject.GameplayTag[] tags)
    {
        for (var iTag = 0; iTag < tags.Length; iTag++)
        {
            AddOrUpdateTag(geSpec, tags[iTag]);
            m_TotalTags++;
        }
    }

    private void AddOrUpdateTag(GameplayEffectSpec geSpec, GameplayTagScriptableObject.GameplayTag tag)
    {
        if (!GameplayEffectSpecs.TryGetValue(tag, out var cachedSpec))
        {
            cachedSpec = new();
            GameplayEffectSpecs[tag] = cachedSpec;
        }

        cachedSpec.Add(geSpec);
    }

}
