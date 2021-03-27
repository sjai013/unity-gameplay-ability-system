using System;
using AbilitySystem.ModifierMagnitude;

namespace AbilitySystem
{
    [Serializable]
    public struct GameplayEffectDefinitionContainer
    {
        /// <summary>
        /// The duration of this GE.  Instant GE are applied immediately and then removed, and Infinite and Has Duration are persistent and remain applied.
        /// </summary>
        public EDurationPolicy DurationPolicy;

        public ModifierMagnitudeScriptableObject DurationModifier;

        /// <summary>
        /// The duration of this GE, if the GE has a finite duration
        /// </summary>
        public float DurationMultiplier;


        /// <summary>
        /// The attribute modifications that this GE provides
        /// </summary>
        public GameplayEffectModifier[] Modifiers;

        /// <summary>
        /// Other GE to apply to the source ability system, based on presence of tags on source
        /// </summary>
        public ConditionalGameplayEffectContainer[] ConditionalGameplayEffects;
    }

}
