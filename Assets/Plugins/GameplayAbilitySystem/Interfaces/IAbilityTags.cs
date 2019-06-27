using System.Collections.Generic;
using GameplayAbilitySystem.GameplayEffects;

namespace GameplayAbilitySystem.Interfaces {
    public interface IAbilityTags {
        /// <summary>
        /// Any additional tags that the ability has
        /// </summary>
        GameplayEffectTagContainer AbilityTags { get; }

        /// <summary>
        /// Abilities with these tags are cancelled
        /// </summary>
        GameplayEffectTagContainer CancelAbilitiesWithTags { get; }

        /// <summary>
        /// Abilities with these tags are blocked
        /// </summary>
        GameplayEffectTagContainer BlockAbilitiesWithTags { get; }

        /// <summary>
        /// Tags to apply to activating owner while this ability is active
        /// </summary>
        GameplayEffectTagContainer ActivationOwnedTags { get; }

        /// <summary>
        /// Ability can only be activated if the activating object has all of these tags
        /// </summary>
        GameplayEffectTagContainer ActivationRequiredTags { get; }

        /// <summary>
        /// Ability is blocked if activating object has any of these tags
        /// </summary>
        GameplayEffectTagContainer ActivationBlockedTags { get; }

        /// <summary>
        /// Ability can only be activated if source object has all of these tags
        /// </summary>
        GameplayEffectTagContainer SourceRequiredTags { get; }

        /// <summary>
        /// Ability is blocked if source object has any of these tags
        /// </summary>
        GameplayEffectTagContainer SourceBlockedTags { get; }

        /// <summary>
        /// Ability can only be activated if target object has all of these tags
        /// </summary>
        GameplayEffectTagContainer TargetRequiredTags { get; }

        /// <summary>
        /// Ability is blocked if target object has any of these tags
        /// </summary>
        GameplayEffectTagContainer TargetBlockedTags { get; }
    }
}
