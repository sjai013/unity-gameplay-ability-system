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
using GameplayAbilitySystem.Common.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace GameplayAbilitySystem.Attributes.Systems {
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

        protected abstract bool RunSystemThisFrame();

        protected override void OnDestroy() {
            AttributeHashAdd.Dispose();
            AttributeHashMultiply.Dispose();
            AttributeHashDivide.Dispose();
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
            if (nEntities == 0) {
                job = inputDependencies;
                return;
            };
            if (hashCapacity < nEntities) { // We need to increase hash capacity
                AttributeHash.Capacity = (int)(nEntities * 1.1);
            } else if (hashCapacity > nEntities * 4) { // We need to reduce hash capacity
                AttributeHash.Dispose();
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
            if (!RunSystemThisFrame()) return inputDependencies;
            ScheduleAttributeJob<Components.Operators.Add>(ref inputDependencies, ref this.Queries[0], ref AttributeHashAdd, out var addJob);
            ScheduleAttributeJob<Components.Operators.Multiply>(ref inputDependencies, ref this.Queries[1], ref AttributeHashMultiply, out var mulJob);
            ScheduleAttributeJob<Components.Operators.Divide>(ref inputDependencies, ref this.Queries[2], ref AttributeHashDivide, out var divideJob);
            inputDependencies = JobHandle.CombineDependencies(addJob, divideJob, mulJob);
            inputDependencies = ScheduleAttributeCombinerJob(inputDependencies);
            inputDependencies = CleanupJob(inputDependencies);
            return inputDependencies;
        }

        protected abstract JobHandle ScheduleAttributeCombinerJob(JobHandle inputDeps);
        protected abstract JobHandle CleanupJob(JobHandle inputDeps);


    }
}
