using System.Linq;
using System.Collections.Generic;
using GameplayAbilitySystem.Enums;
using GameplayAbilitySystem.GameplayEffects;

namespace GameplayAbilitySystem.Interfaces
{

    /// <summary>
    /// This is used to define how long the <see cref="GameplayEffect"/> lasts, and what it does (e.g. how much damage)
    /// </summary>
    public interface IGameplayEffectPolicy
    {
        /// <summary>
        /// Whether the <see cref="GameplayEffect"/> lasts for some finite time, infinite time, or is applied instantly
        /// </summary>
        /// <value></value>
        EDurationPolicy DurationPolicy { get; }

        /// <summary>
        /// Duration of the <see cref="GameplayEffect"/>, if this lasts for some finite time
        /// </summary>
        /// <value></value>
        float DurationMagnitude { get; }

        /// <summary>
        /// How the <see cref="GameplayEffect"/> affects attributes
        /// </summary>
        /// <value></value>
        List<GameplayEffectModifier> Modifiers { get; }
    }

    /// <summary>
    /// Gameplay effect tags
    /// </summary>
    public interface IGameplayEffectTags
    {
        /// <summary>
        /// <see cref="GameplayTag"/> the <see cref="GameplayEffect"/> has
        /// </summary>
        /// <value></value>
        GameplayEffectAddRemoveTagContainer AssetTags { get; }

        /// <summary>
        /// <see cref="GameplayTag"/> that are given to the <see cref="IGameplayAbilitySystem"/>
        /// </summary>
        /// <value></value>
        GameplayEffectAddRemoveTagContainer GrantedTags { get; }

        /// <summary>
        /// <see cref="GameplayTag"/> that are required on the <see cref="IGameplayAbilitySystem"/> for the <see cref="GameplayEffect"> to have an effect.  
        /// If these <see cref="GameplayTag"/> are not on the <see cref="IGameplayAbilitySystem"/>, the effect is "disabled" until these <see cref="GameplayTag"/> are present.
        /// </summary>
        /// <value></value>
        GameplayEffectRequireIgnoreTagContainer OngoingTagRequirements { get; }

        /// <summary>
        /// These <see cref="GameplayTag"/> are required on the target to apply the <see cref="GameplayEffect"/>.  Once the <see cref="GameplayEffect"/> is applied,
        /// this has no effect.
        /// </summary>
        /// <value></value>
        GameplayEffectRequireIgnoreTagContainer ApplicationTagRequirements { get; }

        /// <summary>
        /// Removes any existing <see cref="GameplayEffect"/> that have these <see cref="GameplayTag"/>.
        /// </summary>
        /// <value></value>
        GameplayEffectAddRemoveTagContainer RemoveGameplayEffectsWithTag { get; }
    }

}
