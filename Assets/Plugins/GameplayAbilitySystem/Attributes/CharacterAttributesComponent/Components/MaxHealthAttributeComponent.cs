using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using GameplayAbilitySystem.Attributes.Components;
using Operators = GameplayAbilitySystem.Attributes.Components.Operators;
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Add, MaxHealthAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Multiply, MaxHealthAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Divide, MaxHealthAttributeComponent>))]

namespace GameplayAbilitySystem.Attributes.Components {
    public struct MaxHealthAttributeComponent : IComponentData, IAttributeComponent {
        public float BaseValue { get; set; }
        public float CurrentValue { get; set; }

    }
}
