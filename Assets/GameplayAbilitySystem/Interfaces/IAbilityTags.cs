using System.Collections.Generic;
using GameplayAbilitySystem.GameplayEffects;

namespace GameplayAbilitySystem.Interfaces {
    public interface IAbilityTags {
        /// <summary>
        /// Any additional tags that the ability has
        /// </summary>
        GameplayEffectAddRemoveTagContainer AbilityTags { get; }

        /// <summary>
        /// Abilities with these tags are cancelled
        /// </summary>
        GameplayEffectAddRemoveTagContainer CancelAbilitiesWithTags { get; }

        /// <summary>
        /// Abilities with these tags are blocked
        /// </summary>
        GameplayEffectAddRemoveTagContainer BlockAbilitiesWithTags { get; }

        /// <summary>
        /// Tags to apply to activating owner while this ability is active
        /// </summary>
        GameplayEffectAddRemoveTagContainer ActivationOwnedTags { get; }

        /// <summary>
        /// Ability can only be activated if the activating object has all of these tags
        /// </summary>
        GameplayEffectAddRemoveTagContainer ActivationRequiredTags { get; }

        /// <summary>
        /// Ability is blocked if activating object has any of these tags
        /// </summary>
        GameplayEffectAddRemoveTagContainer ActivationBlockedTags { get; }

        /// <summary>
        /// Ability can only be activated if source object has all of these tags
        /// </summary>
        GameplayEffectAddRemoveTagContainer SourceRequiredTags { get; }

        /// <summary>
        /// Ability is blocked if source object has any of these tags
        /// </summary>
        GameplayEffectAddRemoveTagContainer SourceBlockedTags { get; }

        /// <summary>
        /// Ability can only be activated if target object has all of these tags
        /// </summary>
        GameplayEffectAddRemoveTagContainer TargetRequiredTags { get; }

        /// <summary>
        /// Ability is blocked if target object has any of these tags
        /// </summary>
        GameplayEffectAddRemoveTagContainer TargetBlockedTags { get; }
    }
}
