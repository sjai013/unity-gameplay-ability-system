using GameplayAbilitySystem.Attributes.Components;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;

namespace GameplayAbilitySystem {

    /// <inheritdoc />
    [AddComponentMenu("Ability System/Attributes/Attribute")]
    [System.Serializable]
    public class Attribute : IAttribute {


        [SerializeField]
        AttributeType _attributeType;

        [SerializeField]
        float _baseValue;

        [SerializeField]
        float _currentValue;

        /// <inheritdoc />
        public float BaseValue { get => _baseValue; }

        /// <inheritdoc />
        public float CurrentValue { get => _currentValue; }

        /// <inheritdoc />
        public AttributeType AttributeType { get => _attributeType; set => _attributeType = AttributeType; }

        /// <inheritdoc />
        public void SetAttributeCurrentValue(IAttributeSet AttributeSet, ref float NewValue) {
            AttributeSet.PreAttributeChange(this, ref NewValue);
            _currentValue = NewValue;
            AttributeSet.AttributeCurrentValueChanged.Invoke(new AttributeChangeData()
            {
                Attribute = this
            });
        }

        public void SetAttributeBaseValue(IAttributeSet AttributeSet, ref float NewValue) {
            AttributeSet.PreAttributeBaseChange(this, ref NewValue);
            _baseValue = NewValue;
            AttributeSet.AttributeBaseValueChanged.Invoke(new AttributeChangeData()
            {
                Attribute = this
            });
        }
    }
}
