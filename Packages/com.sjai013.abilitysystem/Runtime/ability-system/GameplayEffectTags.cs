using System;
using GameplayTag.Authoring;
using UnityEngine;

namespace AbilitySystem
{
    [Serializable]
    public struct TGameplayEffectTags<T>
    {
        /// <summary>
        /// The tag that defines this gameplay effect
        /// </summary>
        [SerializeField] public T AssetTag;

        /// <summary>
        /// The tags this GE grants to the ability system character
        /// </summary>
        [SerializeField] public T[] GrantedTags;

        /// <summary>
        /// These tags determine if the GE is considered 'on' or 'off'
        /// </summary>
        [SerializeField] public GameplayTagRequireIgnoreContainer<T> OngoingTagRequirements;

        /// <summary>
        /// These tags must be present for this GE to be applied
        /// </summary>
        [SerializeField] public GameplayTagRequireIgnoreContainer<T> ApplicationTagRequirements;

        /// <summary>
        /// Tag requirements that will remove this GE
        /// </summary>
        [SerializeField] public GameplayTagRequireIgnoreContainer<T> RemovalTagRequirements;

        /// <summary>
        /// Remove GE that match these tags
        /// </summary>
        [SerializeField] public T[] RemoveGameplayEffectsWithTag;
    }

}
