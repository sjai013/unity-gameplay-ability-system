using System.Collections.Generic;
using GameplayAbilitySystem.GameplayEffects;

namespace GameplayAbilitySystem.Interfaces {
    /// <summary>
    /// This is used to represent cooldowns for a <see cref="IGameplayAbility"/>.  
    /// Multiple <see cref="IGameplayCooldown"/> can be set for an ability to represent multiple
    /// shared cooldowns (e.g. a global cooldown shared by all abilities, and a <see cref="IGameplayAbility"/> specific cooldown)
    /// </summary>
    public interface IGameplayCooldown {
        /// <summary>
        /// The descriptor for the cooldown.  The tags granted by this are used to determine cooldowns
        /// </summary>
        /// <value></value>
        GameplayEffect CooldownGameplayEffect { get; }

        /// <summary>
        /// Gets the list of cooldown tags that this applies
        /// </summary>
        /// <returns></returns>
        IEnumerable<GameplayTag> GetCooldownTags();
    }
}
