using System.Collections.Generic;

namespace GameplayAbilitySystem.Interfaces
{
    public interface IAbilityTags
    {
        /// <summary>
        /// Any additional tags that the ability has
        /// </summary>
        List<GameplayTag> AbilityTags { get; }

        /// <summary>
        /// Abilities with these tags are cancelled
        /// </summary>
        List<GameplayTag> CancelAbilitiesWithTags { get; }

        /// <summary>
        /// Abilities with these tags are blocked
        /// </summary>
        List<GameplayTag> BlockAbilitiesWithTags { get; }

        /// <summary>
        /// Tags to apply to activating owner while this ability is active
        /// </summary>
        List<GameplayTag> ActivationOwnedTags { get; }

        /// <summary>
        /// Ability can only be activated if the activating object has all of these tags
        /// </summary>
        List<GameplayTag> ActivationRequiredTags { get; }

        /// <summary>
        /// Ability is blocked if activating object has any of these tags
        /// </summary>
        List<GameplayTag> ActivationBlockedTags { get; }

        /// <summary>
        /// Ability can only be activated if source object has all of these tags
        /// </summary>
        List<GameplayTag> SourceRequiredTags { get; }

        /// <summary>
        /// Ability is blocked if source object has any of these tags
        /// </summary>
        List<GameplayTag> SourceBlockedTags { get; }

        /// <summary>
        /// Ability can only be activated if target object has all of these tags
        /// </summary>
        List<GameplayTag> TargetRequiredTags { get; }

        /// <summary>
        /// Ability is blocked if target object has any of these tags
        /// </summary>
        List<GameplayTag> TargetBlockedTags { get; }
    }
}
