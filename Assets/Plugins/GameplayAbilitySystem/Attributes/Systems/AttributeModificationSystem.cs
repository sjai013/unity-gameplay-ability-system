using System.Collections.Generic;
using GameplayAbilitySystem.Attributes.Components;
using Unity.Entities;
using Unity.Jobs;

namespace GameplayAbilitySystem.Attributes.Systems {
    public abstract class AttributeModificationSystem<TAttribute> : JobComponentSystem
    where TAttribute : struct, IAttributeComponent, IComponentData {
        protected List<EntityQuery> Queries;
        protected EntityQuery actorsWithAttributesQuery;
        protected EntityQuery CreateQuery<TOper>()
        where TOper : struct, IAttributeOperator, IComponentData {
            return GetEntityQuery(
                ComponentType.ReadOnly<AttributeComponentTag<TAttribute>>(),
                ComponentType.ReadOnly<TOper>(),
                ComponentType.ReadOnly<AttributeModifier<TOper, TAttribute>>(),
                ComponentType.ReadOnly<AttributesOwnerComponent>()
                );
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies) {
            inputDependencies = ScheduleJobs(inputDependencies);
            return inputDependencies;
        }
        protected abstract JobHandle ScheduleJobs(JobHandle inputDependencies);
    }

}


