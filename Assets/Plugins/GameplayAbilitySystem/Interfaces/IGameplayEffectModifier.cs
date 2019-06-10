using System;
using System.Collections.Generic;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Attributes;
using GameplayAbilitySystem.Enums;
using GameplayAbilitySystem.GameplayEffects;

namespace GameplayAbilitySystem.Interfaces
{

    /// <summary>
    /// This class defines how a  <see cref="GameplayEffect"/> modifies attributes (e.g. doing damage, healing)
    /// </summary>
    public interface IGameplayEffectModifier
    {
        /// <summary>
        /// The attribute to modify
        /// </summary>
        /// <value></value>
        AttributeType Attribute { get; }

        /// <summary>
        /// The operation (add, multiply, etc.) to apply to the attribute
        /// </summary>
        /// <value></value>
        EModifierOperationType ModifierOperation { get; }

        /// <summary>
        /// How the modification value is obtained
        /// </summary>
        /// <value></value>
        EMagnitudeCalculationTypes MagnitudeCalculationType { get; }

        /// <summary>
        /// Modification value for ScalableFloat type <see cref="EMagnitudeCalculationTypes"/>
        /// </summary>
        /// <value></value>
        float ScaledMagnitude { get; }

        /// <summary>
        /// <see cref="GameplayTag"/> that must be present/must not be present on the source of the <see cref="IGameplayAbilitySystem"/>
        /// </summary>
        /// <value></value>
        GameplayEffectModifierTagCollection SourceTags { get; }

        /// <see cref="GameplayTag"/> that must be present/must not be present on the target of the <see cref="IGameplayAbilitySystem"/>
        GameplayEffectModifierTagCollection TargetTags { get; }

        bool AttemptCalculateMagnitude(out float EvaluatedMagnitude);
    }

    /// <summary>
    /// Defines tags that must be present/must not be present on a <see cref="IGameplayAbilitySystem"/>
    /// </summary>
    [Serializable]
    public class GameplayEffectModifierTagCollection
    {
        /// <summary>
        /// <see cref="GameplayTag"/> that must be present on the source <see cref="IGameplayAbilitySystem"/>
        /// </summary>
        public List<GameplayTag> RequireTags;

        /// <summary>
        /// <see cref="GameplayTag"/> that must not be present on the target <see cref="IGameplayAbilitySystem"/>
        /// </summary>        
        public List<GameplayTag> IgnoreTags;
    }
}
