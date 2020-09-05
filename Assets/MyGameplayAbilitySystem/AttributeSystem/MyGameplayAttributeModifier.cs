using GameplayAbilitySystem.AttributeSystem.Components;
using Unity.Entities;
using Unity.Mathematics;

namespace MyGameplayAbilitySystem
{
    public struct MyGameplayAttributeModifier : IComponentData, IGameplayAttributeModifier<MyAttributeModifierValues>
    {
        public half Value;
        public EMyPlayerAttribute Attribute;
        public EMyAttributeModifierOperator Operator;
        ref MyPlayerAttributes<float> GetAttributeCollection(ref MyAttributeModifierValues attributeModifier)
        {
            switch (Operator)
            {
                case EMyAttributeModifierOperator.Add:
                    return ref attributeModifier.AddValue;
                case EMyAttributeModifierOperator.Multiply:
                    return ref attributeModifier.MultiplyValue;
                case EMyAttributeModifierOperator.Divide:
                    return ref attributeModifier.DivideValue;
                default:
                    // Should never get here
                    return ref attributeModifier.AddValue;
            }
        }

        public void UpdateAttribute(ref MyAttributeModifierValues attributeModifier)
        {

            if (Operator == EMyAttributeModifierOperator.None) return;

            ref var attributeGroup = ref GetAttributeCollection(ref attributeModifier);

            switch (Attribute)
            {
                case EMyPlayerAttribute.Health:
                    attributeGroup.Health += Value;
                    break;
                case EMyPlayerAttribute.MaxHealth:
                    attributeGroup.MaxHealth += Value;
                    break;
                case EMyPlayerAttribute.Mana:
                    attributeGroup.Mana += Value;
                    break;
                case EMyPlayerAttribute.MaxMana:
                    attributeGroup.MaxMana += Value;
                    break;
                case EMyPlayerAttribute.Speed:
                    attributeGroup.Speed += Value;
                    break;
                default:
                    return;
            }

        }
    }
}
