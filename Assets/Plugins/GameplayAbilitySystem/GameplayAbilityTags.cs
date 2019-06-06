using System;
using System.Collections.Generic;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;

namespace GameplayAbilitySystem
{
    /// <inheritdoc />
    [Serializable]
    public class GameplayAbilityTags : IAbilityTags
    {
        /// <inheritdoc />
        public List<GameplayTag> AbilityTags => _abilityTags;
        /// <inheritdoc />
        public List<GameplayTag> CancelAbilitiesWithTags => _cancelAbilitiesWithTags;
        /// <inheritdoc />
        public List<GameplayTag> BlockAbilitiesWithTags => _blockAbilitiesWithTags;
        /// <inheritdoc />
        public List<GameplayTag> ActivationOwnedTags => _activationOwnedTags;
        /// <inheritdoc />
        public List<GameplayTag> ActivationRequiredTags => _activationRequiredTags;
        /// <inheritdoc />
        public List<GameplayTag> ActivationBlockedTags => _activationBlockedTags;
        /// <inheritdoc />
        public List<GameplayTag> SourceRequiredTags => _sourceRequiredTags;
        /// <inheritdoc />
        public List<GameplayTag> SourceBlockedTags => _sourceBlockedTags;
        /// <inheritdoc />
        public List<GameplayTag> TargetRequiredTags => _targetRequiredTags;
        /// <inheritdoc />
        public List<GameplayTag> TargetBlockedTags => _targetBlockedTags;


        [Tooltip("Tags for this ability")]
        [SerializeField]
        protected List<GameplayTag> _abilityTags;


        [Tooltip("Active abilities on player with this AbilitySystem which have these tags are cancelled")]
        [SerializeField]
        protected List<GameplayTag> _cancelAbilitiesWithTags;


        [Tooltip("Tags for this ability")]
        [SerializeField]
        protected List<GameplayTag> _blockAbilitiesWithTags;

        /// <summary>
        /// Tags to apply to activating owner while this ability is active
        /// </summary>
        [Tooltip("Tags to apply to activating owner while this ability is active")]
        [SerializeField]
        protected List<GameplayTag> _activationOwnedTags;

   
        [Tooltip("Ability can only be activated if the activating object has all of these tags")]
        [SerializeField]
        protected List<GameplayTag> _activationRequiredTags;


        [Tooltip("Ability is blocked if activating object has any of these tags")]
        [SerializeField]
        protected List<GameplayTag> _activationBlockedTags;

        [Tooltip("Ability can only be activated if source object has all of these tags")]
        [SerializeField]
        protected List<GameplayTag> _sourceRequiredTags;

        [Tooltip("Ability can only be activated if source object has all of these tags")]
        [SerializeField]
        protected List<GameplayTag> _sourceBlockedTags;

        [Tooltip("Ability can only be activated if source object has all of these tags")]
        [SerializeField]
        protected List<GameplayTag> _targetRequiredTags;

        [Tooltip("Ability can only be activated if source object has all of these tags")]
        [SerializeField]
        protected List<GameplayTag> _targetBlockedTags;
    }
}
