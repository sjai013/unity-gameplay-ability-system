using GameplayAbilitySystem.GameplayEffects;
using GameplayAbilitySystem.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace GameplayAbilitySystem.Attributes.Components {
    /// <summary>
    /// Container for attribute change events
    /// </summary>
    [Serializable]
    public struct AttributeChangeData {

        /// <summary>
        /// <see cref="IAttribute"/> that was cahnged
        /// </summary>
        public IAttribute Attribute;


        // /// <summary>
        // /// Old value of the <see cref="IAttribute"/>
        // /// </summary>
        // public float OldValue;

        // /// <summary>
        // /// The <see cref="GameplayEffect"/> that caused the <see cref="IAttribute"/> change
        // /// </summary>
        // public GameplayEffect Effect;

        // /// <summary>
        // /// <see cref="IGameplayEffectModifier"> belonging to the <see cref="GameplayEffect"/> that caused this change.
        // /// We can extract the <see cref="IAttribute"/> from here to see what attribute was modified.
        // /// </summary>
        // public IGameplayEffectModifier Modifier;

        // /// <summary>
        // /// The target of this effect
        // /// </summary>
        // public IGameplayAbilitySystem Target;

    }

    [Serializable]
    public class AttributeChangeDataEvent : UnityEvent<AttributeChangeData> {

    }
}
