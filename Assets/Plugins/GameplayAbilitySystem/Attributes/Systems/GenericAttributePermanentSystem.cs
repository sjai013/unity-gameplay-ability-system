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

namespace GameplayAbilitySystem.Attributes.Systems {
    /// <summary>
    /// This is a generic attribute permanent modification system which can be used
    /// out of the box, and supports single attribute modifications, using 
    /// Add, Multiply, and Divide operators.
    /// 
    /// The added, multiplied, and divided values are summed for each entity first, and then
    /// the calculation formula is applied to each entity: 
    /// <br/>
    /// <code>
    /// BaseValue = Added + (BaseValue * [1 + multiplied - divided])
    /// </code>
    /// </summary>
    /// <typeparam name="TAttribute">The attribute this system modifies</typeparam>
    [UpdateInGroup(typeof(AttributeBaseValueGroup))]
    public class GenericAttributePermanentSystem<TAttributeTag> : GenericAttributeSystem<TAttributeTag, PermanentAttributeModifierTag>
        where TAttributeTag : struct, IAttributeComponent, IComponentData {
        BeginInitializationEntityCommandBufferSystem m_EntityCommandBuffer;

        protected override void OnCreate() {
            base.OnCreate();
            m_EntityCommandBuffer = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        [BurstCompile]
        [RequireComponentTag(typeof(AbilitySystemActorTransformComponent))]
        struct AttributeCombinerJob : IJobForEachWithEntity<TAttributeTag> {
            [ReadOnly] public NativeMultiHashMap<Entity, float> AddAttributes;
            // [ReadOnly] public NativeMultiHashMap<Entity, float> DivideAttributes;
            // [ReadOnly] public NativeMultiHashMap<Entity, float> MultiplyAttributes;

            public void Execute(Entity entity, int index, ref TAttributeTag attribute) {
                var added = SumFromNMHM(entity, AddAttributes);
                // var multiplied = SumFromNMHM(entity, MultiplyAttributes);
                // var divided = SumFromNMHM(entity, DivideAttributes);

                // Ignore multiplication.  I'm not sure how to handle add/multiply/divide given that order
                // of operations could be arbitrary, and therefore not repeatable.
                // For now, only allow addition, because that's repeatable.
                attribute.BaseValue = added + attribute.BaseValue; //* (1 + multiplied - divided);  

            }
            private float SumFromNMHM(Entity entity, NativeMultiHashMap<Entity, float> values) {
                values.TryGetFirstValue(entity, out var sum, out var multiplierIt);
                while (values.TryGetNextValue(out var tempSum, ref multiplierIt)) {
                    sum += tempSum;
                }
                return sum;
            }
        }

        // Destroy all entities that have PermanentAttributeTag
        [RequireComponentTag(typeof(PermanentAttributeModifierTag))]
        struct AttributeDestructionJob : IJobForEachWithEntity<AttributeComponentTag<TAttributeTag>> {

            public EntityCommandBuffer.Concurrent Ecb;
            public void Execute(Entity entity, int index, [ReadOnly] ref AttributeComponentTag<TAttributeTag> _) {
                Ecb.DestroyEntity(index, entity);
            }
        }

        protected override JobHandle ScheduleAttributeCombinerJob(JobHandle inputDeps) {
            inputDeps = new AttributeCombinerJob
            {
                AddAttributes = AttributeHashAdd,
                // DivideAttributes = AttributeHashDivide,
                // MultiplyAttributes = AttributeHashMultiply
            }.Schedule(this.actorsWithAttributesQuery, inputDeps);
            return inputDeps;
        }


        protected override JobHandle CleanupJob(JobHandle inputDeps) {
            inputDeps = new AttributeDestructionJob
            {
                Ecb = m_EntityCommandBuffer.CreateCommandBuffer().ToConcurrent(),
            }.Schedule(this, inputDeps);

            m_EntityCommandBuffer.AddJobHandleForProducer(inputDeps);
            return inputDeps;
        }
    }
}
