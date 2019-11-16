/*
 * Created on Mon Nov 04 2019
 *
 * The MIT License (MIT)
 * Copyright (c) 2019 Sahil Jain
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using GameplayAbilitySystem.Attributes.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GameplayAbilitySystem.Attributes.Jobs {

    /// <summary>
    /// Creates a job which sums up the attributes for each entity, and stores the results in 
    /// the hashmap AttributeModifierValues.
    /// 
    /// The AttributeModifierValues hashmap can then be used in other jobs.  
    /// </summary>
        [BurstCompile]
        struct GetAttributeValuesJob_Sum<TOper, TAttribute> : IJobChunk
        where TOper : struct, IComponentData, IAttributeOperator
        where TAttribute : struct, IComponentData, IAttributeComponent {
            public NativeMultiHashMap<Entity, float>.ParallelWriter AttributeModifierValues;
            [ReadOnly] public ArchetypeChunkComponentType<AttributesOwnerComponent> owners;
            [ReadOnly] public ArchetypeChunkComponentType<AttributeModifier<TOper, TAttribute>> attributeModifiers;
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex) {
                var chunkOwners = chunk.GetNativeArray(owners);
                var chunkAttributeModifiers = chunk.GetNativeArray(attributeModifiers);
                for (var i = 0; i < chunk.Count; i++) {
                    var owner = chunkOwners[i];
                    var attributeModifier = chunkAttributeModifiers[i];
                    AttributeModifierValues.Add(owner, attributeModifier);
                }
            }
        }


}