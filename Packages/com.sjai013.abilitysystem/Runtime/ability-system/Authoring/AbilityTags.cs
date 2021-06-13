using UnityEngine;
using GameplayTag.Authoring;
using System;
using UnityEngine.Serialization;

namespace AbilitySystem.Authoring
{
    public static class TAbilityTagsExtensions
    {
        public static GameplayTagScriptableObject.GameplayTag[] ToGameplayTagStruct(this GameplayTagScriptableObject[] tags)
        {
            var _tags = new GameplayTagScriptableObject.GameplayTag[tags.Length];
            for (var i = 0; i < _tags.Length; i++)
            {
                if (tags[i] == null) continue;
                _tags[i] = tags[i].TagData;
            }
            return _tags;
        }
    }

    [Serializable]
    public struct TAbilityTags<T>
    {
        /// <summary>
        /// This tag describes the Gameplay Ability
        /// </summary>
        [SerializeField] public T AssetTag;

        /// <summary>
        /// Active Gameplay Abilities (on the same character) that have these tags will be cancelled
        /// </summary>
        [SerializeField] public T[] CancelAbilitiesWithTags;

        /// <summary>
        /// Gameplay Abilities that have these tags will be blocked from activating on the same character
        /// </summary>
        [SerializeField] public T[] BlockAbilitiesWithTags;

        /// <summary>
        /// These tags are granted to the character while the ability is active
        /// </summary>
        [SerializeField] public T[] ActivationOwnedTags;

        /// <summary>
        /// This ability can only be activated if the owner character has all of the Required tags
        /// and none of the Ignore tags.  Usually, the owner is the source as well.
        /// </summary>
        [SerializeField] public GameplayTagRequireIgnoreContainer<T> OwnerTags;

        /// <summary>
        /// This ability can only be activated if the source character has all of the Required tags
        /// and none of the Ignore tags
        /// </summary>
        [SerializeField] public GameplayTagRequireIgnoreContainer<T> SourceTags;

        /// <summary>
        /// This ability can only be activated if the target character has all of the Required tags
        /// and none of the Ignore tags
        /// </summary>
        [SerializeField] public GameplayTagRequireIgnoreContainer<T> TargetTags;
    }

}