using Unity.Entities;
using GameplayAbilitySystem.Attributes.Components;
using Operators = GameplayAbilitySystem.Attributes.Components.Operators;
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Add, MaxManaAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Multiply, MaxManaAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Divide, MaxManaAttributeComponent>))]

namespace GameplayAbilitySystem.Attributes.Components {
    public struct MaxManaAttributeComponent : IComponentData, IAttributeComponent {
        public float BaseValue { get; set; }
        public float CurrentValue { get; set; }
    }
}
