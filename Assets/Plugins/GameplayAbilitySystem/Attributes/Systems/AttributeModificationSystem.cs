using GameplayAbilitySystem.Attributes.Components;
using Unity.Entities;
using Unity.Jobs;

namespace GameplayAbilitySystem.Attributes.Systems {
    public abstract class AttributeModificationSystem<TOper, TAttribute> : JobComponentSystem
    where TAttribute : struct, IAttributeComponent
    where TOper : struct, IAttributeOperator {
        private EntityQuery Query;
        protected override void OnCreate() {
            this.Query = GetEntityQuery(ComponentType.ReadOnly<TAttribute>(), ComponentType.ReadOnly<TOper>(), ComponentType.ReadOnly<AttributeModifier<TOper,TAttribute>>());

        }
        protected override JobHandle OnUpdate(JobHandle inputDependencies) {
            inputDependencies = ScheduleJobs(this.Query, inputDependencies);
            return inputDependencies;
        }
        protected abstract JobHandle ScheduleJobs(EntityQuery query, JobHandle inputDependencies);
    }

}

