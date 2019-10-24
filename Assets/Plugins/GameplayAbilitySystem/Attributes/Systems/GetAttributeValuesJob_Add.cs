using GameplayAbilitySystem.Attributes.Components;
using GameplayAbilitySystem.Attributes.JobTypes;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Operators = GameplayAbilitySystem.Attributes.Components.Operators;

namespace GameplayAbilitySystem.Attributes.JobTypes {
    [BurstCompile]
    public struct GetAttributeValuesJob_Sum<TOper, TAttribute> : IJob
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

}