using System;
using System.Collections.Generic;
using GameplayAbilitySystem.GameplayEffects;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;

namespace GameplayAbilitySystem {
    /// <inheritdoc />
    [Serializable]
    public class GameplayAbilityTags : IAbilityTags {
        /// <inheritdoc />
        public GameplayEffectAddRemoveTagContainer AbilityTags => _abilityTags;
        public GameplayEffectAddRemoveTagContainer CooldownTags => _cooldownTags;
        /// <inheritdoc />
        public GameplayEffectAddRemoveTagContainer CancelAbilitiesWithTags => _cancelAbilitiesWithTags;
        /// <inheritdoc />
        public GameplayEffectAddRemoveTagContainer BlockAbilitiesWithTags => _blockAbilitiesWithTags;
        /// <inheritdoc />
        public GameplayEffectAddRemoveTagContainer ActivationOwnedTags => _activationOwnedTags;
        /// <inheritdoc />
        public GameplayEffectAddRemoveTagContainer ActivationRequiredTags => _activationRequiredTags;
        /// <inheritdoc />
        public GameplayEffectAddRemoveTagContainer ActivationBlockedTags => _activationBlockedTags;
        /// <inheritdoc />
        public GameplayEffectAddRemoveTagContainer SourceRequiredTags => _sourceRequiredTags;
        /// <inheritdoc />
        public GameplayEffectAddRemoveTagContainer SourceBlockedTags => _sourceBlockedTags;
        /// <inheritdoc />
        public GameplayEffectAddRemoveTagContainer TargetRequiredTags => _targetRequiredTags;
        /// <inheritdoc />
        public GameplayEffectAddRemoveTagContainer TargetBlockedTags => _targetBlockedTags;


        [Tooltip("Tags for this ability")]
        [SerializeField]
        protected GameplayEffectAddRemoveTagContainer _abilityTags;

        [Tooltip("Tags to determine whether the ability is on cooldown")]
        [SerializeField]
        protected GameplayEffectAddRemoveTagContainer _cooldownTags;

        [Tooltip("Active abilities on player with this AbilitySystem which have these tags are cancelled")]
        [SerializeField]
        protected GameplayEffectAddRemoveTagContainer _cancelAbilitiesWithTags;


        [Tooltip("Abilities with these tags will be blocked")]
        [SerializeField]
        protected GameplayEffectAddRemoveTagContainer _blockAbilitiesWithTags;

        /// <summary>
        /// Tags to apply to activating owner while this ability is active
        /// </summary>
        [Tooltip("Tags to apply to activating owner while this ability is active")]
        [SerializeField]
        protected GameplayEffectAddRemoveTagContainer _activationOwnedTags;


        [Tooltip("Ability can only be activated if the activating object has all of these tags")]
        [SerializeField]
        protected GameplayEffectAddRemoveTagContainer _activationRequiredTags;


        [Tooltip("Ability is blocked if activating object has any of these tags")]
        [SerializeField]
        protected GameplayEffectAddRemoveTagContainer _activationBlockedTags;

        [Tooltip("Ability can only be activated if source object has all of these tags")]
        [SerializeField]
        protected GameplayEffectAddRemoveTagContainer _sourceRequiredTags;

        [Tooltip("Ability is blocked if source object has any of these tags")]
        [SerializeField]
        protected GameplayEffectAddRemoveTagContainer _sourceBlockedTags;

        [Tooltip("Ability can only be activated if source object has all of these tags")]
        [SerializeField]
        protected GameplayEffectAddRemoveTagContainer _targetRequiredTags;

        [Tooltip("Ability is blocked if source object has any of these tags")]
        [SerializeField]
        protected GameplayEffectAddRemoveTagContainer _targetBlockedTags;
        
    }
}
