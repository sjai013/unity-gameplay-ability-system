using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using GameplayAbilitySystem.Attributes.Components;
using Operators = GameplayAbilitySystem.Attributes.Components.Operators;
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Add, ManaAttributeComponentTag>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Multiply, ManaAttributeComponentTag>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Divide, ManaAttributeComponentTag>))]

namespace GameplayAbilitySystem.Attributes.Components {
    public struct ManaAttributeComponent : IComponentData, IAttributeComponent {
        public int BaseValue;
        public int CurrentValue;
    }

    public struct ManaAttributeComponentTag : IComponentData, IAttributeComponent {}
}