using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using GameplayAbilitySystem.Attributes.Components;
using Operators = GameplayAbilitySystem.Attributes.Components.Operators;
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Add, ManaAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Multiply, ManaAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Divide, ManaAttributeComponent>))]

namespace GameplayAbilitySystem.Attributes.Components {
    public struct ManaAttributeComponent : IComponentData, IAttributeComponent {
        public int BaseValue;
        public int CurrentValue;

    }

}