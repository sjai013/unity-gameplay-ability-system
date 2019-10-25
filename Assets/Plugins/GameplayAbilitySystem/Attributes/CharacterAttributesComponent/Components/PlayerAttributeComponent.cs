using Unity.Entities;
using GameplayAbilitySystem.Attributes.Components;
using Operators = GameplayAbilitySystem.Attributes.Components.Operators;
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Add, CharacterLevelAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Multiply, CharacterLevelAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Divide, CharacterLevelAttributeComponent>))]

namespace GameplayAbilitySystem.Attributes.Components {
    public struct CharacterLevelAttributeComponent : IComponentData, IAttributeComponent {
        public float BaseValue { get; set; }
        public float CurrentValue { get; set; }
    }
}