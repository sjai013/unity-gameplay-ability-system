using GameplayAbilitySystem.GameplayEffects;

namespace GameplayAbilitySystem.Interfaces
{
    /// <summary>
    /// Represents the cost of a <see cref="IGameplayAbility"/>.
    /// The <see cref="GameplayEffect"/> defines the actual cost of the ability by describing which <see cref="IAttribute"/> are reduced on the 
    /// casting <see cref="IGameplayAbilitySystem"/> (e.g. mana)
    /// </summary>
    public interface IGameplayCost
    {
        /// <summary>
        /// Cost definition for a <see cref="IGameplayAbility"/>
        /// </summary>
        /// <value></value>
        GameplayEffect CostGameplayEffect { get; }
    }
}
