using System.Collections;
using System.Collections.Generic;
using GameplayTag.Authoring;
using UnityEngine;

namespace AbilitySystem.Authoring
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Gameplay Effect Definition")]
    public class GameplayEffectScriptableObject : ScriptableObject
    {
        [SerializeField]
        public GameplayEffectDefinitionContainer gameplayEffect;

        [SerializeField] private TGameplayEffectTags<GameplayTagScriptableObject> gameplayEffectTagsAuthoring;
        [SerializeField] public TGameplayEffectTags<GameplayTagScriptableObject.GameplayTag> gameplayEffectTags;

        [SerializeField]
        public GameplayEffectPeriod Period;

        public void OnValidate()
        {
            gameplayEffectTags = ConvertTags();
        }


        private TGameplayEffectTags<GameplayTagScriptableObject.GameplayTag> ConvertTags()
        {
            return new TGameplayEffectTags<GameplayTagScriptableObject.GameplayTag>()
            {
                AssetTag = gameplayEffectTagsAuthoring.AssetTag?.TagData ?? new GameplayTagScriptableObject.GameplayTag(),
                ApplicationTagRequirements = new GameplayTagRequireIgnoreContainer<GameplayTagScriptableObject.GameplayTag>()
                {
                    IgnoreTags = gameplayEffectTagsAuthoring.ApplicationTagRequirements.IgnoreTags?.ToGameplayTagStruct(),
                    RequireTags = gameplayEffectTagsAuthoring.ApplicationTagRequirements.RequireTags?.ToGameplayTagStruct()
                },
                GrantedTags = gameplayEffectTagsAuthoring.GrantedTags?.ToGameplayTagStruct(),
                OngoingTagRequirements = new GameplayTagRequireIgnoreContainer<GameplayTagScriptableObject.GameplayTag>()
                {
                    IgnoreTags = gameplayEffectTagsAuthoring.OngoingTagRequirements.IgnoreTags?.ToGameplayTagStruct(),
                    RequireTags = gameplayEffectTagsAuthoring.OngoingTagRequirements.RequireTags?.ToGameplayTagStruct()
                },
                RemovalTagRequirements = new GameplayTagRequireIgnoreContainer<GameplayTagScriptableObject.GameplayTag>()
                {
                    IgnoreTags = gameplayEffectTagsAuthoring.RemovalTagRequirements.IgnoreTags?.ToGameplayTagStruct(),
                    RequireTags = gameplayEffectTagsAuthoring.RemovalTagRequirements.RequireTags?.ToGameplayTagStruct()
                },
                RemoveGameplayEffectsWithTag = gameplayEffectTagsAuthoring.RemoveGameplayEffectsWithTag?.ToGameplayTagStruct()
            };
        }
    }

}
