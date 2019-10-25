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
    /// 
    /// Note that the hashmap is read and written to, so other jobs should not access this
    /// until this job has completed.
    /// </summary>
    [BurstCompile]
    public struct GetAttributeValuesJob_Sum1<TOper, TAttribute> : IJob
where TOper : struct, IAttributeOperator, IComponentData
where TAttribute : struct, IAttributeComponent, IComponentData {
        public NativeHashMap<Entity, float> AttributeModifierValues;
        [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<AttributesOwnerComponent> attributeOwners;
        [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<AttributeModifier<TOper, TAttribute>> attributeModifiers;
        public void Execute() {
            for (var i = 0; i < attributeOwners.Length; i++) {
                // if (!AttributeModifierValues.TryGetValue(attributeOwners[i], out var val)) {
                //     AttributeModifierValues.TryAdd(attributeOwners[i], 0f);
                // }
                // AttributeModifierValues[attributeOwners[i]] += attributeModifiers[i];

                if (!AttributeModifierValues.TryAdd(attributeOwners[i], attributeModifiers[i])) {
                    AttributeModifierValues[attributeOwners[i]] += attributeModifiers[i];
                }
            }
        }
    }


    [BurstCompile]
    struct GetAttributeValuesJob_Sum2<TOper, TAttribute> : IJobForEachWithEntity<AttributesOwnerComponent, AttributeModifier<TOper, TAttribute>>
    where TOper : struct, IComponentData, IAttributeOperator
    where TAttribute : struct, IComponentData, IAttributeComponent {
        public NativeMultiHashMap<Entity, float>.ParallelWriter AttributeModifierValues;
        public void Execute(Entity entity, int index, [ReadOnly] ref AttributesOwnerComponent owner, [ReadOnly] ref AttributeModifier<TOper, TAttribute> attributeModifier) {
            AttributeModifierValues.Add(owner, attributeModifier);
        }
    }


}