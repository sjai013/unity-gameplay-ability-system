using GameplayAbilitySystem.Attributes;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;

namespace GameplayAbilitySystem
{

    /// <inheritdoc />
    [AddComponentMenu("Ability System/Attributes/Attribute")]
    [System.Serializable]
    public class Attribute : IAttribute
    {
        [SerializeField]
        AttributeType _attributeType;

        [SerializeField]
        float _baseValue;

        [SerializeField]
        float _currentValue;

        /// <inheritdoc />
        public float BaseValue { get => _baseValue; set => _baseValue = value; }

        /// <inheritdoc />
        public float CurrentValue { get => _currentValue; set => _currentValue = value; }

        /// <inheritdoc />
        public AttributeType AttributeType { get => _attributeType; set => _attributeType = AttributeType; }

        /// <inheritdoc />
        public void  SetAttributeCurrentValue(IAttributeSet AttributeSet, ref float NewValue)
        {
            AttributeSet.PreAttributeChange(this, ref NewValue);
            this.CurrentValue = NewValue;
        }
    }
}
