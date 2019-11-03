using GameplayAbilitySystem.Attributes.Components;
using GameplayAbilitySystem.Attributes.JobTypes;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Operators = GameplayAbilitySystem.Attributes.Components.Operators;

namespace GameplayAbilitySystem.Attributes.JobTypes {

    /// <summary>
    /// Creates a job which sums up the attributes for each entity, and stores the results in 
    /// the hashmap AttributeModifierValues.
    /// 
    /// The AttributeModifierValues hashmap can then be used in other jobs.  
    /// </summary>
    [BurstCompile]
    struct GetAttributeValuesJob_Sum<TOper, TAttribute> : IJobForEachWithEntity<AttributesOwnerComponent, AttributeModifier<TOper, TAttribute>>
    where TOper : struct, IComponentData, IAttributeOperator
    where TAttribute : struct, IComponentData, IAttributeComponent {
        public NativeMultiHashMap<Entity, float>.ParallelWriter AttributeModifierValues;
        public void Execute(Entity entity, int index, [ReadOnly] ref AttributesOwnerComponent owner, [ReadOnly] ref AttributeModifier<TOper, TAttribute> attributeModifier) {
            AttributeModifierValues.Add(owner, attributeModifier);
        }
    }


}