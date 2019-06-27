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
        public GameplayEffectTagContainer AbilityTags => _abilityTags;
        public GameplayEffectTagContainer CooldownTags => _cooldownTags;
        /// <inheritdoc />
        public GameplayEffectTagContainer CancelAbilitiesWithTags => _cancelAbilitiesWithTags;
        /// <inheritdoc />
        public GameplayEffectTagContainer BlockAbilitiesWithTags => _blockAbilitiesWithTags;
        /// <inheritdoc />
        public GameplayEffectTagContainer ActivationOwnedTags => _activationOwnedTags;
        /// <inheritdoc />
        public GameplayEffectTagContainer ActivationRequiredTags => _activationRequiredTags;
        /// <inheritdoc />
        public GameplayEffectTagContainer ActivationBlockedTags => _activationBlockedTags;
        /// <inheritdoc />
        public GameplayEffectTagContainer SourceRequiredTags => _sourceRequiredTags;
        /// <inheritdoc />
        public GameplayEffectTagContainer SourceBlockedTags => _sourceBlockedTags;
        /// <inheritdoc />
        public GameplayEffectTagContainer TargetRequiredTags => _targetRequiredTags;
        /// <inheritdoc />
        public GameplayEffectTagContainer TargetBlockedTags => _targetBlockedTags;


        [Tooltip("Tags for this ability")]
        [SerializeField]
        protected GameplayEffectTagContainer _abilityTags;

        [Tooltip("Tags to determine whether the ability is on cooldown")]
        [SerializeField]
        protected GameplayEffectTagContainer _cooldownTags;

        [Tooltip("Active abilities on player with this AbilitySystem which have these tags are cancelled")]
        [SerializeField]
        protected GameplayEffectTagContainer _cancelAbilitiesWithTags;


        [Tooltip("Abilities with these tags will be blocked")]
        [SerializeField]
        protected GameplayEffectTagContainer _blockAbilitiesWithTags;

        /// <summary>
        /// Tags to apply to activating owner while this ability is active
        /// </summary>
        [Tooltip("Tags to apply to activating owner while this ability is active")]
        [SerializeField]
        protected GameplayEffectTagContainer _activationOwnedTags;


        [Tooltip("Ability can only be activated if the activating object has all of these tags")]
        [SerializeField]
        protected GameplayEffectTagContainer _activationRequiredTags;


        [Tooltip("Ability is blocked if activating object has any of these tags")]
        [SerializeField]
        protected GameplayEffectTagContainer _activationBlockedTags;

        [Tooltip("Ability can only be activated if source object has all of these tags")]
        [SerializeField]
        protected GameplayEffectTagContainer _sourceRequiredTags;

        [Tooltip("Ability can only be activated if source object has all of these tags")]
        [SerializeField]
        protected GameplayEffectTagContainer _sourceBlockedTags;

        [Tooltip("Ability can only be activated if source object has all of these tags")]
        [SerializeField]
        protected GameplayEffectTagContainer _targetRequiredTags;

        [Tooltip("Ability can only be activated if source object has all of these tags")]
        [SerializeField]
        protected GameplayEffectTagContainer _targetBlockedTags;
        
    }
}
