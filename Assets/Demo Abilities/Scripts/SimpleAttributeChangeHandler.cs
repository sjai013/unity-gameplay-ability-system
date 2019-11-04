using System.Linq;
using GameplayAbilitySystem.Attributes.Components;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;


namespace AbilitySystemDemo.Attributes {
    [CreateAssetMenu(fileName = "Simple Attribute Change Handler", menuName = "Ability System Demo/Attributes/Simple Attribute Change Handler")]
    public class SimpleAttributeChangeHandler : BaseAttributeChangeHandler {
        public AttributeType MaxHealth;
        public AttributeType Health;
        public AttributeType MaxMana;
        public AttributeType Mana;

        public override void OnAttributeChange(IAttributeSet AttributeSet, IAttribute Attribute, ref float Value) {
            if (Attribute.AttributeType == Health) {
                HandleHealthChange(ref Value, AttributeSet.Attributes.First(x => x.AttributeType == MaxHealth).CurrentValue);
            } else if (Attribute.AttributeType == Mana) {
                HandleManaChange(ref Value, AttributeSet.Attributes.First(x => x.AttributeType == MaxMana).CurrentValue);
            }
        }

        private void HandleHealthChange(ref float Value, float maxValue) {
            Value = Mathf.Clamp(Value, 0, maxValue);
        }

        private void HandleManaChange(ref float Value, float maxValue) {
            Value = Mathf.Clamp(Value, 0, maxValue);
        }


    }
}
