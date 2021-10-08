using System;
using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Authoring;
using GameplayTag.Authoring;
using UnityEngine;

public class PlayerBuffManager : MonoBehaviour
{
    [SerializeField] private AbilitySystemCharacter m_AbilitySystemCharacter;
    [SerializeField] private GameplayEffectIconAssetDatabase m_AssetDatabase;
    [SerializeField] Dictionary<GameplayTagScriptableObject.GameplayTag, (GameplayEffectContainer geContainer, Sprite sprite)> m_ActiveBuffs = new Dictionary<GameplayTagScriptableObject.GameplayTag, (GameplayEffectContainer geContainer, Sprite sprite)>();

    [SerializeField] private UIBuffElement[] m_BuffElements;

    // Update is called once per frame
    void Update()
    {
        m_ActiveBuffs.Clear();

        for (var i = 0; i < m_AbilitySystemCharacter.AppliedGameplayEffects.Count; i++)
        {
            var tags = m_AbilitySystemCharacter.AppliedGameplayEffects[i].spec.GameplayEffect.GetGameplayEffectTags().GrantedTags;
            for (var nTag = 0; nTag < tags.Length; nTag++)
            {
                TryAddBuffToList(tags[nTag], m_AbilitySystemCharacter.AppliedGameplayEffects[i]);
            }
        }

        // Enable and set up the buff display for active buffs
        int buffPointer = 0;
        foreach (var item in m_ActiveBuffs)
        {
            if (buffPointer >= m_BuffElements.Length) break;
            var sprite = item.Value.sprite;
            var spec = item.Value.geContainer.spec;
            var durationRemaining = spec.DurationRemaining / spec.TotalDuration;
            m_BuffElements[buffPointer].Initialise(item.Value.sprite, durationRemaining);
            m_BuffElements[buffPointer].gameObject.SetActive(true);
            buffPointer++;
        }

        for (var i = buffPointer; i < m_BuffElements.Length; i++)
        {
            m_BuffElements[i].gameObject.SetActive(false);
        }
    }

    bool TryAddBuffToList(GameplayTagScriptableObject.GameplayTag tag, GameplayEffectContainer geContainer)
    {
        // Check if the passed in tags need to be handled
        if (!(m_AssetDatabase.HasIcon(tag, out var sprite)))
        {
            return false;
        }

        // If this tag is already in list of buffs to display,
        // compare it to the one that's being added - the one that
        // should remain is the one with the longer time remaining
        if (m_ActiveBuffs.TryGetValue(tag, out var buff))
        {
            if (geContainer.spec.DurationRemaining > buff.geContainer.spec.DurationRemaining)
            {
                m_ActiveBuffs[tag] = (geContainer, sprite);
            }
        }
        else
        {
            m_ActiveBuffs[tag] = (geContainer, sprite);
        }

        return true;
    }




}
