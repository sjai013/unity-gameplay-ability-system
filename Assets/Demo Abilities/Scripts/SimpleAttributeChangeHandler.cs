using GameplayAbilitySystem.Interfaces;
using UnityEngine;


namespace GameplayAbilitySystem.Attributes
{
    [CreateAssetMenu(fileName = "Simple Attribute Change Handler", menuName = "Ability System Demo/Attributes/Simple Attribute Change Handler")]
    public class SimpleAttributeChangeHandler : BaseAttributeChangeHandler
    {
        public AttributeType Health;
        public AttributeType Mana;

        public override void OnAttributeChange(IAttribute Attribute, ref float Value)
        {
            if (Attribute.AttributeType == Health)
            {
                HandleHealthChange(ref Value);
            }
            else if (Attribute.AttributeType == Mana)
            {
                HandleManaChange(ref Value);
            }
        }

        private void HandleHealthChange(ref float Value)
        {
            if (Value < 0) Value = 0;
        }

        private void HandleManaChange(ref float Value)
        {
            if (Value < 0) Value = 0;
        }


    }
}
