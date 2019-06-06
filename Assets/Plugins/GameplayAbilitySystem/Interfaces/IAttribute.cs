using GameplayAbilitySystem.Attributes;

namespace GameplayAbilitySystem.Interfaces
{
    /// <summary>
    /// Attributes are used to define parameters (such as health, speed) for a character.
    /// </summary>
    public interface IAttribute
    {

        /// <summary>
        /// Describes the type of this attribute
        /// </summary>
        /// <value></value>        
        AttributeType AttributeType { get; }

        /// <summary>
        /// The base value of the attribute, unaffected by e.g. buffs
        /// </summary>
        /// <value></value>
        float BaseValue { get; set; }

        /// <summary>
        /// This current value of the attribute, after application of temporary effects, e.g. buffs
        /// </summary>
        /// <value></value>        
        float CurrentValue { get; set; }


        /// <summary>
        /// Set the value of thie attribute
        /// </summary>
        /// <param name="AttributeSet"><see cref="IAttributeSet"/> this attribute belongs to</param>
        /// <param name="NewValue">New value of the attribute</param>
        void SetNumericValueChecked(IAttributeSet AttributeSet, ref float NewValue);
    }
}
