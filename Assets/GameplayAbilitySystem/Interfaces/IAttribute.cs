using GameplayAbilitySystem.Attributes.Components;

namespace GameplayAbilitySystem.Interfaces {
    /// <summary>
    /// Attributes are used to define parameters (such as health, speed) for a character.
    /// </summary>
    public interface IAttribute {

        /// <summary>
        /// Describes the type of this attribute
        /// </summary>
        /// <value></value>        
        AttributeType AttributeType { get; }

        /// <summary>
        /// The base value of the attribute, unaffected by e.g. buffs
        /// </summary>
        /// <value></value>
        float BaseValue { get; }

        /// <summary>
        /// This current value of the attribute, after application of temporary effects, e.g. buffs
        /// </summary>
        /// <value></value>        
        float CurrentValue { get; }


        /// <summary>
        /// Set the current value of the attribute
        /// </summary>
        /// <param name="AttributeSet"><see cref="IAttributeSet"/> this attribute belongs to</param>
        /// <param name="NewValue">New value of the attribute</param>
        void SetAttributeCurrentValue(IAttributeSet AttributeSet, ref float NewValue);
    }
}
