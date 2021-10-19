using System.Collections;
using System.Collections.Generic;
using GameplayTag.Authoring;
using UnityEngine;

namespace AbilitySystem.Authoring
{
    [AddComponentMenu("Gameplay Ability System/Gameplay Effect")]
    public class GameplayEffect : MonoBehaviour
    {
        [SerializeField]
        public GameplayEffectDefinitionContainer gameplayEffect;

        [SerializeField] private TGameplayEffectTags<GameplayTagScriptableObject> m_GameplayEffectTagsAuthoring;
        [HideInInspector] private TGameplayEffectTags<GameplayTagScriptableObject.GameplayTag> m_GameplayEffectTags;
        [SerializeField] private GameplayCueScriptableObject[] m_GameplayCue;
        [SerializeField] private GameplayEffectPeriod m_Period;

        public GameplayCueScriptableObject[] GetGameplayCues()
        {
            return this.m_GameplayCue;
        }

        public TGameplayEffectTags<GameplayTagScriptableObject.GameplayTag> GetGameplayEffectTags()
        {
            return m_GameplayEffectTags;
        }

        public GameplayEffectPeriod GetPeriod()
        {
            return m_Period;
        }

        public void OnValidate()
        {
            m_GameplayEffectTags = ConvertTags();
        }

        public TGameplayEffectTags<GameplayTagScriptableObject> GetGameplayTagsAuthoring()
        {
            return this.m_GameplayEffectTagsAuthoring;
        }

        private TGameplayEffectTags<GameplayTagScriptableObject.GameplayTag> ConvertTags()
        {
            return new TGameplayEffectTags<GameplayTagScriptableObject.GameplayTag>()
            {
                AssetTag = m_GameplayEffectTagsAuthoring.AssetTag?.TagData ?? new GameplayTagScriptableObject.GameplayTag(),
                ApplicationTagRequirements = new GameplayTagRequireIgnoreContainer<GameplayTagScriptableObject.GameplayTag>()
                {
                    IgnoreTags = m_GameplayEffectTagsAuthoring.ApplicationTagRequirements.IgnoreTags?.ToGameplayTagStruct(),
                    RequireTags = m_GameplayEffectTagsAuthoring.ApplicationTagRequirements.RequireTags?.ToGameplayTagStruct()
                },
                GrantedTags = m_GameplayEffectTagsAuthoring.GrantedTags?.ToGameplayTagStruct(),
                OngoingTagRequirements = new GameplayTagRequireIgnoreContainer<GameplayTagScriptableObject.GameplayTag>()
                {
                    IgnoreTags = m_GameplayEffectTagsAuthoring.OngoingTagRequirements.IgnoreTags?.ToGameplayTagStruct(),
                    RequireTags = m_GameplayEffectTagsAuthoring.OngoingTagRequirements.RequireTags?.ToGameplayTagStruct()
                },
                RemovalTagRequirements = new GameplayTagRequireIgnoreContainer<GameplayTagScriptableObject.GameplayTag>()
                {
                    IgnoreTags = m_GameplayEffectTagsAuthoring.RemovalTagRequirements.IgnoreTags?.ToGameplayTagStruct(),
                    RequireTags = m_GameplayEffectTagsAuthoring.RemovalTagRequirements.RequireTags?.ToGameplayTagStruct()
                },
                RemoveGameplayEffectsWithTag = m_GameplayEffectTagsAuthoring.RemoveGameplayEffectsWithTag?.ToGameplayTagStruct()
            };
        }
    }

}
