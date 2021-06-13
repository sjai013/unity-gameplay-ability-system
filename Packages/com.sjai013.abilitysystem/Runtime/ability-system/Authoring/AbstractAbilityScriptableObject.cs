using System;
using GameplayTag.Authoring;
using UnityEngine;
using UnityEngine.Serialization;

namespace AbilitySystem.Authoring
{

    public abstract class AbstractAbilityScriptableObject : ScriptableObject
    {
        /// <summary>
        /// Name of this ability
        /// </summary>
        [SerializeField] private string AbilityName;

        /// <summary>
        /// Tags for this ability
        /// </summary>
        [SerializeField] private TAbilityTags<GameplayTagScriptableObject> AbilityTagsAuthoring;
        public TAbilityTags<GameplayTagScriptableObject.GameplayTag> AbilityTags;

        /// <summary>
        /// The GameplayEffect that defines the cost associated with activating the ability
        /// </summary>
        /// <param name="owner">Usually the character activating this ability</param>
        /// <returns></returns>
        [SerializeField] public GameplayEffectScriptableObject Cost;

        /// <summary>
        /// The GameplayEffect that defines the cooldown associated with this ability
        /// </summary>
        /// <param name="owner">Usually the character activating this ability</param>
        /// <returns></returns>
        [SerializeField] public GameplayEffectScriptableObject Cooldown;

        /// <summary>
        /// Creates the Ability Spec (the instantiation of the ability)
        /// </summary>
        /// <param name="owner">Usually the character casting thsi ability</param>
        /// <returns>Ability Spec</returns>
        public abstract AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner);

        public void OnValidate()
        {
            this.AbilityTags = ConvertTags();
        }

        private TAbilityTags<GameplayTagScriptableObject.GameplayTag> ConvertTags()
        {
            return new TAbilityTags<GameplayTagScriptableObject.GameplayTag>()
            {
                AssetTag = AbilityTagsAuthoring.AssetTag?.TagData ?? new GameplayTagScriptableObject.GameplayTag(),
                ActivationOwnedTags = AbilityTagsAuthoring.ActivationOwnedTags.ToGameplayTagStruct(),
                BlockAbilitiesWithTags = AbilityTagsAuthoring.BlockAbilitiesWithTags.ToGameplayTagStruct(),
                CancelAbilitiesWithTags = AbilityTagsAuthoring.CancelAbilitiesWithTags.ToGameplayTagStruct(),
                OwnerTags = new GameplayTagRequireIgnoreContainer<GameplayTagScriptableObject.GameplayTag>()
                {
                    IgnoreTags = AbilityTagsAuthoring.OwnerTags.IgnoreTags.ToGameplayTagStruct(),
                    RequireTags = AbilityTagsAuthoring.OwnerTags.RequireTags.ToGameplayTagStruct()
                },
                SourceTags = new GameplayTagRequireIgnoreContainer<GameplayTagScriptableObject.GameplayTag>()
                {
                    IgnoreTags = AbilityTagsAuthoring.SourceTags.IgnoreTags.ToGameplayTagStruct(),
                    RequireTags = AbilityTagsAuthoring.SourceTags.RequireTags.ToGameplayTagStruct()
                },
                TargetTags = new GameplayTagRequireIgnoreContainer<GameplayTagScriptableObject.GameplayTag>()
                {
                    IgnoreTags = AbilityTagsAuthoring.TargetTags.IgnoreTags.ToGameplayTagStruct(),
                    RequireTags = AbilityTagsAuthoring.TargetTags.RequireTags.ToGameplayTagStruct()
                },
            };
        }

        private void GameplayTagScriptableObjectToGameplayTag()
        {

        }
    }
}