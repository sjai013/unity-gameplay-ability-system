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
using GameplayAbilitySystem.Common.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace GameplayAbilitySystem.Attributes.Systems {
    /// <summary>
    /// This is a generic attribute temporary modification system which can be used
    /// out of the box, and supports single attribute modifications, using 
    /// Add, Multiply, and Divide operators.
    /// 
    /// The added, multiplied, and divided values are summed for each entity first, and then
    /// the calculation formula is applied to each entity: 
    /// <br/>
    /// <code>
    /// CurrentValue = Added + (BaseValue * [1 + multiplied - divided])
    /// </code>
    /// </summary>
    /// <typeparam name="TAttribute">The attribute this system modifies</typeparam>
    [UpdateInGroup(typeof(AttributeCurrentValueGroup))]
    public class GenericAttributeTemporarySystem<TAttributeTag> : AttributeModificationSystem<TAttributeTag>
        where TAttributeTag : struct, IAttributeComponent, IComponentData {

        protected override void OnCreate() {
            base.OnCreate();
        }

        [BurstCompile]
        struct AttributeCombinerJob2 : IJobForEachWithEntity_EBC<AttributeBufferElement<TemporaryAttributeModifierTag, TAttributeTag>, TAttributeTag> {
            [ReadOnly]
            public ComponentDataFromEntity<AttributeModifier<Components.Operators.Add, TAttributeTag>> Add_CDFE;
            [ReadOnly]
            public ComponentDataFromEntity<AttributeModifier<Components.Operators.Multiply, TAttributeTag>> Mul_CDFE;
            [ReadOnly]
            public ComponentDataFromEntity<AttributeModifier<Components.Operators.Divide, TAttributeTag>> Div_CDFE;

            public void Execute(Entity entity, int index, [ReadOnly] DynamicBuffer<AttributeBufferElement<TemporaryAttributeModifierTag, TAttributeTag>> attributeBuffer, ref TAttributeTag attribute) {
                var added = 0f;
                var multiplied = 0f;
                var divided = 0f;
                for (var i = 0; i < attributeBuffer.Length; i++) {
                    var attributeEntity = attributeBuffer[i].Value;
                    if (Add_CDFE.HasComponent(attributeEntity)) {
                        added += Add_CDFE[attributeEntity];
                    }

                    if (Div_CDFE.HasComponent(attributeEntity)) {
                        added += Div_CDFE[attributeEntity];
                    }

                    if (Mul_CDFE.HasComponent(attributeEntity)) {
                        added += Mul_CDFE[attributeEntity];
                    }
                }
                attribute.CurrentValue = added + attribute.BaseValue * (1 + multiplied - divided);
            }
        }

        protected override JobHandle ScheduleJobs(JobHandle inputDeps) {
            // inputDeps = new AttributeCombinerJob
            // {
            //     AddAttributes = AttributeHashAdd,
            //     DivideAttributes = AttributeHashDivide,
            //     MultiplyAttributes = AttributeHashMultiply
            // }.Schedule(this.actorsWithAttributesQuery, inputDeps);
            inputDeps = new AttributeCombinerJob2
            {
                Add_CDFE = GetComponentDataFromEntity<AttributeModifier<Components.Operators.Add, TAttributeTag>>(true),
                Mul_CDFE = GetComponentDataFromEntity<AttributeModifier<Components.Operators.Multiply, TAttributeTag>>(true),
                Div_CDFE = GetComponentDataFromEntity<AttributeModifier<Components.Operators.Divide, TAttributeTag>>(true)
            }.Schedule(this, inputDeps);
            return inputDeps;
        }

    }
}
