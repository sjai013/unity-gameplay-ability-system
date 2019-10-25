using Unity.Entities;
using GameplayAbilitySystem.Attributes.Components;
using Operators = GameplayAbilitySystem.Attributes.Components.Operators;

[assembly: RegisterGenericComponentType(typeof(AttributeComponentTag<HealthAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Add, HealthAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Multiply, HealthAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Divide, HealthAttributeComponent>))]


namespace GameplayAbilitySystem.Attributes.Components {
    public struct HealthAttributeComponent : IComponentData, IAttributeComponent {
        public float BaseValue { get; set; }
        public float CurrentValue { get; set; }
    }
}