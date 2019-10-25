using Unity.Entities;
using GameplayAbilitySystem.Attributes.Components;
using Operators = GameplayAbilitySystem.Attributes.Components.Operators;

[assembly: RegisterGenericComponentType(typeof(AttributeComponentTag<ManaAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Add, ManaAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Multiply, ManaAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Divide, ManaAttributeComponent>))]

namespace GameplayAbilitySystem.Attributes.Components {
    public struct ManaAttributeComponent : IComponentData, IAttributeComponent {
        public float BaseValue { get; set; }
        public float CurrentValue { get; set; }
    }
}