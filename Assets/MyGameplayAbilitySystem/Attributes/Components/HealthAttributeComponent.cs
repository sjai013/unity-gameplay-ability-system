using Unity.Entities;
using GameplayAbilitySystem.Attributes.Components;
using Operators = GameplayAbilitySystem.Attributes.Components.Operators;
using Unity.Jobs;
using GameplayAbilitySystem.Attributes.Systems;

[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Add, HealthAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Multiply, HealthAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Divide, HealthAttributeComponent>))]

namespace GameplayAbilitySystem.Attributes.Components {
    public struct HealthAttributeComponent : IComponentData, IAttributeComponent {
        public int BaseValue;
        public int CurrentValue;
    }
}




namespace GameplayAbilitySystem.Attributes.Systems {
    public class HealthAddAttributeModificationSystem : AttributeModificationSystem<Operators.Add, HealthAttributeComponent> {

        protected override JobHandle ScheduleJobs(EntityQuery query, JobHandle inputDependencies) {
            //throw new System.NotImplementedException();
            return inputDependencies;
        }
    }

}