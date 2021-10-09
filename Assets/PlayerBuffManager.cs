using System;
using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Authoring;
using GameplayTag.Authoring;
using UnityEngine;

public class PlayerBuffManager : MonoBehaviour
{
    [SerializeField] private GameplayTagAggregator m_GameplayTagAggregator;
    [SerializeField] private GameplayEffectIconAssetDatabase m_AssetDatabase;
    [SerializeField] private UIBuffElement[] m_BuffElements;

    bool GetHighestDurationRemaining(List<GameplayEffectSpec> geContainers, out float durationPercentRemaining)
    {
        var durationRemaining = 0f;
        var totalDuration = -1f;
        for (var i = 0; i < geContainers.Count; i++)
        {
            var spec = geContainers[i];
            if (!spec.IsActive) continue;
            if (spec.TotalDuration > totalDuration)
            {
                durationRemaining = spec.DurationRemaining;
                totalDuration = spec.TotalDuration;
            }
        }
        durationPercentRemaining = durationRemaining / totalDuration;
        return totalDuration >= 0f;

    }
    // Update is called once per frame
    void Update()
    {
        var tagsToSearch = m_AssetDatabase.ListIconMap();

        int buffPointer = 0;
        for (var i = 0; i < tagsToSearch.Length; i++)
        {

            // Check if this tag exists on the player
            if (m_GameplayTagAggregator.TryGetGameplayEffectsForTag(tagsToSearch[i].Tag.TagData, out var geContainers) && geContainers.Count > 0)
            {
                if (GetHighestDurationRemaining(geContainers, out var durationRemaining))
                {
                    var sprite = tagsToSearch[i].Icon;
                    m_BuffElements[buffPointer].Initialise(sprite, durationRemaining);
                    m_BuffElements[buffPointer].gameObject.SetActive(true);
                    buffPointer++;
                }
            }
        }

        for (var i = buffPointer; i < m_BuffElements.Length; i++)
        {
            m_BuffElements[i].gameObject.SetActive(false);
        }
    }

}
