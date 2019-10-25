using System.Collections.Generic;
using GameplayAbilitySystem.Attributes.Components;
using Unity.Entities;
using Unity.Jobs;

namespace GameplayAbilitySystem.Attributes.Systems {
    
    /// <summary>
    /// This is the base for all attribute modification systems.  
    /// Custom attribute modification types should inherit from this class
    /// and modify as necessary.
    /// 
    /// See <see cref="GenericAttributeSystem{TAttributeTag}"> for a sample modifier system
    /// </summary>
    /// <typeparam name="TAttribute">The attribute this system modifies</typeparam>
    public abstract class AttributeModificationSystem<TAttribute> : JobComponentSystem
    where TAttribute : struct, IAttributeComponent, IComponentData {

        /// <summary>
        /// This is the list of queries that are use
        /// </summary>
        protected EntityQuery[] Queries = new EntityQuery[3];
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


