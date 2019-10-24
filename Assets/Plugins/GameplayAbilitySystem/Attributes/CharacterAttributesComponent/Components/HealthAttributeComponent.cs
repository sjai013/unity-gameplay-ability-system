using Unity.Entities;
using GameplayAbilitySystem.Attributes.Components;
using Operators = GameplayAbilitySystem.Attributes.Components.Operators;

[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Add, HealthAttributeComponentTag>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Multiply, HealthAttributeComponentTag>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Divide, HealthAttributeComponentTag>))]

namespace GameplayAbilitySystem.Attributes.Components {
    public struct HealthAttributeComponent : IComponentData {
        public int BaseValue;
        public int CurrentValue;
    }

    public struct HealthAttributeComponentTag : IComponentData, IAttributeComponent {}
}


namespace GameplayAbilitySystem.Attributes.Systems {

    // public class HealthAddAttributeModificationSystem : AttributeModificationSystem_Add<HealthAttributeComponent> { }

}