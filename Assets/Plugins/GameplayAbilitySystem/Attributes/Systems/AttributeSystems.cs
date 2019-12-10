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
using GameplayAbilitySystem.Attributes.Jobs;
using GameplayAbilitySystem.Common;
using GameplayAbilitySystem.Common.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace GameplayAbilitySystem.Attributes.Systems {
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class AttributeSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(AttributeSystemGroup))]
    public class AttributeBaseValueGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(AttributeSystemGroup))]
    [UpdateAfter(typeof(AttributeBaseValueGroup))]
    public class AttributeCurrentValueGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(AttributeSystemGroup))]
    public abstract class GenericAttributeSystem<TAttributeTag, TAttributeModifierTag> : AttributeModificationSystem<TAttributeTag>
        where TAttributeTag : struct, IAttributeComponent, IComponentData
        where TAttributeModifierTag : IAttributeModifierTag, new() {
        protected NativeMultiHashMap<Entity, float> AttributeHashAdd = new NativeMultiHashMap<Entity, float>(0, Allocator.Persistent);
        protected NativeMultiHashMap<Entity, float> AttributeHashMultiply = new NativeMultiHashMap<Entity, float>(0, Allocator.Persistent);
        protected NativeMultiHashMap<Entity, float> AttributeHashDivide = new NativeMultiHashMap<Entity, float>(0, Allocator.Persistent);
        protected override void OnCreate() {
            var attributeModifierTag = new TAttributeModifierTag();
            this.Queries[0] = GetEntityQuery(
                  attributeModifierTag.AttributeOperatorQueryComponents<TAttributeTag, Components.Operators.Add>()
              );
            this.Queries[1] = GetEntityQuery(
                attributeModifierTag.AttributeOperatorQueryComponents<TAttributeTag, Components.Operators.Multiply>()
            );
            this.Queries[2] = GetEntityQuery(
                attributeModifierTag.AttributeOperatorQueryComponents<TAttributeTag, Components.Operators.Divide>()
            );

            this.actorsWithAttributesQuery = GetEntityQuery(
                ComponentType.ReadOnly<AbilitySystemActorTransformComponent>(),
                ComponentType.ReadWrite<TAttributeTag>()
                );
        }



        /// <summary>
        /// Schedules an attribute job
        /// </summary>
        /// <param name="inputDependencies">JobHandle</param>
        /// <param name="query">The EntityQuery used for filtering group</param>
        /// <param name="AttributeHash">Attribute MultiHashMap mapping entity to attribute value</param>
        /// <param name="job">Returned job handle</param>
        /// <typeparam name="TOper">The type of operator for this attribute job</typeparam>
        private void ScheduleAttributeJob<TOper>(ref JobHandle inputDependencies, ref EntityQuery query, ref NativeMultiHashMap<Entity, float> AttributeHash, out JobHandle job)
        where TOper : struct, IAttributeOperator, IComponentData {
            var nEntities = query.CalculateEntityCount();
            var hashCapacity = AttributeHash.Capacity;
            AttributeHash.Clear();
            if (hashCapacity < nEntities) { // We need to increase hash capacity
                AttributeHash.Capacity = (int)(nEntities * 1.1);
            } else if (hashCapacity > nEntities * 4) { // We need to reduce hash capacity
                AttributeHash = new NativeMultiHashMap<Entity, float>(nEntities, Allocator.Persistent);
            }
            // // AttributeHash = new NativeMultiHashMap<Entity, float>(query.CalculateEntityCount(), Allocator.TempJob);
            inputDependencies = new GetAttributeValuesJob_Sum<TOper, TAttributeTag>
            {
                owners = GetArchetypeChunkComponentType<AttributesOwnerComponent>(false),
                attributeModifiers = GetArchetypeChunkComponentType<AttributeModifier<TOper, TAttributeTag>>(false),
                AttributeModifierValues = AttributeHash.AsParallelWriter()
            }.Schedule(query, inputDependencies);
            job = inputDependencies;
        }


        protected override JobHandle ScheduleJobs(JobHandle inputDependencies) {
            ScheduleAttributeJob<Components.Operators.Add>(ref inputDependencies, ref this.Queries[0], ref AttributeHashAdd, out var addJob);
            ScheduleAttributeJob<Components.Operators.Multiply>(ref inputDependencies, ref this.Queries[1], ref AttributeHashMultiply, out var mulJob);
            ScheduleAttributeJob<Components.Operators.Divide>(ref inputDependencies, ref this.Queries[2], ref AttributeHashDivide, out var divideJob);
            inputDependencies = JobHandle.CombineDependencies(addJob, divideJob, mulJob);
            inputDependencies = ScheduleAttributeCombinerJob(inputDependencies);
            inputDependencies = CleanupJob(inputDependencies);

            // inputDependencies = new AttributeCombinerJob
            // {
            //     AddAttributes = AttributeHashAdd,
            //     DivideAttributes = AttributeHashDivide,
            //     MultiplyAttributes = AttributeHashMultiply
            // }.Schedule(this.actorsWithAttributesQuery, inputDependencies);


            return inputDependencies;
        }

        protected abstract JobHandle ScheduleAttributeCombinerJob(JobHandle inputDeps);
        protected abstract JobHandle CleanupJob(JobHandle inputDeps);


    }

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
    public class GenericAttributeTemporarySystem<TAttributeTag> : GenericAttributeSystem<TAttributeTag, TemporaryAttributeModifierTag>
        where TAttributeTag : struct, IAttributeComponent, IComponentData {

        [BurstCompile]
        [RequireComponentTag(typeof(AbilitySystemActorTransformComponent))]
        struct AttributeCombinerJob : IJobForEachWithEntity<TAttributeTag> {
            [ReadOnly] public NativeMultiHashMap<Entity, float> AddAttributes;
            [ReadOnly] public NativeMultiHashMap<Entity, float> DivideAttributes;
            [ReadOnly] public NativeMultiHashMap<Entity, float> MultiplyAttributes;

            public void Execute(Entity entity, int index, ref TAttributeTag attribute) {
                var added = SumFromNMHM(entity, AddAttributes);
                var multiplied = SumFromNMHM(entity, MultiplyAttributes);
                var divided = SumFromNMHM(entity, DivideAttributes);

                attribute.CurrentValue = added + attribute.BaseValue * (1 + multiplied - divided);
            }
            private float SumFromNMHM(Entity entity, NativeMultiHashMap<Entity, float> values) {
                values.TryGetFirstValue(entity, out var sum, out var multiplierIt);
                while (values.TryGetNextValue(out var tempSum, ref multiplierIt)) {
                    sum += tempSum;
                }
                return sum;
            }
        }
        protected override JobHandle ScheduleAttributeCombinerJob(JobHandle inputDeps) {
            inputDeps = new AttributeCombinerJob
            {
                AddAttributes = AttributeHashAdd,
                DivideAttributes = AttributeHashDivide,
                MultiplyAttributes = AttributeHashMultiply
            }.Schedule(this.actorsWithAttributesQuery, inputDeps);
            return inputDeps;
        }

        protected override JobHandle CleanupJob(JobHandle inputDeps) {
            return inputDeps;
        }
    }


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
            return inputDeps;
        }
    }
}
