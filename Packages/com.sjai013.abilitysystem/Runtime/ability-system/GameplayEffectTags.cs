using System;
using GameplayTag.Authoring;
using UnityEngine;

namespace AbilitySystem
{
    [Serializable]
    public struct GameplayEffectTags
    {
        /// <summary>
        /// The tag that defines this gameplay effect
        /// </summary>
        [SerializeField] public GameplayTagScriptableObject AssetTag;

        /// <summary>
        /// The tags this GE grants to the ability system character
        /// </summary>
        [SerializeField] public GameplayTagScriptableObject[] GrantedTags;

        /// <summary>
        /// These tags determine if the GE is considered 'on' or 'off'
        /// </summary>
        [SerializeField] public GameplayTagRequireIgnoreContainer OngoingTagRequirements;

        /// <summary>
        /// These tags must be present for this GE to be applied
        /// </summary>
        [SerializeField] public GameplayTagRequireIgnoreContainer ApplicationTagRequirements;

        /// <summary>
        /// Tag requirements that will remove this GE
        /// </summary>
        [SerializeField] public GameplayTagRequireIgnoreContainer RemovalTagRequirements;

        /// <summary>
        /// Remove GE that match these tags
        /// </summary>
        [SerializeField] public GameplayTagScriptableObject[] RemoveGameplayEffectsWithTag;
    }

}
